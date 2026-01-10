using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public static class World
{
    public static List<Tile> Tiles { get; private set; } = new List<Tile>();
    public static List<DroppedItem> Items { get; private set; } = new List<DroppedItem>();

    public static bool AddTile(Tile tile)
    {
        var tileOnSamePosition = SearchTile(tile.Position);

        if (tileOnSamePosition?.TileType == tile.TileType)
            return false;

        if (tileOnSamePosition != null)
            RemoveTile(tileOnSamePosition);

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

    public static void Update(GameTime gameTime)
    {
        foreach (var tile in Tiles)
            tile.Update(gameTime);

        foreach (var item in Items)
            item.Update(gameTime);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var tile in Tiles)
            tile.Draw(spriteBatch);

        foreach (var item in Items)
            item.Draw(spriteBatch);
    }
}