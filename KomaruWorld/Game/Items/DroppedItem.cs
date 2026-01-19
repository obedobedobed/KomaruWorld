using Microsoft.Xna.Framework;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class DroppedItem : GameObject
{
    private float gravityVelocity;
    private bool isGrounded;

    public Item Item { get; private set; }
    private static int totalItems = 0;
    
    
    // Local ID (kept for safety)
    public int ItemWorldId;
    
    // Network ID (used for multiplayer sync)
    public int NetworkId { get; set; } = -1;

    public DroppedItem(Item item, Vector2 position) : base(item.Texture, position, DroppedItemSize)
    {
        Item = item;
        ItemWorldId = ++totalItems;
    }

    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        gravityVelocity += GRAVITY_ACELERATION * deltaTime;

        if (gravityVelocity > MAXIMAL_GRAVITY)
            gravityVelocity = MAXIMAL_GRAVITY;

        isGrounded = false;
        var nextHitbox = new Rectangle(Rectangle.X, (int)(Rectangle.Y + gravityVelocity), Rectangle.Width, Rectangle.Height);

        foreach (var tile in World.Tiles)
        {
            if (nextHitbox.Intersects(tile.Hitbox))
            {
                isGrounded = true;
                gravityVelocity = DEFAULT_GRAVITY;
                Position = new Vector2(Position.X, tile.Rectangle.Top - Rectangle.Height);
                break;
            }
        }

        if (isGrounded)
            return;
        
        Position += new Vector2(0f, gravityVelocity);
    }
}