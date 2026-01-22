using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class CraftMenuMaterial : GameObject
{
    private int amount;
    private Vector2 amountTextPos
    {
        get
        {
            return new Vector2
            (
                Position.X + Size.X / 2,
                Position.Y + Size.Y + TEXT_SPACING
            );
        }
    }

    public CraftMenuMaterial(Texture2D texture, Vector2 position, Vector2 size, int amount)
    : base(texture, position, size)
    {
        this.amount = amount;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        Text.Draw(amount.ToString(), amountTextPos, Color.White, spriteBatch, TextDrawingMode.Center);
    }
}