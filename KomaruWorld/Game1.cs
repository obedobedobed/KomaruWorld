using KomaruWorld.Content.Registries;
using KomaruWorld.Content.Scenes;
using KomaruWorld.Content.SceneSystem;
using KomaruWorld.Content.TextSystem;
using KomaruWorld.Content.WorldSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace KomaruWorld;

public class Game1 : Game
{
    public GraphicsDeviceManager Graphics { get; private set; }
    private SpriteBatch spriteBatch;
    private RenderTarget2D renderTarget;

    public static Game1 Instance;

    public const int PIXEL_SCREEN_WIDTH = (int)(320 * World.SIZE_MOD / 1.25f);
    public const int PIXEL_SCREEN_HEIGHT = (int)(180 * World.SIZE_MOD / 1.25f);

    public Game1()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Graphics.PreferredBackBufferWidth = 800;
        Graphics.PreferredBackBufferHeight = 450;
    }

    protected override void Initialize()
    {
        base.Initialize();

        Instance = this;

        renderTarget = new RenderTarget2D(Graphics.GraphicsDevice, PIXEL_SCREEN_WIDTH, PIXEL_SCREEN_HEIGHT);
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        
        TexturesRegistry.Register(Content);
        TilesRegistry.Register();
        Text.Setup();

        SceneManager.Load(new WorldScene(spriteBatch, Graphics, Content));
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        SceneManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SceneManager.Draw();

        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        spriteBatch.Draw(renderTarget, new Rectangle(0, 0,
        Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight), Color.White);

        spriteBatch.End();

        base.Draw(gameTime);
    }
}
