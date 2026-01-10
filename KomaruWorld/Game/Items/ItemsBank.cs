using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public static class ItemsBank
{
    // Textures
    public static Texture2D GrassTexture { get; private set; }
    public static Texture2D DirtTexture { get; private set; }
    public static Texture2D StoneTexture { get; private set; }
    public static Texture2D LogTexture { get; private set; }
    public static Texture2D LeavesTexture { get; private set; }
    public static Texture2D PlanksTexture { get; private set; }
    public static Texture2D StickTexture { get; private set; }
    public static Texture2D LeafTexture { get; private set; }

    public static void LoadTextures(ContentManager Content)
    {
        GrassTexture = Content.Load<Texture2D>("Sprites/Tiles/GrassTile");
        DirtTexture = Content.Load<Texture2D>("Sprites/Tiles/DirtTile");
        StoneTexture = Content.Load<Texture2D>("Sprites/Tiles/StoneTile");
        LogTexture = Content.Load<Texture2D>("Sprites/Tiles/LogTile");
        LeavesTexture = Content.Load<Texture2D>("Sprites/Tiles/LeavesTile");
        PlanksTexture = Content.Load<Texture2D>("Sprites/Tiles/PlanksTile");
        StickTexture = Content.Load<Texture2D>("Sprites/Items/Stick");
        LeafTexture = Content.Load<Texture2D>("Sprites/Items/Leaf");
    }

    // Items
    public static Item Grass { get { return new PlaceableItem("Grass", 99, GrassTexture, 0, Tiles.Grass); } }
    public static Item Dirt { get { return new PlaceableItem("Dirt", 99, DirtTexture, 1, Tiles.Dirt); } }
    public static Item Stone { get { return new PlaceableItem("Stone", 99, StoneTexture, 2, Tiles.Stone); } }
    public static Item Log { get { return new PlaceableItem("Log", 99, LogTexture, 3, Tiles.Log); } }
    public static Item Leaves { get { return new PlaceableItem("Leaf", 99, LeavesTexture, 4, Tiles.Leaves); } }
    public static Item Planks { get { return new PlaceableItem("Planks", 99, PlanksTexture, 5, Tiles.Planks); } }
    public static Item Stick { get { return new Item("Stick", 99, StickTexture, 6); } }
    public static Item Leaf { get { return new Item("Leaf", 99, LeafTexture, 7); } }
}