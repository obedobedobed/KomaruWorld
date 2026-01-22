using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class Atlas
{
    public Texture2D Texture { get; private set; }
    public List<Rectangle> Rectangles { get; private set; } = new List<Rectangle>();

    public Atlas(Texture2D texture, Vector2 spriteSize)
    {
        Texture = texture;

        int yPos = 0;
        for (int y = 0; y < Texture.Height / spriteSize.Y; y++)
        {
            int xPos = 0;
            for (int x = 0; x < Texture.Width / spriteSize.X; x++)
            {
                Rectangles.Add(new Rectangle(xPos, yPos, (int)spriteSize.X, (int)spriteSize.Y));

                xPos += (int)spriteSize.X;
            }

            yPos += (int)spriteSize.Y;
        }
    }
}