using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class SignTile : Tile
{
    public string SignText { get; private set; } = "hello world";
    public bool Writing { get; set; } = false;
    public static bool BlockedInput { get; private set; } = false;
    private KeyboardState lastKeyboard;

    public SignTile(Texture2D texture, Vector2 position, Vector2 size, bool canCollide, Tiles tileType,
    ToolToDestroy toolToDestroy, float destroyTime, int minimalToolPower, DropData drop)
    : base(texture, position, size, canCollide, tileType, toolToDestroy, destroyTime, minimalToolPower, drop)
    {
        
    }

    public override void Update(GameTime gameTime)
    {
        if (Writing)
        {
            var keyboard = Keyboard.GetState();
            BlockedInput = true;

            foreach (var key in keyboard.GetPressedKeys())
                if (!lastKeyboard.IsKeyDown(key))
                    Write(key);

            lastKeyboard = keyboard;
        }

        base.Update(gameTime);
    }

    private void Write(Keys key)
    {
        var keyboard = Keyboard.GetState();

        if (key == Keys.Back && SignText.Length > 0)
        {
            string tmpText = SignText;
            SignText = string.Empty;

            for (int i = 0; i < tmpText.Length; i++)
            {
                if (i == tmpText.Length - 1)
                    break;
                SignText += tmpText[i];
            }
        }
        else if (key == Keys.Space)
            SignText += " ";
        else if (key >= Keys.A && key <= Keys.Z)
        {
            bool upper = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

            char _char = (char)('a' + key - Keys.A);
            if (upper)
                _char = char.ToUpper(_char);

            SignText += _char;
        }
        else if (key >= Keys.D0 && key <= Keys.D9)
        {
            char _char = (char)('0' + key - Keys.D0);
            SignText += _char;
        }
        else if (key == Keys.Enter || key == Keys.Escape)
        {
            BlockedInput = false;
            Writing = false;
        }
    }

    public void DrawInputMenu(SpriteBatch spriteBatch, Texture2D pixel)
    {
        spriteBatch.Draw(pixel, new Rectangle(0, 0, VIRTUAL_WIDTH,
        VIRTUAL_HEIGHT), new Color(0, 0, 0, 150));

        Text.Draw(SignText, new Vector2(VIRTUAL_WIDTH / 2, VIRTUAL_HEIGHT / 2), Color.White,
        spriteBatch, TextDrawingMode.Center);

        Text.Draw("Press Enter/Escape for end editing", new Vector2(VIRTUAL_WIDTH / 2,
        VIRTUAL_HEIGHT - GlyphSize.X - UI_SPACING), Color.White, spriteBatch, TextDrawingMode.Center);
    }
}