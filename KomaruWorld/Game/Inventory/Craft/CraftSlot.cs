using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class CraftSlot : GameObject
{
    public delegate void OpenCraftMenu(CraftData craftData);
    private OpenCraftMenu openCraftMenu;
    private MouseState lastMouse;
    public CraftData CraftData { get; private set; }
    public int page { get; private set; }

    private int defaultFrame = 0;
    private int choosedFrame = 1;
    private bool pressed = false;

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

    private Vector2 itemAmountPosition => new Vector2(Position.X + Size.X, Position.Y + Size.Y - GlyphSize.Y);

    public CraftSlot(Atlas atlas, Vector2 position, Vector2 size, CraftData craftData, OpenCraftMenu openCraftMenu, int page)
    : base(atlas, position, size, 0)
    {
        this.openCraftMenu = openCraftMenu;
        CraftData = craftData;
        this.page = page;
    }

    public override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        var normalizedCursorPos = mouse.NormalizeForWindow();
        var cursorRectangle = new Rectangle(normalizedCursorPos.X,normalizedCursorPos.Y, 1, 1);

        if (cursorRectangle.Intersects(Rectangle))
        {
            pressed = true;
            frame = choosedFrame;
        }
        else
        {
            pressed = false;
            frame = defaultFrame;
        }

        if (pressed && mouse.LeftButton == ButtonState.Pressed && lastMouse.LeftButton == ButtonState.Released)
            openCraftMenu(CraftData);

        lastMouse = mouse;
    }
    
    public void SetDefaultFrame() => frame = defaultFrame;

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.Draw(CraftData.Item.Texture, itemRectangle, Color.White);
        Text.Draw($"x{CraftData.ItemAmount}", itemAmountPosition, Color.White, spriteBatch, TextDrawingMode.Left);
    }
}