using System;
using Microsoft.Xna.Framework;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class WorldGenerator
{
    private static string[] tree =
    {
        " @@@ ",
        "@@@@@",
        "@@#@@",
        "  #  ",
        "  #  "
    };
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
                    targetTile = TilesBank.Stone(targetPosition);
                else if (y > 15)
                    targetTile = TilesBank.Dirt(targetPosition);
                else if (y == 15)
                    targetTile = TilesBank.Grass(targetPosition);
                else
                    targetTile = null;

                if (y == 15 - tree.Length && Random.Shared.Next(0, 101) <= treeSpawnChance && canGenerateTrees)
                {
                    GenerateTree(xPos, yPos);
                    generatedTreeTilesAgo = 0;
                    canGenerateTrees = false;
                }

                if (generatedTreeTilesAgo > tree[0].Length)
                    canGenerateTrees = true;

                generatedTreeTilesAgo++;

                if (targetTile != null)
                    World.AddTile(targetTile);

                xPos += TileSize.X;
            }

            yPos += TileSize.Y;
        }
    }

    public static void GenerateTree(float baseX, float baseY)
    {
        float yPos = baseY;
        for (int y = 0; y < tree.Length; y++)
        {
            float xPos = baseX;
            for (int x = 0; x < tree[0].Length; x++)
            {
                Vector2 targetPosition = new Vector2(xPos, yPos);
                Tile targetTile = tree[y][x] switch
                {
                    '@' => TilesBank.Leaf(targetPosition),
                    '#' => TilesBank.Log(targetPosition),
                    _ => null,
                };

                if (targetTile != null)
                    World.AddTile(targetTile);

                xPos += TileSize.X;
            }

            yPos += TileSize.Y;
        }
    }
}