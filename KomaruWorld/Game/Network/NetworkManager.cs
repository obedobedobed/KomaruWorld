using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld
{
    public class NetworkManager
    {
        public static NetworkManager Instance { get; private set; }

        private EventBasedNetListener _listener;
        private NetManager _netManager;
        private NetPacketProcessor _packetProcessor;
        
        private NetDataWriter _dataWriter = new NetDataWriter();
        
        // Cache textures to reuse for remote players
        private Texture2D _cachedPlayerTexture;
        private Texture2D _cachedSlotTexture;

        // Throttling for logs
        private int _logThrottleCounter = 0;

        public NetPeer ServerPeer { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsHost { get; private set; }

        public Dictionary<int, Player> RemotePlayers = new Dictionary<int, Player>();

        public NetworkManager()
        {
            Instance = this;
            _listener = new EventBasedNetListener();
            _packetProcessor = new NetPacketProcessor();

            // --- CALLBACKS ---

            _listener.ConnectionRequestEvent += request =>
            {
                if (_netManager.ConnectedPeersCount < 10)
                    request.AcceptIfKey("KomaruSecret");
                else
                    request.Reject();
            };

            _listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine($"Connection established: Peer {peer.Id}");
                //FileLogger.Log($"Connection established: Peer {peer.Id}");
                if (!IsHost)
                {
                    ServerPeer = peer;
                }
                else
                {
                    // Host: Send world data to the newly connected client
                    //FileLogger.Log($"Sending world data to new client {peer.Id}");
                    SendWorldData(peer);
                }
            };

            _listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                try
                {
                    // DEBUG: Log that we got bytes (helps identify if packets are dropped physically or logically)
                    // Only log periodically to avoid spam, or if it's a specific size if you want to target mob packets
                    // Mob packets are likely small.
                    // int size = dataReader.AvailableBytes;
                    // if (size > 0 && _logThrottleCounter % 60 == 0) FileLogger.Log($"[RAW-NET] Received {size} bytes from {fromPeer.Id}");

                    _packetProcessor.ReadAllPackets(dataReader, fromPeer);
                }
                catch (ParseException pex) 
                {
                    //FileLogger.Log($"[NET-ERR] Parse Exception (Unknown Packet?): {pex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading packet: {ex.Message}");
                    //FileLogger.Log($"Error reading packet: {ex.Message}\n{ex.StackTrace}");
                }
            };

            // --- PACKET HANDLERS ---
            
            _packetProcessor.SubscribeReusable<PacketPlayerPosition, NetPeer>(OnPlayerPositionReceived);
            _packetProcessor.SubscribeReusable<PacketBlockChange, NetPeer>(OnBlockChangeReceived);
            _packetProcessor.SubscribeReusable<PacketWorldSync, NetPeer>(OnWorldSyncReceived);
            _packetProcessor.SubscribeReusable<PacketMobPosition, NetPeer>(OnMobPositionReceived);
            _packetProcessor.SubscribeReusable<PacketTileDamage, NetPeer>(OnTileDamageReceived);
            _packetProcessor.SubscribeReusable<PacketItemDrop, NetPeer>(OnItemDropReceived);
            _packetProcessor.SubscribeReusable<PacketItemPickup, NetPeer>(OnItemPickupReceived);
        }

        public void StartHost(int port = 9050)
        {
            if (IsRunning) return;

            //FileLogger.Initialize("server");
            //FileLogger.Log("Starting host...");
            
            _netManager = new NetManager(_listener);
            _netManager.Start(port);

            IsHost = true;
            IsRunning = true;
            Console.WriteLine($"Host started on port {port}");
            //FileLogger.Log($"Host started on port {port}");
        }

        public void StartClient(string address, int port = 9050)
        {
            if (IsRunning) return;

            FileLogger.Initialize("client"); 
            //FileLogger.Log($"Connecting to {address}:{port}...");
            
            _netManager = new NetManager(_listener);
            _netManager.Start();
            _netManager.Connect(address, port, "KomaruSecret");

            IsHost = false;
            IsRunning = true;
            Console.WriteLine($"Connecting to {address}:{port}...");
            //FileLogger.Log("Client started");
        }

        public void Stop()
        {
            if (!IsRunning) return;
            _netManager.Stop();
            IsRunning = false;
            IsHost = false;
            RemotePlayers.Clear();
        }

        public void Update()
        {
            if (IsRunning)
                _netManager.PollEvents();
        }

        // --- SENDING LOGIC ---

        public void SendPosition(Vector2 position, int frame, bool flip)
        {
            if (!IsRunning) return;
            if (!IsHost && ServerPeer == null) return;

            // Debug Logging (Throttled)
            _logThrottleCounter++;
            if (_logThrottleCounter % 60 == 0)
            {
                string role = IsHost ? "HOST" : "CLIENT";
                //FileLogger.Log($"[NET-OUT] {role} Sending Pos: {position.X:F1}, {position.Y:F1} | Frame: {frame}");
            }

            var packet = new PacketPlayerPosition
            {
                PlayerId = IsHost ? -1 : 0,
                X = position.X,
                Y = position.Y,
                Frame = frame,
                Flip = flip
            };

            _dataWriter.Reset();
            _packetProcessor.Write(_dataWriter, packet);

            if (IsHost)
                _netManager.SendToAll(_dataWriter, DeliveryMethod.Sequenced);
            else if (ServerPeer != null)
                ServerPeer.Send(_dataWriter, DeliveryMethod.Sequenced);
        }

        public void SendBlockChange(Vector2 pos, Tiles type, bool place)
        {
            if (!IsRunning) return;
            if (!IsHost && ServerPeer == null) return;
            
            //FileLogger.Log($"[NET-OUT] Block Change: {type} at {pos} Place: {place}");

            var packet = new PacketBlockChange 
            { 
                X = pos.X, 
                Y = pos.Y, 
                TileType = type, 
                IsPlacement = place 
            };

            _dataWriter.Reset();
            _packetProcessor.Write(_dataWriter, packet);

            if (IsHost)
                _netManager.SendToAll(_dataWriter, DeliveryMethod.ReliableOrdered);
            else if (ServerPeer != null)
                ServerPeer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
        }

        public void SendMobPosition(Mob mob)
        {
            if (!IsRunning || !IsHost) return;

            var packet = new PacketMobPosition
            {
                MobId = mob.NetworkId,
                X = mob.Position.X,
                Y = mob.Position.Y,
                Frame = mob.frame,
                Flip = (mob.Flip == SpriteEffects.FlipHorizontally)
            };

            _dataWriter.Reset();
            _packetProcessor.Write(_dataWriter, packet);
            _netManager.SendToAll(_dataWriter, DeliveryMethod.Sequenced);
        }

        public void SendTileDamage(Vector2 pos, float speed, int power)
        {
            if (!IsRunning) return;
            if (!IsHost && ServerPeer == null) return;

            var packet = new PacketTileDamage
            {
                X = pos.X,
                Y = pos.Y,
                Speed = speed,
                Power = power
            };

            _dataWriter.Reset();
            _packetProcessor.Write(_dataWriter, packet);

            if (IsHost)
                _netManager.SendToAll(_dataWriter, DeliveryMethod.Sequenced); // Sequenced is fine for continuous damage
            else if (ServerPeer != null)
                ServerPeer.Send(_dataWriter, DeliveryMethod.Sequenced);
        }
        
        public void SendItemDrop(int networkId, int itemId, int amount, Vector2 pos)
        {
            if (!IsRunning || !IsHost) return;

            var packet = new PacketItemDrop
            {
                NetworkId = networkId,
                ItemId = itemId,
                Amount = amount,
                X = pos.X,
                Y = pos.Y
            };
            
            //FileLogger.Log($"[NET-OUT] Item Drop: ID {networkId} (Item: {itemId}) at {pos}");

            _dataWriter.Reset();
            _packetProcessor.Write(_dataWriter, packet);
            _netManager.SendToAll(_dataWriter, DeliveryMethod.ReliableOrdered);
        }
        
        public void SendItemPickup(int networkId)
        {
            if (!IsRunning) return;
            
            var packet = new PacketItemPickup
            {
                NetworkId = networkId,
                PlayerId = IsHost ? -1 : ServerPeer.Id
            };

            //FileLogger.Log($"[NET-OUT-PICKUP] Sending Pickup Packet for NetID: {networkId}. AmHost: {IsHost}");

            _dataWriter.Reset();
            _packetProcessor.Write(_dataWriter, packet);

            if (IsHost)
            {
                // Host picked it up, tell everyone
                _netManager.SendToAll(_dataWriter, DeliveryMethod.ReliableOrdered);
            }
            else if (ServerPeer != null)
            {
                // Client picked it up, tell Host
                ServerPeer.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
            }
        }
        
        private void SendWorldData(NetPeer client)
        {
            if (!IsHost) return;

            try
            {
                //FileLogger.Log("Serializing world tiles...");
                var tileData = new System.Text.StringBuilder();
                
                foreach (var tile in World.Tiles)
                {
                    tileData.Append($"{tile.Position.X},{tile.Position.Y},{(int)tile.TileType};");
                }

                var packet = new PacketWorldSync
                {
                    TileData = tileData.ToString()
                };

                //FileLogger.Log($"Sending {World.Tiles.Count} tiles to client...");
                _dataWriter.Reset();
                _packetProcessor.Write(_dataWriter, packet);
                client.Send(_dataWriter, DeliveryMethod.ReliableOrdered);
                
                //FileLogger.Log("World data sent successfully");
            }
            catch (Exception ex)
            {
                //FileLogger.Log($"Error sending world data: {ex.Message}\n{ex.StackTrace}");
            }
        }

        // --- RECEIVING LOGIC ---

        private void OnPlayerPositionReceived(PacketPlayerPosition packet, NetPeer peer)
        {
            try
            {
                if (_logThrottleCounter % 60 == 0)
                {
                    //FileLogger.Log($"[NET-IN] Packet from Peer {peer.Id} (Content ID: {packet.PlayerId}) | Pos: {packet.X:F1},{packet.Y:F1}");
                }
                
                if (IsHost)
                {
                    if (packet.PlayerId != -1)
                        packet.PlayerId = peer.Id;
                    
                    _dataWriter.Reset();
                    _packetProcessor.Write(_dataWriter, packet);
                    _netManager.SendToAll(_dataWriter, DeliveryMethod.Sequenced, peer);
                }

                bool isOurOwnPacket = false;
                if (IsHost) isOurOwnPacket = (packet.PlayerId == -1);
                else isOurOwnPacket = (packet.PlayerId == ServerPeer?.Id);
                
                if (isOurOwnPacket) return;

                if (!RemotePlayers.ContainsKey(packet.PlayerId))
                {
                    //FileLogger.Log($"[SPAWN] New ID detected: {packet.PlayerId}. Spawning remote player...");
                    Console.WriteLine($"New Player Spawned: {packet.PlayerId}");
                    SpawnRemotePlayer(packet);
                }

                if (RemotePlayers.ContainsKey(packet.PlayerId))
                {
                    var p = RemotePlayers[packet.PlayerId];
                    if (p != null)
                        p.SetRemotePosition(new Vector2(packet.X, packet.Y), packet.Frame, packet.Flip);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnPlayerPositionReceived: {ex.Message}");
                //FileLogger.Log($"Error in OnPlayerPositionReceived: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void OnBlockChangeReceived(PacketBlockChange packet, NetPeer peer)
        {
            //FileLogger.Log($"[NET-IN] Block Change from {peer.Id}: {packet.TileType} at {packet.X},{packet.Y} Place: {packet.IsPlacement}");
            
            // 1. Apply to Local World
            var pos = new Vector2(packet.X, packet.Y);
            if (packet.IsPlacement)
            {
                var tile = TilesBank.FindTile(packet.TileType, pos);
                World.AddTile(tile); // Will fail gracefully if tile exists
            }
            else
            {
                var tile = World.SearchTile(pos);
                if (tile != null)
                {
                    World.RemoveTile(tile);
                }
            }

            // 2. If Host, Rebroadcast to others (excluding sender)
            if (IsHost)
            {
                _dataWriter.Reset();
                _packetProcessor.Write(_dataWriter, packet);
                _netManager.SendToAll(_dataWriter, DeliveryMethod.ReliableOrdered, peer);
            }
        }

        private void OnItemPickupReceived(PacketItemPickup packet, NetPeer peer)
        {
            //FileLogger.Log($"[NET-IN-PICKUP] Received Pickup Packet for NetID: {packet.NetworkId} from Peer: {packet.PlayerId}");

            // 1. Remove from Local World
            var item = World.GetDroppedItem(packet.NetworkId);
            if (item != null)
            {
                //FileLogger.Log($"[NET-IN-PICKUP] Item found locally (NetID: {item.NetworkId}). Removing...");
                World.RemoveItem(item);
            }
            else
            {
                //FileLogger.Log($"[NET-IN-PICKUP] WARNING: Item NetID {packet.NetworkId} NOT found locally!");
            }

            // 2. If Host, Rebroadcast to others
            if (IsHost)
            {
                //FileLogger.Log($"[NET-IN-PICKUP] Host Rebroadcasting Pickup for NetID: {packet.NetworkId}");
                
                _dataWriter.Reset();
                _packetProcessor.Write(_dataWriter, packet);
                // Send to all EXCEPT the original sender (peer) because they already removed it locally
                _netManager.SendToAll(_dataWriter, DeliveryMethod.ReliableOrdered, peer);
            }
        }
        
        private void OnWorldSyncReceived(PacketWorldSync packet, NetPeer peer)
        {
            try
            {
                //FileLogger.Log("Received world sync packet");
                if (IsHost) return;

                //FileLogger.Log("Clearing client world...");
                World.Tiles.Clear();
                
                //FileLogger.Log("Parsing tile data...");
                if (!string.IsNullOrEmpty(packet.TileData))
                {
                    var tileEntries = packet.TileData.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    FileLogger.Log($"Loading {tileEntries.Length} tiles...");
                    
                    foreach (var entry in tileEntries)
                    {
                        var parts = entry.Split(',');
                        if (parts.Length == 3)
                        {
                            float x = float.Parse(parts[0]);
                            float y = float.Parse(parts[1]);
                            Tiles tileType = (Tiles)int.Parse(parts[2]);
                            
                            var tile = TilesBank.FindTile(tileType, new Vector2(x, y));
                            World.AddTile(tile);
                        }
                    }
                }

                //FileLogger.Log($"World sync complete - loaded {World.Tiles.Count} tiles");
            }
            catch (Exception ex)
            {
                //FileLogger.Log($"Error in OnWorldSyncReceived: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void OnTileDamageReceived(PacketTileDamage packet, NetPeer peer)
        {
            // Apply damage locally
            var pos = new Vector2(packet.X, packet.Y);
            var tile = World.SearchTile(pos);
            
            if (tile != null)
            {
                tile.TakeDamage(packet.Speed, packet.Power);
            }

            // If Host, Rebroadcast to others (excluding sender)
            if (IsHost)
            {
                _dataWriter.Reset();
                _packetProcessor.Write(_dataWriter, packet);
                _netManager.SendToAll(_dataWriter, DeliveryMethod.Sequenced, peer);
            }
        }
        
        private void OnMobPositionReceived(PacketMobPosition packet, NetPeer peer)
        {
            if (IsHost) return;

            // Debug log every update to confirm receipt (can comment out later)
            // FileLogger.Log($"[MOB-IN] Update for Mob {packet.MobId}");

            var mob = World.GetMob(packet.MobId);
            if (mob != null)
            {
                mob.SetRemotePosition(new Vector2(packet.X, packet.Y), packet.Frame, packet.Flip);
            }
            else
            {
                // This will fire if World.Reset happened but AddMob didn't happen, or IDs are mismatched
                FileLogger.Log($"[NET-ERR] Received mob packet for ID {packet.MobId}, but mob not found locally!");
            }
        }
        
        private void OnItemDropReceived(PacketItemDrop packet, NetPeer peer)
        {
            if (IsHost) return; 
            
            FileLogger.Log($"[NET-IN-DROP] Received Drop Packet. NetID: {packet.NetworkId}, ItemID: {packet.ItemId} at {packet.X},{packet.Y}");

            var item = ItemsBank.GetItem(packet.ItemId);
            if (item != null)
            {
                var droppedItem = new DroppedItem(item, new Vector2(packet.X, packet.Y));
                droppedItem.NetworkId = packet.NetworkId; // SYNC ID
                World.AddItem(droppedItem);
                
                FileLogger.Log($"[NET-IN-DROP] Added item to world. Local WorldID: {droppedItem.ItemWorldId}, Assigned NetID: {droppedItem.NetworkId}");
            }
            else
            {
                FileLogger.Log($"[NET-ERR] Unknown ItemID in drop packet: {packet.ItemId}");
            }
        }

        private void SpawnRemotePlayer(PacketPlayerPosition packet)
        {
            try
            {
                RemotePlayers.Add(packet.PlayerId, null);
                
                if (_cachedPlayerTexture == null || _cachedSlotTexture == null)
                {
                    FileLogger.Log("Loading textures from Content...");
                    try
                    {
                        _cachedPlayerTexture = Game1.Instance.Content.Load<Texture2D>("Sprites/KomaruAtlas");
                        _cachedSlotTexture = Game1.Instance.Content.Load<Texture2D>("Sprites/UI/SlotAtlas");
                        FileLogger.Log("Textures loaded successfully");
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Log($"Failed to load textures: {ex.Message}. Removing from dictionary.");
                        RemotePlayers.Remove(packet.PlayerId);
                        return;
                    }
                }

                FileLogger.Log("Creating atlases from cached textures...");
                var playerAtlas = new Atlas(_cachedPlayerTexture, GameParameters.EntitySize / GameParameters.SIZE_MOD);
                var slotAtlas = new Atlas(_cachedSlotTexture, GameParameters.SlotSize / GameParameters.SIZE_MOD);

                FileLogger.Log("Creating player object...");
                var newPlayer = new Player(playerAtlas, new Vector2(packet.X, packet.Y), GameParameters.EntitySize, 1, slotAtlas);

                FileLogger.Log("Setting player as remote...");
                newPlayer.IsRemote = true;
                newPlayer.SetRemotePosition(new Vector2(packet.X, packet.Y), packet.Frame, packet.Flip);

                FileLogger.Log("Adding player to world...");
                World.AddPlayer(newPlayer);
                
                FileLogger.Log("Updating RemotePlayers dictionary...");
                RemotePlayers[packet.PlayerId] = newPlayer; 
                
                FileLogger.Log($"Successfully spawned remote player {packet.PlayerId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error spawning remote player: {ex.Message}");
                FileLogger.Log($"ERROR spawning remote player: {ex.Message}\n{ex.StackTrace}");
                if (RemotePlayers.ContainsKey(packet.PlayerId)) RemotePlayers.Remove(packet.PlayerId);
            }
        }
    }
}