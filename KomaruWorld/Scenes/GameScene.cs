using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class GameScene(ContentManager content, SpriteBatch spriteBatch, GraphicsDeviceManager graphicsManager)
: Scene(content, spriteBatch, graphicsManager)
{
    private Player player;
    private KeyboardState lastKeyboard;

    // Debug
    private bool debugMenuOpened = false;
    // private bool lockedFps = true;

    // FPS Counting
    private int countingFPS = 0;
    private const float FPS_COUNT_TIME = 1f;
    private float timeToCountFps = FPS_COUNT_TIME;
    private int FPS = 0;

    public override void Load()
    {
        var playerAtlas = new Atlas(texture: Content.Load<Texture2D>("Sprites/KomaruAtlas"), spriteSize: PlayerSize);
        player = new Player(playerAtlas, new Vector2(200, 100), PlayerSize * 4, defaultFrame: 1);

        // Test
        World.AddTile(TilesBank.Grass(new Vector2(200, 300)));
        World.AddTile(TilesBank.Stone(new Vector2(232, 300)));
        World.AddTile(TilesBank.Grass(new Vector2(264, 300)));
        World.AddTile(TilesBank.Leaf(new Vector2(168, 300)));
        World.AddTile(TilesBank.Leaf(new Vector2(136, 300)));
        World.AddTile(TilesBank.Log(new Vector2(328, 236)));
        World.AddTile(TilesBank.Grass(new Vector2(328, 268)));
        World.AddTile(TilesBank.Dirt(new Vector2(72, 236)));
        World.AddTile(TilesBank.Dirt(new Vector2(72, 268)));
    }

    public override void Update(GameTime gameTime)
    {
        World.Update(gameTime);
        player.Update(gameTime);

        foreach (var _object in Objects)
            _object.Update(gameTime);

        var keyboard = Keyboard.GetState();

        // Functions
        if (keyboard.IsKeyDown(Keys.F1) && !lastKeyboard.IsKeyDown(Keys.F1))
            debugMenuOpened = !debugMenuOpened;

        // if (keyboard.IsKeyDown(Keys.F2) && !lastKeyboard.IsKeyDown(Keys.F2))
        // {
        //     GraphicsManager.SynchronizeWithVerticalRetrace = !GraphicsManager.SynchronizeWithVerticalRetrace;
        //     Game1.Instance.IsFixedTimeStep = !Game1.Instance.IsFixedTimeStep;
        //     lockedFps = !lockedFps;
        //     GraphicsManager.ApplyChanges();
        // }

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
        World.Draw(SpriteBatch);
        player.Draw(SpriteBatch);

        foreach (var _object in Objects)
            _object.Draw(SpriteBatch);

        if (debugMenuOpened)
        {
            // string vsyncString = lockedFps ? "VSync, p" : "P";
            // string lockedFpsString = lockedFps ? "un" : string.Empty;

            Text.Write("DEBUG MENU (F1 TO CLOSE)", Vector2.Zero, Color.White, SpriteBatch);
            // Text.Write($"FPS:{FPS} ({vsyncString}ress F2 for {lockedFpsString}lock)",
            // new Vector2(0, GlyphSize.Y * 2 * 1 + 2 * 1), Color.White, SpriteBatch);
            Text.Write($"FPS:{FPS} (Fixed timestep)",
            new Vector2(0, GlyphSize.Y * 2 * 1 + 2 * 1), Color.White, SpriteBatch);
            Text.Write($"Player position: x:{(int)player.Position.X}, y:{(int)player.Position.Y}",
            new Vector2(0, GlyphSize.Y * 2 * 2 + 2 * 2), Color.White, SpriteBatch);
            Text.Write($"Player gravity: {(int)player.GravityMod}",
            new Vector2(0, GlyphSize.Y * 2 * 3 + 2 * 3), Color.White, SpriteBatch);
        }
    }
}