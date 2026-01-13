using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class TextButton : GameObject
{
    public delegate void Action();
    private Action action;

    private string text;
    private Color defaultColor;
    private Color choosedColor;
    private Color color;
    public int frameAdder { get; set; } = 0;

    private bool pressed = false;

    private static MouseState lastMouse;

    public TextButton(string text, Vector2 position, Color defaultColor, Color choosedColor, Action action)
    : base(null, position, Vector2.Zero)
    {
        color = defaultColor;
        Size = new Vector2(GlyphSize.X, Text.CalculateStringWidth(text));
        this.text = text;
        this.defaultColor = defaultColor;
        this.choosedColor = choosedColor;
        this.action = action;
    }

    public override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        var cursorRectangle = new Rectangle(mouse.X, mouse.Y, 1, 1);

        if (cursorRectangle.Intersects(Rectangle))
        {
            pressed = true;
            color = choosedColor;
        }
        else
        {
            pressed = false;
            color = defaultColor;
        }

        if (pressed && mouse.LeftButton == ButtonState.Pressed && lastMouse.LeftButton == ButtonState.Released)
            action();

        lastMouse = mouse;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Text.Draw(text, Position, color, spriteBatch, TextDrawingMode.Center);
    }
}