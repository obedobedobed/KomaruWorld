using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class Slot : GameObject
{
    public Item Item { get; private set; }
    public int ItemAmount { get; private set; }

    private int slotId;
    private int defaultFrame;
    private int choosedFrame;
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
    private Vector2 itemCountPosition { get { return new Vector2(Position.X + Size.X, Position.Y + Size.Y - GlyphSize.Y); } }

    public Slot(Atlas atlas, Vector2 position, Vector2 size, int defaultFrame, int choosedFrame, int slotId) :
    base(atlas, position, size, defaultFrame)
    {
        this.defaultFrame = defaultFrame;
        this.choosedFrame = choosedFrame;
        this.slotId = slotId;
    }

    public void UpdateFrame() => frame = GameScene.Instance.Player.HotbarSlot == slotId ? choosedFrame : defaultFrame;
    public void UpdateItem(Item item)
    {
        Item = item;
        ItemAmount = 1;
    }
    public void UpdateItem(Item item, int count)
    {
        Item = item;
        ItemAmount = count;
    }
    public void CountItem(bool countBack = false) => ItemAmount += countBack ? -1 : 1;
    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(atlas.Texture, Rectangle, atlas.Rectangles[frame], Color.White);
        if (Item != null)
        {
            spriteBatch.Draw(Item.Texture, itemRectangle, Color.White);
            if (ItemAmount > 1)
                Text.Draw(ItemAmount.ToString(), itemCountPosition, frame == defaultFrame ? Color.White : Color.Black,
                spriteBatch, TextDrawingMode.Left);
        }
    }
}