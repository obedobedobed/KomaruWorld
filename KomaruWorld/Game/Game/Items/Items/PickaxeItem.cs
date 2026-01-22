using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class PickaxeItem(string name, Texture2D texture, int id, float speed, int power)
: Item(name, 1, texture, id, true)
{
    public float Speed { get; private set; } = speed;
    public int Power { get; private set; } = power;
}