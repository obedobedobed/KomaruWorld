using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld.Content.SceneSystem;

public abstract class GameScene(SpriteBatch sp, GraphicsDeviceManager gm, ContentManager c)
{
    protected float deltaTime = 0f;
    protected SpriteBatch spriteBatch = sp;
    protected GraphicsDeviceManager graphicsManager = gm;
    protected ContentManager Content = c;

    public virtual void Load()
    {
        
    }

    public virtual void Update(GameTime gameTime)
    {
        deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public virtual void Draw()
    {
        
    }
}