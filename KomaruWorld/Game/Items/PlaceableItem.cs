using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class PlaceableItem(string name, int maxStack, Texture2D texture, Tiles placeTile) : Item(name, maxStack, texture)
{
    public Tiles PlaceTile { get; private set; } = placeTile;
}