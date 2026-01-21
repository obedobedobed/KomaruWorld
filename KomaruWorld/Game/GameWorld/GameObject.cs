using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class GameObject
{
    protected Atlas atlas;
    public Texture2D texture;
    public Vector2 Position { get; protected set; }
    public Vector2 Size { get; protected set; }
    protected int frame;

    protected const float DEFAULT_GRAVITY = 0.25f * SIZE_MOD;
    protected const float GRAVITY_ACELERATION = 2.5f * SIZE_MOD;
    protected const float MAXIMAL_GRAVITY = 6f * SIZE_MOD;

    public Rectangle Rectangle
    {
        get
        {
            return new Rectangle
            (
                (int)Position.X, (int)Position.Y,
                (int)Size.X, (int)Size.Y
            );
        }
    }

    public GameObject(Atlas atlas, Vector2 position, Vector2 size, int defaultFrame)
    {
        this.atlas = atlas;
        Position = position;
        Size = size;
        frame = defaultFrame;
    }

    public GameObject(Texture2D texture, Vector2 position, Vector2 size)
    {
        this.texture = texture;
        Position = position;
        Size = size;
    }

    public virtual void Update(GameTime gameTime)
    {
        
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (atlas != null)
            spriteBatch.Draw
            (
                atlas.Texture, Rectangle,
                atlas.Rectangles[frame], Color.White
            );
        else
            spriteBatch.Draw(texture, Rectangle, Color.White);
    }
}