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

    // Window resizing
    private RenderTarget2D renderTarget;

    public Game1()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
        Instance = this;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        renderTarget = new RenderTarget2D(GraphicsDevice, VIRTUAL_WIDTH, VIRTUAL_HEIGHT);
        Graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
        Graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
        Graphics.IsFullScreen = true;
        Graphics.ApplyChanges();

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
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add your update logic here
        
        if (IsActive)
        {
            var renderRectangle = new Rectangle
            (
                0, 0,
                renderTarget.Width,
                renderTarget.Height
            );
            var mouse = Mouse.GetState();

            int minX = renderRectangle.X;
            int maxX = renderRectangle.Right - 1;
            int minY = renderRectangle.Y;
            int maxY = renderRectangle.Bottom - 1;

            int clampedX = Math.Clamp(mouse.X, minX, maxX);
            int clampedY = Math.Clamp(mouse.Y, minY, maxY);

            if (mouse.X != clampedX || mouse.Y != clampedY)
                Mouse.SetPosition(clampedX, clampedY);

            SceneManager.Update(gameTime);
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

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        var renderRectangle = new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
        spriteBatch.Draw(renderTarget, renderRectangle, Color.White);

        var mouse = Mouse.GetState();

        var cursorPos = new Vector2
        (
            mouse.Position.X / (VIRTUAL_WIDTH / (float)Graphics.PreferredBackBufferWidth),
            mouse.Position.Y / (VIRTUAL_HEIGHT / (float)Graphics.PreferredBackBufferHeight)

        ).ToPoint();

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
}
