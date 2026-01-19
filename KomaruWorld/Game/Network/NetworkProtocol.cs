using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace KomaruWorld
{
    public class JoinPacket
    {
        public string UserName { get; set; }
    }

    public class PacketPlayerPosition
    {
        public int PlayerId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Frame { get; set; }
        public bool Flip { get; set; }
    }

    public class PacketBlockChange
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Tiles TileType { get; set; }
        public bool IsPlacement { get; set; }
    }
    
    public class PacketWorldSync
    {
        public string TileData { get; set; }
    }
    
    public class PacketMobPosition
    {
        public int MobId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Frame { get; set; }
        public bool Flip { get; set; }
    }
    
    public class PacketTileDamage
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Speed { get; set; }
        public int Power { get; set; }
    }

    public class PacketItemDrop
    {
        public int NetworkId { get; set; }
        public int ItemId { get; set; }
        public int Amount { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
    
    public class PacketItemPickup
    {
        public int NetworkId { get; set; }
        public int PlayerId { get; set; }
    }
}