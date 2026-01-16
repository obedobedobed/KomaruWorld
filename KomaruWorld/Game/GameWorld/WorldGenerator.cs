using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class WorldGenerator
{
    private static WorldStructure tree = new WorldStructure
    (
        [
            " @@@ ",
            "@@@@@",
            "@@#@@",
            "  #  ",
            "  #  "
        ],
        new Dictionary<char, Tiles>() { {'@', Tiles.Leaves}, {'#', Tiles.Log} },
        new Point(3, 5)
    );
    private static int treeSpawnChance = 30;

    public static void Generate(int width, int height)
    {
        bool canGenerateTrees = true;
        int generatedTreeTilesAgo = 0;

        float yPos = 0;
        for (int y = 0; y < height; y++)
        {
            float xPos = 0;
            for (int x = 0; x < width; x++)
            {
                Tile targetTile;
                Vector2 targetPosition = new Vector2(xPos, yPos);

                if (y > 18)
                {
                    if (Random.Shared.Next(0, 100) <= 20)
                        targetTile = TilesBank.FindTile((Tiles)Random.Shared.Next(7, 11), targetPosition);
                    else  
                        targetTile = TilesBank.Stone(targetPosition);
                }
                else if (y > 15)
                    targetTile = TilesBank.Dirt(targetPosition);
                else if (y == 15)
                    targetTile = TilesBank.Grass(targetPosition);
                else
                    targetTile = null;

                if (y == 15 && Random.Shared.Next(0, 101) <= treeSpawnChance && canGenerateTrees)
                {
                    tree.Generate(new Vector2(xPos, yPos));
                    generatedTreeTilesAgo = 0;
                    canGenerateTrees = false;
                }

                if (generatedTreeTilesAgo > tree.Layout[0].Length)
                    canGenerateTrees = true;

                generatedTreeTilesAgo++;

                if (targetTile != null)
                    World.AddTile(targetTile);

                xPos += TileSize.X;
            }

            yPos += TileSize.Y;
        }
    }
}