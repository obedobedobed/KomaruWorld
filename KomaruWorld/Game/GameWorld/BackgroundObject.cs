using Microsoft.Xna.Framework;

namespace KomaruWorld;

public class BackgroundObject : GameObject
{
    public BackgroundObject(Atlas atlas, Vector2 position, Vector2 size, int defaultFrame)
    : base(atlas, position, size, defaultFrame)
    {
        
    }

    public void Move(Vector2 velocity)
    {
        Position += velocity;
    }
}