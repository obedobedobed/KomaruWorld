using Microsoft.Xna.Framework;

namespace KomaruWorld;

public static class GameParameters
{
    public const string GAME_NAME = "Komaru World";
    public const string GAME_VERSION = "0.1.0 (Beta)";
    public const int VIRTUAL_WIDTH = 800;
    public const int VIRTUAL_HEIGHT = 450;
    public const int SIZE_MOD = 4;
    public const int BG_MOD = 3;
    public const int TEXT_MOD = 2;
    public const int TEXT_SPACING = 1 * TEXT_MOD;
    public const int UI_SPACING = 1 * SIZE_MOD;
    public const int INV_SLOTS_IN_LINE = 5;
    public const int INV_SLOTS_LINES = 3;
    public const int HEARTS_IN_LINE = 8;
    public const int OUT_PIXELS = 1 * TEXT_MOD;
    public static readonly Vector2 EntitySize = new Vector2(16, 16) * SIZE_MOD;
    public static readonly Vector2 SlotSize = new Vector2(16, 16) * SIZE_MOD;
    public static readonly Vector2 DroppedItemSize = new Vector2(6, 6) * SIZE_MOD;
    public static readonly Vector2 ItemSize = new Vector2(10, 10) * SIZE_MOD;
    public static readonly Point ToolsHandlePos = new Point(2, 7);
    public static readonly Vector2 TileSize = new Vector2(8, 8) * SIZE_MOD;
    public static readonly Vector2 BGTileSize = new Vector2(8, 8) * BG_MOD;
    public static readonly Vector2 HeartSize = new Vector2(6, 6) * SIZE_MOD;
    public static readonly Vector2 CraftMenuSize = new Vector2(72, 64) * SIZE_MOD;
    public static readonly Vector2 CraftMenuItemSlotPos = new Vector2(28, 4) * SIZE_MOD;
    public static readonly Vector2 CloudSize = new Vector2(64, 32) * BG_MOD;
    public static readonly Point GlyphSize = new Point(8 * TEXT_MOD, 8 * TEXT_MOD);
    public static readonly Point CursorSize = new Point(6 * SIZE_MOD, 6 * SIZE_MOD);
    public static readonly Point PlayerActionRadius = new Point(4, 4);
    public static Vector2 InventorySlotsPos { get; private set; }
    public const float FRAME_TIME = 0.4f;
    public static readonly Point VerySmallWorldSize = new Point(80, 60);
    public static readonly Point SmallWorldSize = new Point(360, 180);
    public static readonly Point MediumWorldSize = new Point(540, 270);
    public static readonly Point BigWorldSize = new Point(720, 360);

    public static void Setup()
    {
        InventorySlotsPos = new Vector2
        (
            x: (VIRTUAL_WIDTH - SlotSize.X * INV_SLOTS_IN_LINE -
            UI_SPACING * (INV_SLOTS_IN_LINE - 1)) / 2,
            y: VIRTUAL_HEIGHT / 2 -
            SlotSize.Y * INV_SLOTS_LINES / 2 - SlotSize.Y * 1
        );
    }
}