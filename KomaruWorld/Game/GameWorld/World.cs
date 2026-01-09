using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public static class World
{
    public static List<Tile> Tiles { get; private set; } = new List<Tile>();

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

    public static void Update(GameTime gameTime)
    {
        foreach (var tile in Tiles)
            tile.Update(gameTime);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var tile in Tiles)
            tile.Draw(spriteBatch);
    }
}