using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class Tile : GameObject
{
    public bool CanCollide { get; private set; }
    public Rectangle Hitbox { get { return CanCollide ? Rectangle : Rectangle.Empty; } }
    private static int totalTiles = 0;
    public int TileWorldID { get; private set; }

    public Tile(Texture2D texture, Vector2 position, Vector2 size, bool canCollide)
    : base(texture, position, size)
    {
        CanCollide = canCollide;
        TileWorldID = ++totalTiles;
    }
}