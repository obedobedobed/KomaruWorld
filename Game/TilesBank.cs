using Microsoft.Xna.Framework;
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
    private static Texture2D LeafTexture;
    private static Texture2D PlanksTexture;

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
}