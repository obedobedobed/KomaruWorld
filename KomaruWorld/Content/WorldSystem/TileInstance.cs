using KomaruWorld.Content.Registries;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld.Content.WorldSystem;

public class TileInstance(int id, Vector2 position)
{
    public int TileID { get; private set; } = id;
    public Vector2 Position { get; private set; } = position;

    public Rectangle Rectangle 
    {
        get
        {
            return TilesRegistry.tiles[TileID].collideable ?
            new Rectangle((int)Position.X, (int)Position.Y, 8 * World.SIZE_MOD, 8 * World.SIZE_MOD) : Rectangle.Empty;
        }
    }

    public void DrawOutline(SpriteBatch spriteBatch)
    {
        TilesRegistry.tiles[TileID].DrawOutline(spriteBatch, Position);
    }

    public void DrawTile(SpriteBatch spriteBatch)
    {
        TilesRegistry.tiles[TileID].DrawTile(spriteBatch, Position);
    }
}