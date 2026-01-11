using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class ArmorSlot : GameObject
{
    public ArmorElementItem Item { get; private set; }
    public ArmorElement TargetElement { get; private set; }

    private Vector2 itemSize;
    private Rectangle itemRectangle
    {
        get
        {
            return new Rectangle
            (
                (int)(Position.X + Size.X / 2 - itemSize.X / 2),
                (int)(Position.Y + Size.Y / 2 - itemSize.Y / 2),
                (int)itemSize.X, (int)itemSize.Y
            );
        }
    }

    public ArmorSlot(Atlas atlas, Vector2 position, Vector2 size, int frame, Vector2 itemSize, ArmorElement element)
    : base(atlas, position, size, frame)
    {
        this.itemSize = itemSize;
        TargetElement = element;
    }

    public void UpdateItem(ArmorElementItem item) => Item = item;
    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(atlas.Texture, Rectangle, atlas.Rectangles[frame], Color.White);
        if (Item != null)
            spriteBatch.Draw(Item.Texture, itemRectangle, Color.White);
    }
}