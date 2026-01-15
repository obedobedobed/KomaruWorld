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

    public Game1()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Graphics.PreferredBackBufferHeight = 450;
        IsMouseVisible = false;
        Instance = this;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

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
            SceneManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        SceneManager.Scene.Draw();

        var mouse = Mouse.GetState();
        var cursorRectangle = new Rectangle(mouse.Position, CursorSize);
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
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
