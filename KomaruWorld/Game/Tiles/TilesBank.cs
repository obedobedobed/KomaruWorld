using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class TilesBank
{
    // Textures
    private static Texture2D GrassTexture;
    private static Texture2D DirtTexture;
    private static Texture2D StoneTexture;
    private static Texture2D LogTexture;
    private static Texture2D LeavesTexture;
    private static Texture2D PlanksTexture;
    private static Texture2D IronOreTexture;
    private static Texture2D GoldOreTexture;
    private static Texture2D EmeraldOreTexture;
    private static Texture2D AmethystOreTexture;
    private static Texture2D NatureLogTexture;
    private static Texture2D NatureLeavesTexture;

    public static void LoadContent(ContentManager Content)
    {
        var damageSound = Content.Load<SoundEffect>("Audio/SFX/TileDamage");
        var destroySound = Content.Load<SoundEffect>("Audio/SFX/TileDestroy");
        Tile.SetupSFX(damageSound, destroySound);

        GrassTexture = Content.Load<Texture2D>("Sprites/Tiles/GrassTile");
        DirtTexture = Content.Load<Texture2D>("Sprites/Tiles/DirtTile");
        StoneTexture = Content.Load<Texture2D>("Sprites/Tiles/StoneTile");
        LogTexture = Content.Load<Texture2D>("Sprites/Tiles/LogTile");
        LeavesTexture = Content.Load<Texture2D>("Sprites/Tiles/LeavesTile");
        PlanksTexture = Content.Load<Texture2D>("Sprites/Tiles/PlanksTile");
        IronOreTexture = Content.Load<Texture2D>("Sprites/Tiles/IronOre");
        GoldOreTexture = Content.Load<Texture2D>("Sprites/Tiles/GoldOre");
        EmeraldOreTexture = Content.Load<Texture2D>("Sprites/Tiles/EmeraldOre");
        AmethystOreTexture = Content.Load<Texture2D>("Sprites/Tiles/AmethystOre");
        NatureLogTexture = Content.Load<Texture2D>("Sprites/Tiles/NatureLogTile");
        NatureLeavesTexture = Content.Load<Texture2D>("Sprites/Tiles/NatureLeavesTile");
    }

    // Tiles
    public static Tile Grass(Vector2 position) => new Tile(GrassTexture, position, TileSize, true, Tiles.Grass, toolToDestroy: ToolToDestroy.Pickaxe, destroyTime: 0.7f, minimalToolPower: 1, new DropData([ItemsBank.Dirt], [100], [new Range(1, 1)]));
    public static Tile Dirt(Vector2 position) => new Tile(DirtTexture, position, TileSize, true, Tiles.Dirt, toolToDestroy: ToolToDestroy.Pickaxe, destroyTime: 0.7f, minimalToolPower: 1, new DropData([ItemsBank.Dirt], [100], [new Range(1, 1)]));
    public static Tile Stone(Vector2 position) => new Tile(StoneTexture, position, TileSize, true, Tiles.Stone, toolToDestroy: ToolToDestroy.Pickaxe, destroyTime: 0.9f, minimalToolPower: 1, new DropData([ItemsBank.Stone], [100], [new Range(1, 1)]));
    public static Tile Log(Vector2 position) => new Tile(LogTexture, position, TileSize, true, Tiles.Log, toolToDestroy: ToolToDestroy.Axe, destroyTime: 0.7f, minimalToolPower: 1, new DropData([ItemsBank.Planks], [100], [new Range(4, 4)]));
    public static Tile Leaves(Vector2 position) => new Tile(LeavesTexture, position, TileSize, true, Tiles.Leaves, toolToDestroy: ToolToDestroy.Both, destroyTime: 0.15f, minimalToolPower: 1, new DropData([ItemsBank.Leaf, ItemsBank.Stick], [100, 30], [new Range(1, 2), new Range(1, 1)]));
    public static Tile Planks(Vector2 position) => new Tile(PlanksTexture, position, TileSize, true, Tiles.Planks, toolToDestroy: ToolToDestroy.Axe, destroyTime: 0.35f, minimalToolPower: 1, new DropData([ItemsBank.Planks], [100], [new Range(1, 1)]));
    public static Tile IronOre(Vector2 position) => new Tile(IronOreTexture, position, TileSize, true, Tiles.IronOre, toolToDestroy: ToolToDestroy.Pickaxe, destroyTime: 1f, minimalToolPower: 1, new DropData([ItemsBank.IronOre], [100], [new Range(1, 1)]));
    public static Tile GoldOre(Vector2 position) => new Tile(GoldOreTexture, position, TileSize, true, Tiles.GoldOre, toolToDestroy: ToolToDestroy.Pickaxe, destroyTime: 1.2f, minimalToolPower: 1, new DropData([ItemsBank.GoldOre], [100], [new Range(1, 1)]));
    public static Tile EmeraldOre(Vector2 position) => new Tile(EmeraldOreTexture, position, TileSize, true, Tiles.EmeraldOre, toolToDestroy: ToolToDestroy.Pickaxe, destroyTime: 1.4f, minimalToolPower: 2, new DropData([ItemsBank.EmeraldOre], [100], [new Range(1, 1)]));
    public static Tile AmethystOre(Vector2 position) => new Tile(AmethystOreTexture, position, TileSize, true, Tiles.AmethystOre, toolToDestroy: ToolToDestroy.Pickaxe, destroyTime: 1.6f, minimalToolPower: 3, new DropData([ItemsBank.AmethystOre], [100], [new Range(1, 1)]));
    public static Tile NatureLog(Vector2 position) => new Tile(NatureLogTexture, position, TileSize, false, Tiles.Log, toolToDestroy: ToolToDestroy.Axe, destroyTime: 0.7f, minimalToolPower: 1, new DropData([ItemsBank.Planks], [100], [new Range(4, 4)]));
    public static Tile NatureLeaves(Vector2 position) => new Tile(NatureLeavesTexture, position, TileSize, false, Tiles.Leaves, toolToDestroy: ToolToDestroy.Both, destroyTime: 0.15f, minimalToolPower: 1, new DropData([ItemsBank.Leaf, ItemsBank.Stick], [100, 30], [new Range(1, 2), new Range(1, 1)]));

    public static Tile FindTile(Tiles tile, Vector2 position)
    {
        return tile switch
        {
            Tiles.Grass => Grass(position),
            Tiles.Dirt => Dirt(position),
            Tiles.Stone => Stone(position),
            Tiles.Log => Log(position),
            Tiles.Leaves => Leaves(position),
            Tiles.Planks => Planks(position),
            Tiles.IronOre => IronOre(position),
            Tiles.GoldOre => GoldOre(position),
            Tiles.EmeraldOre => EmeraldOre(position),
            Tiles.AmethystOre => AmethystOre(position),
            Tiles.NatureLog => NatureLog(position),
            Tiles.NatureLeaves => NatureLeaves(position),
            _ => null,
        };
    }
}