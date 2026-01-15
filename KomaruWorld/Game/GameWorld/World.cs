using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public static class World
{
    public static List<Tile> Tiles { get; private set; } = new List<Tile>();
    public static List<DroppedItem> Items { get; private set; } = new List<DroppedItem>();
    public static List<Mob> Mobs { get; private set; } = new List<Mob>();

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
        Mobs.Add(mob);
    }

    public static void Update(GameTime gameTime)
    {
        Tile[] tiles = Tiles.ToArray();
        DroppedItem[] items = Items.ToArray();
        Mob[] mobs = Mobs.ToArray();

        foreach (var tile in tiles)
            tile?.Update(gameTime);

        foreach (var item in items)
            item?.Update(gameTime);

        foreach (var mob in mobs)
            mob?.Update(gameTime);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var tile in Tiles)
            tile.Draw(spriteBatch);

        foreach (var item in Items)
            item.Draw(spriteBatch);

        foreach (var mob in Mobs)
            mob.Draw(spriteBatch);
    }
}