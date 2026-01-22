using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public abstract class Scene(ContentManager content, SpriteBatch spriteBatch, GraphicsDeviceManager graphicsManager)
{
    protected ContentManager Content = content;
    protected SpriteBatch SpriteBatch = spriteBatch;
    protected GraphicsDeviceManager GraphicsManager = graphicsManager;
    
    public List<GameObject> Objects { get; protected set; } = new List<GameObject>();

    public abstract void Load();
    public abstract void Update(GameTime gameTime);
    public abstract void Draw();
}