using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class Text
{
    private static Atlas glyphes;
    private static Point glyphSize;
    private static Dictionary<char, int> charToGlyphId = new Dictionary<char, int>()
    {
        { ' ', 0 },
        { 'a', 1 },
        { 'b', 2 },
        { 'c', 3 },
        { 'd', 4 },
        { 'e', 5 },
        { 'f', 6 },
        { 'g', 7 },
        { 'h', 8 },
        { 'i', 9 },
        { 'j', 10 },
        { 'k', 11 },
        { 'l', 12 },
        { 'm', 13 },
        { 'n', 14 },
        { 'o', 15 },
        { 'p', 16 },
        { 'q', 17 },
        { 'r', 18 },
        { 's', 19 },
        { 't', 20 },
        { 'u', 21 },
        { 'v', 22 },
        { 'w', 23 },
        { 'x', 24 },
        { 'y', 25 },
        { 'z', 26 },
        { 'A', 27 },
        { 'B', 28 },
        { 'C', 29 },
        { 'D', 30 },
        { 'E', 31 },
        { 'F', 32 },
        { 'G', 33 },
        { 'H', 34 },
        { 'I', 35 },
        { 'J', 36 },
        { 'K', 37 },
        { 'L', 38 },
        { 'M', 39 },
        { 'N', 40 },
        { 'O', 41 },
        { 'P', 42 },
        { 'Q', 43 },
        { 'R', 44 },
        { 'S', 45 },
        { 'T', 46 },
        { 'U', 47 },
        { 'V', 48 },
        { 'W', 49 },
        { 'X', 50 },
        { 'Y', 51 },
        { 'Z', 52 },
        { '.', 53 },
        { '!', 54 },
        { '?', 55 },
        { ':', 56 },
        { ',', 57 },
        { ';', 58 },
        { '_', 59 },
        { '-', 60 },
        { '+', 61 },
        { '=', 62 },
        { '(', 63 },
        { ')', 64 },
        { '[', 65 },
        { ']', 66 },
        { '{', 67 },
        { '}', 68 },
        { '/', 69 },
        { '\\', 70 },
        { '*', 71 },
        { '`', 72 },
        { '"', 73 },
        { '\'', 74 },
        { '&', 75 },
        { '^', 76 },
        { '%', 77 },
        { '$', 78 },
        { '#', 79 },
        { '@', 80 },
        { '~', 81 },
        { '1', 82 },
        { '2', 83 },
        { '3', 84 },
        { '4', 85 },
        { '5', 86 },
        { '6', 87 },
        { '7', 88 },
        { '8', 89 },
        { '9', 90 },
        { '0', 91 },
    };

    private static Dictionary<char, int> customWidthGlyphes = new Dictionary<char, int>
    {
        { 'i', 3 },  
        { 'j', 6 },  
        { 'k', 7 },  
        { 'l', 5 },  
        { 't', 5 },  
        { 'x', 7 },  
        { 'y', 7 },  
        { 'I', 7 },  
        { 'T', 7 },  
        { 'X', 7 },  
        { 'Y', 7 },  
        { '.', 3 },  
        { '!', 3 },  
        { '?', 6 },  
        { ':', 3 },  
        { ',', 4 },  
        { ';', 4 },  
        { '-', 7 },  
        { '+', 7 },  
        { '=', 7 },  
        { '(', 5 },  
        { ')', 5 },  
        { '[', 5 },  
        { ']', 5 },  
        { '{', 5 },  
        { '}', 5 },  
        { '/', 5 },  
        { '\\', 5 },  
        { '*', 6 },  
        { '`', 3 },  
        { '"', 6 },  
        { '\'', 3 },  
        { '^', 7 },  
        { '%', 7 },  
        { '$', 7 },  
    };

    public static void Setup(Atlas glyphes, Point glyphSize)
    {
        Text.glyphes = glyphes;
        Text.glyphSize = glyphSize;
    }

    public static void Draw(string text, Vector2 position, Color color, SpriteBatch spriteBatch, TextDrawingMode drawingMode)
    {
        int glyphPosition = drawingMode switch
        {
            TextDrawingMode.Right => (int)position.X,
            TextDrawingMode.Left => (int)position.X - CalculateStringWidth(text),
            TextDrawingMode.Center => (int)position.X - CalculateStringWidth(text) / 2,
            _ => 0
        };

        foreach (char _char in text)
        {
            int glyphWidth;
            if (!customWidthGlyphes.TryGetValue(_char, out glyphWidth))
                glyphWidth = glyphSize.X;
            else
                glyphWidth *= TEXT_MOD;

            bool canFindGlyph = charToGlyphId.TryGetValue(_char, out int glyphId);
            if (!canFindGlyph)
                throw new System.Exception($"Cannot find glyph {_char} in dictionary!");
            else
            {
                spriteBatch.Draw(glyphes.Texture, new Rectangle
                (glyphPosition, (int)position.Y, glyphSize.X, glyphSize.Y),
                glyphes.Rectangles[glyphId], color);
            }

            glyphPosition += glyphWidth;
        }
    }

    private static int CalculateStringWidth(string _string)
    {
        int width = 0;

        foreach (char _char in _string)
        {
            int glyphWidth;

            if (!customWidthGlyphes.TryGetValue(_char, out glyphWidth))
                glyphWidth = glyphSize.X;
            else
                glyphWidth *= TEXT_MOD;

            width += glyphWidth;
        }

        return width;
    }
}