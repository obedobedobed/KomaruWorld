using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class ArmorElementItem(string name, Texture2D texture, int id, int armor, ArmorElement element, Atlas armorAtlas)
: Item(name, 1, texture, id)
{
    public int Armor { get; private set; } = armor;
    public ArmorElement Element { get; private set; } = element;
    public Atlas ArmorAtlas { get; private set; } = armorAtlas;
}