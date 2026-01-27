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
        new Dictionary<char, Tiles>() { {'@', Tiles.NatureLeaves}, {'#', Tiles.NatureLog} },
        new Point(3, 5)
    );
    private const int TREE_SPAWN_CHANCE = 30;
    private const int ORE_SPAWN_CHANCE = 10;

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
                Tile targetTile = null;
                Tile targetWall = null;
                Vector2 targetPosition = new Vector2(xPos, yPos);

                if (y > 18)
                {
                    if (Random.Shared.Next(0, 100) <= ORE_SPAWN_CHANCE)
                        targetTile = TilesBank.FindTile((Tiles)Random.Shared.Next(7, 11), targetPosition);
                    else  
                        targetTile = TilesBank.Stone(targetPosition);

                    targetWall = TilesBank.CavesWall(targetPosition);
                }
                else if (y > 15)
                {
                    targetTile = TilesBank.Dirt(targetPosition);
                    targetWall = TilesBank.DirtWall(targetPosition);
                }
                else if (y == 15)
                {
                    targetTile = TilesBank.Grass(targetPosition);
                    targetWall = TilesBank.DirtWall(targetPosition);
                }

                if (y == 15 && Random.Shared.Next(0, 101) <= TREE_SPAWN_CHANCE && canGenerateTrees)
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

                if (targetWall != null)
                    World.AddWall(targetWall);

                // Creating border
                if (x == 0)
                    World.AddTile(TilesBank.Border(new Vector2(xPos - TileSize.X, yPos)));
                else if (x == width - 1)
                    World.AddTile(TilesBank.Border(new Vector2(xPos + TileSize.X, yPos)));

                if (y == 0)
                    World.AddTile(TilesBank.Border(new Vector2(xPos, yPos - TileSize.Y)));
                else if (y == height - 1)
                    World.AddTile(TilesBank.Border(new Vector2(xPos, yPos + TileSize.Y)));

                xPos += TileSize.X;
            }

            yPos += TileSize.Y;
        }
    }
}