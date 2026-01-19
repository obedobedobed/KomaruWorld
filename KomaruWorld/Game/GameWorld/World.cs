using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public static class World
{
    // Counters for network IDs
    private static int _mobIdCounter = 0;
    private static int _dropIdCounter = 0; 
    
    public static List<Tile> Tiles { get; private set; } = new List<Tile>();
    public static List<DroppedItem> Items { get; private set; } = new List<DroppedItem>();
    public static List<Mob> Mobs { get; private set; } = new List<Mob>();
    public static List<Player> Players { get; private set; } = new List<Player>();
    public static bool AddTile(Tile tile, bool replace = false)
    {
        var tileOnSamePosition = SearchTile(tile.Position);

        if (tileOnSamePosition?.TileType == tile.TileType)
            return false;

        if (tileOnSamePosition != null && replace)
            RemoveTile(tileOnSamePosition);
        if (tileOnSamePosition != null && !replace)
            return false;

        Tiles.Add(tile);
        return true;
    }

    public static void RemoveTile(Tile tile)
    {
        Tiles.Remove(tile);
    }

    public static Tile SearchTile(Vector2 position)
    {
        foreach (var tile in Tiles)
            if (tile.Position == position)
                return tile;

        return null;
    }

    public static void AddItem(DroppedItem item)
    {
        Items.Add(item);
    }

    public static void RemoveItem(DroppedItem item)
    {
        Items.Remove(item);
    }

    public static void AddMob(Mob mob)
    {
        // Assign a deterministic Network ID based on spawn order
        mob.NetworkId = _mobIdCounter++;
        Mobs.Add(mob);
        //FileLogger.Log($"[WORLD] Added Mob '{mob.Name}' with NetworkID: {mob.NetworkId} at {mob.Position}");
    }
    
    public static void AddPlayer(Player player)
    {
        Players.Add(player);
    }

    public static void Update(GameTime gameTime)
    {
        Tile[] tiles = Tiles.ToArray();
        DroppedItem[] items = Items.ToArray();
        Mob[] mobs = Mobs.ToArray();
        Player[] players = Players.ToArray();
        
        foreach (var tile in tiles)
            tile?.Update(gameTime);

        foreach (var item in items)
            item?.Update(gameTime);

        foreach (var mob in mobs)
            mob?.Update(gameTime);
        
        foreach (var player in players)  // ADD THIS
            player?.Update(gameTime);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var tile in Tiles)
            tile.Draw(spriteBatch);

        foreach (var item in Items)
            item.Draw(spriteBatch);

        foreach (var mob in Mobs)
            mob.Draw(spriteBatch);
        
        foreach (var player in Players)
            player.Draw(spriteBatch);
    }
    
    // --- Helper to find mob by ID ---
    public static Mob GetMob(int id)
    {
        // For small counts, linear search is fine. 
        // For many mobs, consider a Dictionary<int, Mob>
        foreach (var mob in Mobs)
        {
            if (mob.NetworkId == id) return mob;
        }
        return null;
    }
    
    // --- Helper to find dropped item by ID ---
    public static DroppedItem GetDroppedItem(int id)
    {
        foreach (var item in Items)
        {
            if (item.NetworkId == id) return item;
        }
        
        return null;
    }
    
    // --- Generate next drop ID ---
    public static int GetNextDropId()
    {
        return _dropIdCounter++;
    }
    
    // --- Reset method to align IDs on game start ---
    public static void Reset()
    {
        Tiles.Clear();
        Items.Clear();
        Mobs.Clear();
        Players.Clear();
        _mobIdCounter = 0;
        //FileLogger.Log("[WORLD] World reset. Mob ID counter set to 0.");
    }
}