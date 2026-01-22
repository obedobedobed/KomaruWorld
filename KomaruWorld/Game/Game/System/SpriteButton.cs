using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KomaruWorld;

public class SpriteButton : GameObject
{
    public delegate void Action();
    private Action action;

    private int defaultFrame;
    private int choosedFrame;
    public int frameAdder { get; set; } = 0;

    private bool pressed = false;

    private MouseState lastMouse;

    public SpriteButton(Atlas atlas, Vector2 position, Vector2 size, int defaultFrame, int choosedFrame, Action action)
    : base(atlas, position, size, defaultFrame)
    {
        this.defaultFrame = defaultFrame;
        this.choosedFrame = choosedFrame;
        this.action = action;
    }

    public override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        var normalizedCursorPos = mouse.NormalizeForWindow();
        var cursorRectangle = new Rectangle(normalizedCursorPos.X, normalizedCursorPos.Y, 1, 1);

        if (cursorRectangle.Intersects(Rectangle))
        {
            pressed = true;
            Frame = choosedFrame + frameAdder;
        }
        else
        {
            pressed = false;
            Frame = defaultFrame + frameAdder;
        }

        if (pressed && mouse.LeftButton == ButtonState.Pressed && lastMouse.LeftButton == ButtonState.Released)
            action();

        lastMouse = mouse;
    }
}