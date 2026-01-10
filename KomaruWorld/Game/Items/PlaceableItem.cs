using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class PlaceableItem(string name, int maxStack, Texture2D texture, int id, Tiles placeTile)
: Item(name, maxStack, texture, id)
{
    public Tiles ItemTile { get; private set; } = placeTile;
}