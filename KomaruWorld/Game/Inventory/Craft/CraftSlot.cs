using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KomaruWorld;

public class CraftSlot : GameObject
{
    public delegate void OpenCraftMenu(CraftData craftData);
    private OpenCraftMenu openCraftMenu;
    private static MouseState lastMouse;
    public CraftData CraftData;

    private int defaultFrame = 0;
    private int choosedFrame = 1;
    private bool pressed = false;

    public CraftSlot(Atlas atlas, Vector2 position, Vector2 size, CraftData craftData, OpenCraftMenu openCraftMenu)
    : base(atlas, position, size, 0)
    {
        this.openCraftMenu = openCraftMenu;
        CraftData = craftData;
    }

    public override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        var cursorRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

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
}