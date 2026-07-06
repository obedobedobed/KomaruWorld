using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld.Content.WorldSystem;

public abstract class Tile
{
    protected Texture2D texture;
    public Texture2D Texture { get => texture; protected set => texture = value; }
    protected int pixelWidth = 8;
    protected int pixelHeight = 8;
    public bool collideable { get; private set; } = true;
    public int TileID { get; private set; } = -1;

    public Tile() => SetDefaults();

    public void RegistryRegister(int ID)
    {
        TileID = ID;
    }

    public virtual void SetDefaults()
    {
        
    }

    public virtual void DrawOutline(SpriteBatch spriteBatch, Vector2 drawPosition)
    {
        var outlineSourceRectangle = new Rectangle
        (
            0, pixelHeight,
            pixelWidth, pixelHeight
        );

        spriteBatch.Draw(Texture, new Rectangle((int)drawPosition.X - World.SIZE_MOD, (int)drawPosition.Y - World.SIZE_MOD,
        (pixelWidth + 2) * World.SIZE_MOD, (pixelHeight + 2) * World.SIZE_MOD), outlineSourceRectangle, Color.White);
    }

    public virtual void DrawTile(SpriteBatch spriteBatch, Vector2 drawPosition)
    {
        var tileSourceRectangle = new Rectangle
        (
            0, 0,
            pixelWidth, pixelHeight
        );

        spriteBatch.Draw(Texture, new Rectangle((int)drawPosition.X, (int)drawPosition.Y,
        pixelWidth * World.SIZE_MOD, pixelHeight * World.SIZE_MOD), tileSourceRectangle, Color.White);
    }

    public static Vector2 TileToWorldPosition(Vector2 tilePos) => tilePos * 8;
    public static Vector2 WorldToTilePosition(Vector2 worldPos) => worldPos / 8;
}