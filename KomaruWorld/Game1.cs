using System;
using KomaruWorld;
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
    
    //Network
    public NetworkManager NetworkManager; 

    //Dev Console
    public DevConsole Console;
    
    public Game1()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        Instance = this;

        // Enable full screen by default
        Graphics.IsFullScreen = true;
        
        // Ensure game continues running when minimized
        this.InactiveSleepTime = TimeSpan.Zero;
    }

    protected override void Initialize()
    {
        this.Exiting += (sender, args) => { NetworkManager?.Stop(); };
        
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
        
        
        NetworkManager = new NetworkManager();
        // Initialize Console
        Console = new DevConsole(GraphicsDevice, Window);
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add your update logic here
        
        // 1. Update Console (Toggling)
        if (Console != null) Console.Update();
            
        // 2. Network always runs
        if (NetworkManager != null) NetworkManager.Update();
        
        // Quit Game Logic
        var keyboard = Keyboard.GetState();
        if (IsActive)
        {
            // We check !lastKeyboard to ensure we don't exit immediately if holding Escape from a previous action
            if (keyboard.IsKeyDown(Keys.Escape) && !lastKeyboard.IsKeyDown(Keys.Escape))
            {
                // Check if the player is currently in the inventory to prevent exiting the game
                // when the user just intends to close the menu.
                var player = GameScene.Instance?.Player;
                bool inInventory = player != null && player.InInventory;

                if (!inInventory)
                {
                    Exit();
                }
            }

            // Fullscreen Toggle Logic: F11 OR Alt+Enter
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

            var renderRectangle = CalculateRenderRectangle();
            var mouse = Mouse.GetState();

            int minX = renderRectangle.X;
            int maxX = renderRectangle.Right - 1;
            int minY = renderRectangle.Y;
            int maxY = renderRectangle.Bottom - 1;

            int clampedX = Math.Clamp(mouse.X, minX, maxX);
            int clampedY = Math.Clamp(mouse.Y, minY, maxY);

            if (mouse.X != clampedX || mouse.Y != clampedY)
                Mouse.SetPosition(clampedX, clampedY);

            lastKeyboard = keyboard;
        }
        
        // Always update scene
        SceneManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        SceneManager.Scene.Draw();

        // Draw the Console on top of the game
        if (Console != null && Console.IsOpen)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            Console.Draw(spriteBatch);
            spriteBatch.End();
        }
        
        GraphicsDevice.SetRenderTarget(null);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        var renderRectangle = CalculateRenderRectangle();
        spriteBatch.Draw(renderTarget, renderRectangle, Color.White);

        var mouse = Mouse.GetState();

        var cursorPosVec2 = new Vector2
        (
            (mouse.Position.X - renderRectangle.X) / (VIRTUAL_WIDTH  / (float)Window.ClientBounds.Width),
            (mouse.Position.Y - renderRectangle.Y) / (VIRTUAL_HEIGHT / (float)Window.ClientBounds.Height)
        );

        var cursorPos = cursorPosVec2.ToPoint();

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
        // Point renderSize;

        // if (Window.ClientBounds.Width > Window.ClientBounds.Height)
        // {
        //     float scale = Window.ClientBounds.Height / VIRTUAL_HEIGHT;

        //     renderSize.Y = Window.ClientBounds.Height;
        //     renderSize.X = (int)(VIRTUAL_WIDTH * scale);
        // }
        // else
        // {
        //     float scale = Window.ClientBounds.Width / VIRTUAL_WIDTH;

        //     renderSize.X = Window.ClientBounds.Width;
        //     renderSize.Y = (int)(VIRTUAL_HEIGHT * scale);
        // }

        return new Rectangle
        (
            // Window.ClientBounds.Width / 2 - renderSize.X / 2,
            // Window.ClientBounds.Height / 2 - renderSize.Y / 2,
            // renderSize.X,
            // renderSize.Y
            0, 0,
            Graphics.PreferredBackBufferWidth,
            Graphics.PreferredBackBufferHeight
        );
    }
}