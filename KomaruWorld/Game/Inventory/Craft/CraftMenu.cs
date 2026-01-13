using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class CraftMenu : GameObject
{
    private TextButton craftButton;
    public CraftData CraftData { get; private set; }
    private Rectangle itemRectangle
    {
        get
        {
            return new Rectangle
            (
                (int)(CraftMenuItemSlotPos.X + Position.X + SlotSize.X / 2 - ItemSize.X / 2),
                (int)(CraftMenuItemSlotPos.Y + Position.Y + SlotSize.Y / 2 - ItemSize.Y / 2),
                (int)ItemSize.X, (int)ItemSize.Y
            );
        }
    }
    public CraftMenu(Texture2D texture, Vector2 position, Vector2 size, TextButton.Action craft) : base(texture, position, size)
    {
        var craftButtonPos = new Vector2
        (
            position.X + CraftMenuSize.X / 2,
            position.Y + CraftMenuSize.Y - GlyphSize.Y - UI_SPACING * 4
        );
        craftButton = new TextButton("Craft", craftButtonPos, Color.White, Color.Yellow, craft);
    }

    public void SetCraftData(CraftData craftData) => CraftData = craftData;

    public override void Update(GameTime gameTime)
    {
        craftButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        craftButton.Draw(spriteBatch);
        spriteBatch.Draw(CraftData.Item.Texture, itemRectangle, Color.White);
    }
}