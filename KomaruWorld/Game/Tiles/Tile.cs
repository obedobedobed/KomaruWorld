using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class Tile : GameObject
{
    public bool CanCollide { get; private set; }
    public Rectangle Hitbox { get { return CanCollide ? Rectangle : Rectangle.Empty; } }
    private static int totalTiles = 0;
    public int TileWorldID { get; private set; }
    public Tiles TileType { get; private set; }
    public DropData DropData { get; private set; }

    public Tile(Texture2D texture, Vector2 position, Vector2 size, bool canCollide, Tiles tileType, DropData drop)
    : base(texture, position, size)
    {
        CanCollide = canCollide;
        TileWorldID = ++totalTiles;
        TileType = tileType;
        DropData = drop;
    }

    public void Drop()
    {
        var drop = DropData.CalculateDrop();

        foreach (var dropItem in drop)
            for (int i = 0; i < dropItem.Amount; i++)
                World.AddItem(new DroppedItem(dropItem.Item, Position));
    }
}