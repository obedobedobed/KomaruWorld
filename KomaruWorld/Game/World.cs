using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public static class World
{
    public static List<Tile> Tiles { get; private set; } = new List<Tile>();

    public static void AddTile(Tile tile)
    {
        Tiles.Add(tile);
    }

    public static void RemoveTile(Tile tile)
    {
        Tiles.Remove(tile);
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