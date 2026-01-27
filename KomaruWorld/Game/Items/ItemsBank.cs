using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class ItemsBank
{
    public static Atlas DestroyingAtlas { get; private set; }

    // Textures
    private static Texture2D GrassTexture;
    private static Texture2D DirtTexture;
    private static Texture2D StoneTexture;
    private static Texture2D LogTexture;
    private static Texture2D LeavesTexture;
    private static Texture2D PlanksTexture;
    private static Texture2D StickTexture;
    private static Texture2D LeafTexture;
    private static Texture2D AxeTexture;
    private static Texture2D PickaxeTexture;
    private static Texture2D SwordTexture;
    private static Texture2D IronHelmetTexture;
    private static Atlas IronHelmetAtlas;
    private static Texture2D IronChestplateTexture;
    private static Atlas IronChestplateAtlas;
    private static Texture2D IronLegginsTexture;
    private static Atlas IronLegginsAtlas;
    private static Texture2D IronIngotTexture;
    private static Texture2D GoldIngotTexture;
    private static Texture2D EmeraldTexture;
    private static Texture2D AmethystTexture;
    private static Texture2D IronOreTexture;
    private static Texture2D GoldOreTexture;
    private static Texture2D EmeraldOreTexture;
    private static Texture2D AmethystOreTexture;
    private static Texture2D GoldAxeTexture;
    private static Texture2D GoldPickaxeTexture;
    private static Texture2D GoldSwordTexture;
    private static Texture2D EmeraldAxeTexture;
    private static Texture2D EmeraldPickaxeTexture;
    private static Texture2D EmeraldSwordTexture;
    private static Texture2D AmethystAxeTexture;
    private static Texture2D AmethystPickaxeTexture;
    private static Texture2D AmethystSwordTexture;
    private static Texture2D DoorTexture;
    private static Texture2D SignTexture;

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
        GoldAxeTexture = Content.Load<Texture2D>("Sprites/Items/Tools/GoldAxe");
        GoldPickaxeTexture = Content.Load<Texture2D>("Sprites/Items/Tools/GoldPickaxe");
        GoldSwordTexture = Content.Load<Texture2D>("Sprites/Items/Tools/GoldSword");
        EmeraldAxeTexture = Content.Load<Texture2D>("Sprites/Items/Tools/EmeraldAxe");
        EmeraldPickaxeTexture = Content.Load<Texture2D>("Sprites/Items/Tools/EmeraldPickaxe");
        EmeraldSwordTexture = Content.Load<Texture2D>("Sprites/Items/Tools/EmeraldSword");
        AmethystAxeTexture = Content.Load<Texture2D>("Sprites/Items/Tools/AmethystAxe");
        AmethystPickaxeTexture = Content.Load<Texture2D>("Sprites/Items/Tools/AmethystPickaxe");
        AmethystSwordTexture = Content.Load<Texture2D>("Sprites/Items/Tools/AmethystSword");
        DoorTexture = Content.Load<Texture2D>("Sprites/Tiles/DoorTileClosed");
        SignTexture = Content.Load<Texture2D>("Sprites/Tiles/SignTile");
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
    public static Item Sword => new SwordItem("Iron sword", SwordTexture, id: 10, damage: 1);
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
    public static Item GoldAxe => new AxeItem("Gold axe", GoldAxeTexture, id: 22, speed: 1.4f, power: 2);
    public static Item GoldPickaxe => new PickaxeItem("Gold pickaxe", GoldPickaxeTexture, id: 23, speed: 1.4f, power: 2);
    public static Item GoldSword => new SwordItem("Gold sword", GoldSwordTexture, id: 24, damage: 2);
    public static Item EmeraldAxe => new AxeItem("Emerald axe", EmeraldAxeTexture, id: 25, speed: 2f, power: 3);
    public static Item EmeraldPickaxe => new PickaxeItem("Emerald pickaxe", EmeraldPickaxeTexture, id: 26, speed: 2f, power: 3);
    public static Item EmeraldSword => new SwordItem("Emerald sword", EmeraldSwordTexture, id: 27, damage: 4);
    public static Item AmethystAxe => new AxeItem("Amethyst axe", AmethystAxeTexture, id: 28, speed: 2.8f, power: 4);
    public static Item AmethystPickaxe => new PickaxeItem("Amethyst pickaxe", AmethystPickaxeTexture, id: 29, speed: 2.8f, power: 4);
    public static Item AmethystSword => new SwordItem("Amethyst sword", AmethystSwordTexture, id: 30, damage: 7);
    public static Item Door => new PlaceableItem("Door", 99, DoorTexture, id: 31, Tiles.Door);
    public static Item Sign => new PlaceableItem("Sign", 99, SignTexture, id: 32, Tiles.Sign);
}