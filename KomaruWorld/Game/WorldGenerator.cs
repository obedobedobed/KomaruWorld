using Microsoft.Xna.Framework;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class WorldGenerator
{
    public static void Generate(int width, int height)
    {
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

                if (targetTile != null)
                    World.AddTile(targetTile);

                xPos += TileSize.X * 4;
            }

            yPos += TileSize.Y * 4;
        }
    }
}