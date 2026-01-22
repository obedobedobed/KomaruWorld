using System.Collections.Generic;
using Microsoft.Xna.Framework;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class WorldStructure
{
    public string[] Layout { get; private set; }
    private Dictionary<char, Tiles> charToTile;
    private Point center;

    public WorldStructure(string[] layout, Dictionary<char, Tiles> charToTile, Point center)
    {
        Layout = layout;
        this.charToTile = charToTile;
        this.center = center;
    }

    public void Generate(Vector2 position)
    {
        position.Y -= TileSize.Y * center.Y;
        position.X -= TileSize.X * center.X;

        float yAdder = 0;
        for (int y = 0; y < Layout.Length; y++)
        {
            float xAdder = 0;
            for (int x = 0; x < Layout[0].Length; x++)
            {
                var tileToAdd = charToTile.GetValueOrDefault(Layout[y][x], Tiles.None);

                if (tileToAdd != Tiles.None)
                    World.AddTile(TilesBank.FindTile(tileToAdd, new Vector2(position.X + xAdder, position.Y + yAdder)));

                xAdder += TileSize.X;
            }

            yAdder += TileSize.Y;
        }
    }
}