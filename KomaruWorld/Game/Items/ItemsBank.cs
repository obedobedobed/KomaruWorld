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
    public static Texture2D LeafTexture { get; private set; }
    public static Texture2D PlanksTexture { get; private set; }
    public static Texture2D StickTexture { get; private set; }

    public static void LoadTextures(ContentManager Content)
    {
        GrassTexture = Content.Load<Texture2D>("Sprites/Tiles/GrassTile");
        DirtTexture = Content.Load<Texture2D>("Sprites/Tiles/DirtTile");
        StoneTexture = Content.Load<Texture2D>("Sprites/Tiles/StoneTile");
        LogTexture = Content.Load<Texture2D>("Sprites/Tiles/LogTile");
        LeafTexture = Content.Load<Texture2D>("Sprites/Tiles/LeafTile");
        PlanksTexture = Content.Load<Texture2D>("Sprites/Tiles/PlanksTile");
        StickTexture = Content.Load<Texture2D>("Sprites/Items/Stick");
    }

    // Items
    public static Item Grass { get { return new PlaceableItem("Grass", 99, GrassTexture, Tiles.Grass); } }
    public static Item Dirt { get { return new PlaceableItem("Dirt", 99, DirtTexture, Tiles.Dirt); } }
    public static Item Stone { get { return new PlaceableItem("Stone", 99, StoneTexture, Tiles.Stone); } }
    public static Item Log { get { return new PlaceableItem("Log", 99, LogTexture, Tiles.Log); } }
    public static Item Leaf { get { return new PlaceableItem("Leaf", 99, LeafTexture, Tiles.Leaf); } }
    public static Item Planks { get { return new PlaceableItem("Planks", 99, PlanksTexture, Tiles.Planks); } }
    public static Item Stick { get { return new Item("Stick", 99, StickTexture); } }
}