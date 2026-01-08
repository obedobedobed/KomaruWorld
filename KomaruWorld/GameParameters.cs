using Microsoft.Xna.Framework;

namespace KomaruWorld;

public static class GameParameters
{
    public static readonly Vector2 PlayerSize = new Vector2(16, 16);
    public static readonly Vector2 TileSize = new Vector2(8, 8);
    public static readonly Point GlyphSize = new Point(8, 8);
    public const float FRAME_TIME = 0.4f;
}