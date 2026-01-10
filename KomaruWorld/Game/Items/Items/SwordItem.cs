using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class SwordItem(string name, Texture2D texture, int id, int damage)
: Item(name, 1, texture, id)
{
    public int Damage { get; private set; } = damage;
}