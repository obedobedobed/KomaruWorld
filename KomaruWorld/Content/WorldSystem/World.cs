using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld.Content.WorldSystem;

public static class World
{
    public const int SIZE_MOD = 4;

    public static List<TileInstance> Tiles { get; private set; }
    public static List<TileInstance> TilesToRemove { get; private set; } = new List<TileInstance>();
    public static Point WorldSize { get; private set; }

    public static void LoadWorld(Point size, List<TileInstance> tiles)
    {
        WorldSize = size;
        Tiles = tiles;
    }

    public static void AddTile(int id, Vector2 position)
    {
        bool tileOnPos = false;

        foreach (var tile in Tiles)
        {
            if (tile.Position == position)
            {
                tileOnPos = true;
                break;
            }
        }

        if (!tileOnPos)
            Tiles.Add(new TileInstance(id, position));
    }

    public static void RemoveTile(Vector2 position)
    {
        TilesToRemove.Add(FindTile(position));
    }

    public static TileInstance FindTile(Vector2 position)
    {
        foreach (var tile in Tiles)
        {
            if (tile.Position == position)
            {
                return tile;
            }
        }

        return null;
    }

    public static void Update(GameTime gameTime)
    {
        foreach (var tile in TilesToRemove)
            Tiles.Remove(tile);

        TilesToRemove.Clear();
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var tile in Tiles)
            tile.DrawOutline(spriteBatch);

        foreach (var tile in Tiles)
            tile.DrawTile(spriteBatch);
    }
}