using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class Item(string name, int maxStack, Texture2D texture)
{
    public Texture2D Texture { get; protected set; } = texture;
    public string Name { get; protected set; } = name;
    public int MaxStack { get; protected set; } = maxStack;
}