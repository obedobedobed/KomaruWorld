using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class ItemsBank
{
    public static Atlas DestroyingAtlas { get; private set; }

    // Textures
    public static Texture2D GrassTexture { get; private set; }
    public static Texture2D DirtTexture { get; private set; }
    public static Texture2D StoneTexture { get; private set; }
    public static Texture2D LogTexture { get; private set; }
    public static Texture2D LeavesTexture { get; private set; }
    public static Texture2D PlanksTexture { get; private set; }
    public static Texture2D StickTexture { get; private set; }
    public static Texture2D LeafTexture { get; private set; }
    public static Texture2D AxeTexture { get; private set; }
    public static Texture2D PickaxeTexture { get; private set; }
    public static Texture2D SwordTexture { get; private set; }
    public static Texture2D IronHelmetTexture { get; private set; }
    private static Atlas IronHelmetAtlas;
    public static Texture2D IronChestplateTexture { get; private set; }
    private static Atlas IronChestplateAtlas;
    public static Texture2D IronLegginsTexture { get; private set; }
    private static Atlas IronLegginsAtlas;
    public static Texture2D IronIngotTexture { get; private set; }
    public static Texture2D GoldIngotTexture { get; private set; }
    public static Texture2D EmeraldTexture { get; private set; }
    public static Texture2D AmethystTexture { get; private set; }
    public static Texture2D IronOreTexture { get; private set; }
    public static Texture2D GoldOreTexture { get; private set; }
    public static Texture2D EmeraldOreTexture { get; private set; }
    public static Texture2D AmethystOreTexture { get; private set; }

    public static void LoadContent(ContentManager Content)
    {
        DestroyingAtlas = new Atlas(Content.Load<Texture2D>("Sprites/Tiles/TileDestroyingAtlas"), TileSize / SIZE_MOD);

        GrassTexture = Content.Load<Texture2D>("Sprites/Tiles/GrassTile");
        DirtTexture = Content.Load<Texture2D>("Sprites/Tiles/DirtTile");
        StoneTexture = Content.Load<Texture2D>("Sprites/Tiles/StoneTile");
        LogTexture = Content.Load<Texture2D>("Sprites/Tiles/LogTile");
        LeavesTexture = Content.Load<Texture2D>("Sprites/Tiles/LeavesTile");
        PlanksTexture = Content.Load<Texture2D>("Sprites/Tiles/PlanksTile");
        StickTexture = Content.Load<Texture2D>("Sprites/Items/Stick");
        LeafTexture = Content.Load<Texture2D>("Sprites/Items/Leaf");
        AxeTexture = Content.Load<Texture2D>("Sprites/Items/Tools/Axe");
        PickaxeTexture = Content.Load<Texture2D>("Sprites/Items/Tools/Pickaxe");
        SwordTexture = Content.Load<Texture2D>("Sprites/Items/Tools/Sword");
        IronHelmetTexture = Content.Load<Texture2D>("Sprites/Items/Armor/IronHelmetItem");
        IronHelmetAtlas = new Atlas(Content.Load<Texture2D>("Sprites/Items/Armor/IronHelmet"), EntitySize / SIZE_MOD);
        IronChestplateTexture = Content.Load<Texture2D>("Sprites/Items/Armor/IronChestplateItem");
        IronChestplateAtlas = new Atlas(Content.Load<Texture2D>("Sprites/Items/Armor/IronChestplate"), EntitySize / SIZE_MOD);
        IronLegginsTexture = Content.Load<Texture2D>("Sprites/Items/Armor/IronLegginsItem");
        IronLegginsAtlas = new Atlas(Content.Load<Texture2D>("Sprites/Items/Armor/IronLeggins"), EntitySize / SIZE_MOD);
        IronIngotTexture = Content.Load<Texture2D>("Sprites/Items/IronIngot");
        GoldIngotTexture = Content.Load<Texture2D>("Sprites/Items/GoldIngot");
        EmeraldTexture = Content.Load<Texture2D>("Sprites/Items/Emerald");
        AmethystTexture = Content.Load<Texture2D>("Sprites/Items/Amethyst");
        IronOreTexture = Content.Load<Texture2D>("Sprites/Tiles/IronOre");
        GoldOreTexture = Content.Load<Texture2D>("Sprites/Tiles/GoldOre");
        EmeraldOreTexture = Content.Load<Texture2D>("Sprites/Tiles/EmeraldOre");
        AmethystOreTexture = Content.Load<Texture2D>("Sprites/Tiles/AmethystOre");
    }

    // Items
    public static Item Grass => new PlaceableItem("Grass", 99, GrassTexture, id: 0, Tiles.Grass);
    public static Item Dirt => new PlaceableItem("Dirt", 99, DirtTexture, id: 1, Tiles.Dirt);
    public static Item Stone => new PlaceableItem("Stone", 99, StoneTexture, id: 2, Tiles.Stone);
    public static Item Log => new PlaceableItem("Log", 99, LogTexture, id: 3, Tiles.Log);
    public static Item Leaves => new PlaceableItem("Leaves", 99, LeavesTexture, id: 4, Tiles.Leaves);
    public static Item Planks => new PlaceableItem("Planks", 99, PlanksTexture, id: 5, Tiles.Planks);
    public static Item Stick => new Item("Stick", 99, StickTexture, id: 6);
    public static Item Leaf => new Item("Leaf", 99, LeafTexture, id: 7);
    public static Item Axe => new AxeItem("Iron axe", AxeTexture, id: 8, speed: 1f, power: 1);
    public static Item Pickaxe => new PickaxeItem("Iron pickaxe", PickaxeTexture, id: 9, speed: 1f, power: 1);
    public static Item Sword => new SwordItem("Iron sword", SwordTexture, id: 10, damage: 5);
    public static Item IronHelmet => new ArmorElementItem("Iron helmet", IronHelmetTexture, id: 11, armor: 2, element: ArmorElement.Helmet, IronHelmetAtlas);
    public static Item IronChestplate => new ArmorElementItem("Iron chestplate", IronChestplateTexture, id: 12, armor: 3, element: ArmorElement.Chestplate, IronChestplateAtlas);
    public static Item IronLeggins => new ArmorElementItem("Iron leggins", IronLegginsTexture, id: 13, armor: 2, element: ArmorElement.Leggins, IronLegginsAtlas);
    public static Item IronIngot => new Item("Iron ingot", maxStack: 99, IronIngotTexture, id: 14);
    public static Item GoldIngot => new Item("Gold ingot", maxStack: 99, GoldIngotTexture, id: 15);
    public static Item Emerald => new Item("Emerald", maxStack: 99, EmeraldTexture, id: 16);
    public static Item Amethyst => new Item("Amethyst", maxStack: 99, AmethystTexture, id: 17);
    public static Item IronOre => new PlaceableItem("Iron ore", 99, IronOreTexture, id: 18, Tiles.IronOre);
    public static Item GoldOre => new PlaceableItem("Gold ore", 99, GoldOreTexture, id: 19, Tiles.GoldOre);
    public static Item EmeraldOre => new PlaceableItem("Emerald ore", 99, EmeraldOreTexture, id: 20, Tiles.EmeraldOre);
    public static Item AmethystOre => new PlaceableItem("Amethyst ore", 99, AmethystOreTexture, id: 21, Tiles.AmethystOre);
}