using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class Item(string name, int maxStack, Texture2D texture, int id, bool isTool = false)
{
    public Texture2D Texture { get; protected set; } = texture;
    public string Name { get; protected set; } = name;
    public int MaxStack { get; protected set; } = maxStack;
    public int ID { get; private set; } = id;
    public bool IsTool { get; private set; } = isTool;
}