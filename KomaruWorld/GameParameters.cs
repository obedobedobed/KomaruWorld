using Microsoft.Xna.Framework;

namespace KomaruWorld;

public static class GameParameters
{
    public const string GAME_NAME = "Komaru World";
    public const string GAME_VERSION = "0.0.2";
    public const int SIZE_MOD = 4;
    public const int TEXT_MOD = 2;
    public const int TEXT_SPACING = 1 * TEXT_MOD;
    public const int UI_SPACING = 1 * SIZE_MOD;
    public static readonly Vector2 PlayerSize = new Vector2(16, 16) * SIZE_MOD;
    public static readonly Vector2 SlotSize = new Vector2(16, 16) * SIZE_MOD;
    public static readonly Vector2 DroppedItemSize = new Vector2(6, 6) * SIZE_MOD;
    public static readonly Vector2 ItemSize = new Vector2(10, 10) * SIZE_MOD;
    public static readonly Vector2 TileSize = new Vector2(8, 8) * SIZE_MOD;
    public static readonly Point GlyphSize = new Point(8 * TEXT_MOD, 8 * TEXT_MOD);
    public const float FRAME_TIME = 0.4f;
}