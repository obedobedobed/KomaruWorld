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
    public static Texture2D LeafTexture { get; private set; }
    public static Texture2D PlanksTexture { get; private set; }

    public static void LoadTextures(ContentManager Content)
    {
        GrassTexture = Content.Load<Texture2D>("Sprites/Tiles/GrassTile");
        DirtTexture = Content.Load<Texture2D>("Sprites/Tiles/DirtTile");
        StoneTexture = Content.Load<Texture2D>("Sprites/Tiles/StoneTile");
        LogTexture = Content.Load<Texture2D>("Sprites/Tiles/LogTile");
        LeafTexture = Content.Load<Texture2D>("Sprites/Tiles/LeafTile");
        PlanksTexture = Content.Load<Texture2D>("Sprites/Tiles/PlanksTile");
    }

    // Tiles
    public static Tile Grass(Vector2 position) => new Tile(GrassTexture, position, TileSize * 4, true);
    public static Tile Dirt(Vector2 position) => new Tile(DirtTexture, position, TileSize * 4, true);
    public static Tile Stone(Vector2 position) => new Tile(StoneTexture, position, TileSize * 4, true);
    public static Tile Log(Vector2 position) => new Tile(LogTexture, position, TileSize * 4, true);
    public static Tile Leaf(Vector2 position) => new Tile(LeafTexture, position, TileSize * 4, true);
    public static Tile Planks(Vector2 position) => new Tile(PlanksTexture, position, TileSize * 4, true);

    public static Tile FindTile(Tiles tile, Vector2 position)
    {
        return tile switch
        {
            Tiles.Grass => Grass(position),
            Tiles.Dirt => Dirt(position),
            Tiles.Stone => Stone(position),
            Tiles.Log => Log(position),
            Tiles.Leaf => Leaf(position),
            Tiles.Planks => Planks(position),
            _ => null,
        };
    }
}