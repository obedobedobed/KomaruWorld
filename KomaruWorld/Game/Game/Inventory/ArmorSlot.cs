using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class ArmorSlot : GameObject
{
    public ArmorElementItem Item { get; private set; }
    public ArmorElement TargetElement { get; private set; }

    private Rectangle itemRectangle
    {
        get
        {
            return new Rectangle
            (
                (int)(Position.X + Size.X / 2 - ItemSize.X / 2),
                (int)(Position.Y + Size.Y / 2 - ItemSize.Y / 2),
                (int)ItemSize.X, (int)ItemSize.Y
            );
        }
    }

    public ArmorSlot(Atlas atlas, Vector2 position, Vector2 size, int frame, ArmorElement element)
    : base(atlas, position, size, frame)
    {
        TargetElement = element;
    }

    public void UpdateItem(ArmorElementItem item) => Item = item;
    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(atlas.Texture, Rectangle, atlas.Rectangles[Frame], Color.White);
        if (Item != null)
            spriteBatch.Draw(Item.Texture, itemRectangle, Color.White);
    }
}