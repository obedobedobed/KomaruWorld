using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class AxeItem(string name, Texture2D texture, int id, float speed)
: Item(name, 1, texture, id, true)
{
    public float Speed { get; private set; } = speed;
}