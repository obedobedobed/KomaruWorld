using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KomaruWorld;

public class Slot : GameObject
{
    public Item Item { get; private set; }
    public int ItemCount { get; private set; }

    private bool choosed;
    private int defaultFrame;
    private int choosedFrame;
    private Vector2 itemSize;
    private Rectangle itemRectangle
    {
        get
        {
            return new Rectangle
            (
                (int)(Position.X + itemSize.X / 2),
                (int)(Position.Y + itemSize.X / 2),
                (int)itemSize.X, (int)itemSize.Y
            );
        }
    }

    public Slot(Atlas atlas, Vector2 position, Vector2 size, int defaultFrame, int choosedFrame, Vector2 itemSize) :
    base(atlas, position, size, defaultFrame)
    {
        this.defaultFrame = defaultFrame;
        this.choosedFrame = choosedFrame;
        this.itemSize = itemSize;
    }

    public void UpdateFrame() => frame = choosed ? choosedFrame : defaultFrame;
    public void UpdateItem(Item item) => Item = item;
    public void CountItem(bool countBack) => ItemCount += countBack ? -1 : 1;
    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(atlas.Texture, Rectangle, atlas.Rectangles[frame], Color.White);
        if (Item != null)
            spriteBatch.Draw(Item.Texture, itemRectangle, Color.White);
    }
}