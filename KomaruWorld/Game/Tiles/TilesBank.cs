using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class TilesBank
{
    // Textures
    public static Texture2D GrassTexture { get; private set; }
    public static Texture2D DirtTexture { get; private set; }
    public static Texture2D StoneTexture { get; private set; }
    public static Texture2D LogTexture { get; private set; }
    public static Texture2D LeavesTexture { get; private set; }
    public static Texture2D PlanksTexture { get; private set; }

    public static void LoadTextures(ContentManager Content)
    {
        GrassTexture = Content.Load<Texture2D>("Sprites/Tiles/GrassTile");
        DirtTexture = Content.Load<Texture2D>("Sprites/Tiles/DirtTile");
        StoneTexture = Content.Load<Texture2D>("Sprites/Tiles/StoneTile");
        LogTexture = Content.Load<Texture2D>("Sprites/Tiles/LogTile");
        LeavesTexture = Content.Load<Texture2D>("Sprites/Tiles/LeavesTile");
        PlanksTexture = Content.Load<Texture2D>("Sprites/Tiles/PlanksTile");
    }

    // Tiles
    public static Tile Grass(Vector2 position) => new Tile(GrassTexture, position, TileSize, true, Tiles.Grass, new DropData([ItemsBank.Grass], [100], [new Range(1, 1)]));
    public static Tile Dirt(Vector2 position) => new Tile(DirtTexture, position, TileSize, true, Tiles.Dirt, new DropData([ItemsBank.Dirt], [100], [new Range(1, 1)]));
    public static Tile Stone(Vector2 position) => new Tile(StoneTexture, position, TileSize, true, Tiles.Stone, new DropData([ItemsBank.Stone], [100], [new Range(1, 1)]));
    public static Tile Log(Vector2 position) => new Tile(LogTexture, position, TileSize, true, Tiles.Log, new DropData([ItemsBank.Planks], [100], [new Range(4, 4)]));
    public static Tile Leaves(Vector2 position) => new Tile(LeavesTexture, position, TileSize, true, Tiles.Leaves, new DropData([ItemsBank.Leaf, ItemsBank.Stick], [100, 50], [new Range(1, 2), new Range(1, 1)]));
    public static Tile Planks(Vector2 position) => new Tile(PlanksTexture, position, TileSize, true, Tiles.Planks, new DropData([ItemsBank.Planks], [100], [new Range(1, 1)]));

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
            _ => null,
        };
    }
}