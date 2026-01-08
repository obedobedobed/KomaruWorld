using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class GameScene(ContentManager content, SpriteBatch spriteBatch, GraphicsDeviceManager graphicsManager)
: Scene(content, spriteBatch, graphicsManager)
{
    private Player player;

    public override void Load()
    {
        var playerAtlas = new Atlas(texture: Content.Load<Texture2D>("Sprites/KomaruAtlas"), spriteSize: PlayerSize);
        player = new Player(playerAtlas, new Vector2(200, 100), PlayerSize * 4, defaultFrame: 1);

        // Test
        World.AddTile(new Tile(Content.Load<Texture2D>("Sprites/Tiles/GrassTile"), new Vector2(200, 300), TileSize * 4, true));
        World.AddTile(new Tile(Content.Load<Texture2D>("Sprites/Tiles/GrassTile"), new Vector2(232, 300), TileSize * 4, true));
        World.AddTile(new Tile(Content.Load<Texture2D>("Sprites/Tiles/GrassTile"), new Vector2(264, 300), TileSize * 4, true));
        World.AddTile(new Tile(Content.Load<Texture2D>("Sprites/Tiles/GrassTile"), new Vector2(168, 300), TileSize * 4, true));
        World.AddTile(new Tile(Content.Load<Texture2D>("Sprites/Tiles/GrassTile"), new Vector2(136, 300), TileSize * 4, true));
        World.AddTile(new Tile(Content.Load<Texture2D>("Sprites/Tiles/GrassTile"), new Vector2(328, 236), TileSize * 4, true));
        World.AddTile(new Tile(Content.Load<Texture2D>("Sprites/Tiles/GrassTile"), new Vector2(328, 268), TileSize * 4, true));
        World.AddTile(new Tile(Content.Load<Texture2D>("Sprites/Tiles/GrassTile"), new Vector2(72, 236), TileSize * 4, true));
        World.AddTile(new Tile(Content.Load<Texture2D>("Sprites/Tiles/GrassTile"), new Vector2(72, 268), TileSize * 4, true));
        // World.AddTile(new Tile(Content.Load<Texture2D>("Sprites/Tiles/DirtTile"), new Vector2(36, 300), TileSize * 4, true));
    }

    public override void Update(GameTime gameTime)
    {
        World.Update(gameTime);
        player.Update(gameTime);

        foreach (var _object in Objects)
            _object.Update(gameTime);
    }

    public override void Draw()
    {
        World.Draw(SpriteBatch);
        player.Draw(SpriteBatch);

        foreach (var _object in Objects)
            _object.Draw(SpriteBatch);
    }
}