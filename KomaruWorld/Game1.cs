using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class Game1 : Game
{
    public static Game1 Instance;
    public GraphicsDeviceManager Graphics { get; private set; }
    private SpriteBatch spriteBatch;
    private Texture2D cursorTexture;
    public int FPS = 0;

    // FPS counting
    private int fpsCounting = 0;
    private const float FPS_COUNT_TIME = 1f;
    private float fpsCountingTime = FPS_COUNT_TIME;

    // Input
    private KeyboardState lastKeyboard;

    // Window resizing
    private RenderTarget2D renderTarget;
    private Point defaultScreenSize = new Point(800, 450);

    public Game1()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        Instance = this;

        // Enable full screen by default
        // Graphics.IsFullScreen = true;

        Graphics.PreferredBackBufferWidth = defaultScreenSize.X;
        Graphics.PreferredBackBufferHeight = defaultScreenSize.Y;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        // If starting in full screen, match the screen resolution
        if (Graphics.IsFullScreen)
        {
            Graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            Graphics.ApplyChanges();
        }

        renderTarget = new RenderTarget2D(GraphicsDevice, VIRTUAL_WIDTH, VIRTUAL_HEIGHT);

        base.Initialize();

        Logger.Log("Game initialized");
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here

        Setup();
        TilesBank.LoadContent(Content);
        ItemsBank.LoadContent(Content);
        MobsBank.LoadContent(Content);
        Text.Setup(new Atlas(Content.Load<Texture2D>("Sprites/Font"), GlyphSize.ToVector2() / TEXT_MOD), GlyphSize);
        cursorTexture = Content.Load<Texture2D>("Sprites/Cursor");

        SceneManager.Load(new GameScene(Content, spriteBatch, Graphics));

        Logger.Log("Content loaded");
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add your update logic here
        
        if (IsActive)
        {
            var keyboard = Keyboard.GetState();

            // Quit Game
            if (keyboard.IsKeyDown(Keys.Escape) && !lastKeyboard.IsKeyDown(Keys.Escape))
            {
                var player = GameScene.Instance?.Player;
                bool inInventory = player != null && player.InInventory;

                if (!inInventory)
                {
                    Logger.Log("Good Bye!");
                    Logger.WriteLogs();
                    Exit();
                }
            }

            // Fullscreen Toggle
            bool f11Pressed = keyboard.IsKeyDown(Keys.F11) && !lastKeyboard.IsKeyDown(Keys.F11);
            bool altEnterPressed = (keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt)) 
                                   && keyboard.IsKeyDown(Keys.Enter) && !lastKeyboard.IsKeyDown(Keys.Enter);

            if (f11Pressed || altEnterPressed)
            {
                Graphics.IsFullScreen = !Graphics.IsFullScreen;
                if (Graphics.IsFullScreen)
                {
                    Graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                    Graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                }
                else
                {
                    Graphics.PreferredBackBufferWidth = defaultScreenSize.X;
                    Graphics.PreferredBackBufferHeight = defaultScreenSize.Y;
                }
                Graphics.ApplyChanges();
            }

            SceneManager.Update(gameTime);

            lastKeyboard = keyboard;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        SceneManager.Scene.Draw();

        GraphicsDevice.SetRenderTarget(null);

        spriteBatch.Begin(samplerState: SamplerState.AnisotropicWrap);

        var renderRectangle = CalculateRenderRectangle();
        spriteBatch.Draw(renderTarget, renderRectangle, Color.White);

        spriteBatch.End();

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        var mouse = Mouse.GetState();
        var cursorPos = new Point(mouse.X, mouse.Y);

        var cursorRectangle = new Rectangle(cursorPos, CursorSize);
        spriteBatch.Draw(cursorTexture, cursorRectangle, Color.White);

        spriteBatch.End();

        if ((fpsCountingTime -= (float)gameTime.ElapsedGameTime.TotalSeconds) <= 0)
        {
            FPS = fpsCounting;
            fpsCounting = 0;
            fpsCountingTime = FPS_COUNT_TIME;
        }

        fpsCounting++;

        base.Draw(gameTime);
    }

    private Rectangle CalculateRenderRectangle()
    {
        return new Rectangle
        (
            0, 0,
            Graphics.PreferredBackBufferWidth,
            Graphics.PreferredBackBufferHeight
        );
    }
}