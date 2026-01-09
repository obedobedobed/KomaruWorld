using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class GameScene(ContentManager content, SpriteBatch spriteBatch, GraphicsDeviceManager graphicsManager)
: Scene(content, spriteBatch, graphicsManager)
{
    public static GameScene Instance;
    public OrthographicCamera camera { get; private set; } = new OrthographicCamera(graphicsManager.GraphicsDevice);
    private Player player;
    private KeyboardState lastKeyboard;
    private Texture2D pixel;

    // World
    private int worldWidth = 60;
    private int worldHeight = 40;

    // Debug
    private bool debugMenuOpened = false;

    // FPS Counting
    private int countingFPS = 0;
    private const float FPS_COUNT_TIME = 1f;
    private float timeToCountFps = FPS_COUNT_TIME;
    private int FPS = 0;

    public override void Load()
    {
        Instance = this;
        pixel = Content.Load<Texture2D>("Sprites/Pixel");

        WorldGenerator.Generate(worldWidth, worldHeight);

        var playerAtlas = new Atlas(texture: Content.Load<Texture2D>("Sprites/KomaruAtlas"), spriteSize: PlayerSize / SIZE_MOD);
        var slotAtlas = new Atlas(texture: Content.Load<Texture2D>("Sprites/UI/SlotAtlas"), spriteSize: SlotSize / SIZE_MOD);
        player = new Player(playerAtlas, new Vector2(worldWidth * TileSize.X / 2, 100), PlayerSize,
        defaultFrame: 1, slotAtlas: slotAtlas);

        camera.Position = player.Position;
    }

    public override void Update(GameTime gameTime)
    {
        World.Update(gameTime);
        player.Update(gameTime);

        var playerPosCentered = player.Position - new Vector2
        (GraphicsManager.PreferredBackBufferWidth / 2, GraphicsManager.PreferredBackBufferHeight / 2) +
        player.Size / 2;
        camera.Position = Vector2.Lerp(camera.Position, playerPosCentered, 0.1f);

        foreach (var _object in Objects)
            _object.Update(gameTime);

        var keyboard = Keyboard.GetState();

        // Functions
        if (keyboard.IsKeyDown(Keys.F1) && !lastKeyboard.IsKeyDown(Keys.F1))
            debugMenuOpened = !debugMenuOpened;

        if ((timeToCountFps -= (float)gameTime.ElapsedGameTime.TotalSeconds) <= 0)
        {
            FPS = countingFPS;
            countingFPS = 0;
            timeToCountFps = FPS_COUNT_TIME;
        }

        countingFPS++;

        lastKeyboard = keyboard;
    }

    public override void Draw()
    {
        var view = camera.GetViewMatrix();

        // Game world (applying transform matrix)
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: view);

        World.Draw(SpriteBatch);
        player.Draw(SpriteBatch);

        foreach (var _object in Objects)
            _object.Draw(SpriteBatch);

        SpriteBatch.End(); 

        // UI (not applying transform matrix)
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        if (player.InInventory)
        {
            SpriteBatch.Draw(pixel, new Rectangle(0, 0, GraphicsManager.PreferredBackBufferWidth,
            GraphicsManager.PreferredBackBufferHeight), new Color(0, 0, 0, 150));
            player.Inventory.DrawInventory(SpriteBatch); 
        }

        player.Inventory.DrawHotbar(SpriteBatch);

        Texture2D tileInHandTexture = player.TileInHand switch
        {
            0 => TilesBank.GrassTexture,
            1 => TilesBank.DirtTexture,
            2 => TilesBank.StoneTexture, 
            3 => TilesBank.LogTexture, 
            4 => TilesBank.LeafTexture, 
            5 => TilesBank.PlanksTexture,
            _ => null
        };

        if (tileInHandTexture != null)
            SpriteBatch.Draw
            (
                tileInHandTexture, new Rectangle
                (
                    (int)(GraphicsManager.PreferredBackBufferWidth - TileSize.X - UI_SPACING), UI_SPACING,
                    (int)TileSize.X, (int)TileSize.Y
                ), Color.White
            );

        if (debugMenuOpened)
        {
            Text.Write($"{GAME_NAME} - v{GAME_VERSION}",
            new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 0 + UI_SPACING - GlyphSize.Y / 2 * 0),
            Color.White, SpriteBatch);
            Text.Write($"FPS:{FPS} (Fixed timestep)",
            new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 1 + UI_SPACING - GlyphSize.Y / 2 * 1),
            Color.White, SpriteBatch);
            Text.Write($"Position: x{(int)player.Position.X}, y{(int)player.Position.Y}",
            new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 2 + UI_SPACING - GlyphSize.Y / 2 * 2),
            Color.White, SpriteBatch);
            Text.Write($"Gravity: {(int)player.GravityMod}",
            new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 3 + UI_SPACING - GlyphSize.Y / 2 * 3),
            Color.White, SpriteBatch);
            Text.Write($"Tile: {player.TileInHand}",
            new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 4 + UI_SPACING - GlyphSize.Y / 2 * 4),
            Color.White, SpriteBatch);
            Text.Write($"Cursor: x{player.cursorPosition.X}, y{player.cursorPosition.Y}",
            new Vector2(UI_SPACING, GlyphSize.Y * TEXT_SPACING * 5 + UI_SPACING - GlyphSize.Y / 2 * 5),
            Color.White, SpriteBatch);

            int screenHeight = GraphicsManager.PreferredBackBufferHeight;

            long usedMemory = GC.GetTotalMemory(false) / 1024;
            long totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024 / 1024;

            Text.Write($"OS: {RuntimeInformation.OSDescription}",
            new Vector2(UI_SPACING, screenHeight - GlyphSize.Y * TEXT_SPACING * 4 + GlyphSize.Y / 2 * 4),
            Color.White, SpriteBatch);
            Text.Write($"CPU: {Environment.ProcessorCount} threads CPU",
            new Vector2(UI_SPACING, screenHeight - GlyphSize.Y * TEXT_SPACING * 3 + GlyphSize.Y / 2 * 3),
            Color.White, SpriteBatch);
            Text.Write($"Memory: {usedMemory}MB/{totalMemory}MB used",
            new Vector2(UI_SPACING, screenHeight - GlyphSize.Y * TEXT_SPACING * 2 + GlyphSize.Y / 2 * 2),
            Color.White, SpriteBatch);
            Text.Write($"GPU: {GraphicsAdapter.DefaultAdapter.Description}",
            new Vector2(UI_SPACING, screenHeight - GlyphSize.Y * TEXT_SPACING * 1 + GlyphSize.Y / 2 * 1),
            Color.White, SpriteBatch);
        }

        SpriteBatch.End();
    }
}