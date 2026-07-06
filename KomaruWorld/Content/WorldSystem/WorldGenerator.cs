using System.Collections.Generic;
using KomaruWorld.Content.WorldSystem.IDs;
using Microsoft.Xna.Framework;

namespace KomaruWorld.Content.WorldSystem;

public static class WorldGenerator
{
    public static List<TileInstance> Generate(Point size)
    {
        var tiles = new List<TileInstance>();

        float yPos = 0;
        for (int y = 0; y < size.Y; y++)
        {
            float xPos = 0;
            for (int x = 0; x < size.X; x++)
            {
                int id = -1;

                if (y == 35)
                    id = TilesIDs.GrassTile;
                else if (y > 35 && y <= 40)
                    id = TilesIDs.DirtTile;
                else if (y > 40)
                    id = TilesIDs.StoneTile;

                if (id >= 0)
                    tiles.Add(new TileInstance(id, new Vector2(xPos, yPos)));

                xPos += 8 * World.SIZE_MOD;
            }

            yPos += 8 * World.SIZE_MOD;
        }

        return tiles;
    }
}