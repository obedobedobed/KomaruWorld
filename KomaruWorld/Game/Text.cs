using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        { ']', 68 },
        { '{', 69 },
        { '}', 69 },
        { '/', 70 },
        { '\\', 71 },
        { '*', 72 },
        { '`', 73 },
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

    public static void Setup(Atlas glyphes, Point glyphSize)
    {
        Text.glyphes = glyphes;
        Text.glyphSize = glyphSize;
    }

    public static void Write(string text, Vector2 position, Color color, SpriteBatch spriteBatch)
    {
        int glyphPosition = (int)position.X;

        foreach (char _char in text)
        {
            bool canFindGlyph = charToGlyphId.TryGetValue(_char, out int glyphId);
            if (!canFindGlyph)
                throw new System.Exception($"Cannot find glyph {_char} in dictionary!");
            else
                spriteBatch.Draw(glyphes.Texture, new Rectangle
                (glyphPosition, (int)position.Y, glyphSize.X, glyphSize.Y),
                glyphes.Rectangles[glyphId], color);

            glyphPosition += glyphSize.X;
        }
    }
}