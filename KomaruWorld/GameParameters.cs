using Microsoft.Xna.Framework;

namespace KomaruWorld;

public static class GameParameters
{
    public const string GAME_NAME = "Komaru World";
    public const string GAME_VERSION = "0.0.4 (Beta)";
    public const int SIZE_MOD = 4;
    public const int TEXT_MOD = 2;
    public const int TEXT_SPACING = 1 * TEXT_MOD;
    public const int UI_SPACING = 1 * SIZE_MOD;
    public const int INV_SLOTS_IN_LINE = 5;
    public const int INV_SLOTS_LINES = 3;
    public const int OUT_PIXELS = 1 * TEXT_MOD;
    public static readonly Vector2 EntitySize = new Vector2(16, 16) * SIZE_MOD;
    public static readonly Vector2 SlotSize = new Vector2(16, 16) * SIZE_MOD;
    public static readonly Vector2 DroppedItemSize = new Vector2(6, 6) * SIZE_MOD;
    public static readonly Vector2 ItemSize = new Vector2(10, 10) * SIZE_MOD;
    public static readonly Vector2 TileSize = new Vector2(8, 8) * SIZE_MOD;
    public static readonly Vector2 CraftMenuSize = new Vector2(72, 64) * SIZE_MOD;
    public static readonly Vector2 CraftMenuItemSlotPos = new Vector2(28, 4) * SIZE_MOD;
    public static readonly Point GlyphSize = new Point(8 * TEXT_MOD, 8 * TEXT_MOD);
    public static readonly Point CursorSize = new Point(6 * SIZE_MOD, 6 * SIZE_MOD);
    public static Vector2 InventorySlotsPos { get; private set; }
    public const float FRAME_TIME = 0.4f;

    public static void Setup()
    {
        InventorySlotsPos = new Vector2
        (
            x: (Game1.Instance.Graphics.PreferredBackBufferWidth - SlotSize.X * INV_SLOTS_IN_LINE -
            UI_SPACING * (INV_SLOTS_IN_LINE - 1)) / 2,
            y: Game1.Instance.Graphics.PreferredBackBufferHeight / 2 -
            SlotSize.Y * INV_SLOTS_LINES / 2 - SlotSize.Y * 1
        );
    }
}