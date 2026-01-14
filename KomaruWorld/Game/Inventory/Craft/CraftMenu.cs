using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class CraftMenu : GameObject
{
    private TextButton craftButton;
    public CraftData CraftData { get; private set; }
    private CraftMenuMaterial[] materials;

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

    private Vector2 itemNamePos
    {
        get
        {
            return new Vector2
            (
                (int)(CraftMenuItemSlotPos.X + Position.X + SlotSize.X / 2),
                (int)(CraftMenuItemSlotPos.Y + Position.Y + SlotSize.Y + UI_SPACING * 2)
            );
        }
    }

    private Vector2 fromTextPos
    {
        get
        {
            return new Vector2
            (
                (int)(CraftMenuItemSlotPos.X + Position.X + SlotSize.X / 2),
                (int)(CraftMenuItemSlotPos.Y + Position.Y + SlotSize.Y + UI_SPACING * 2 + GlyphSize.Y * 2)
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

    public void SetCraftData(CraftData craftData)
    {
        CraftData = craftData;
        materials = new CraftMenuMaterial[CraftData.Materials.Count];

        float materialsWidth = ItemSize.X * materials.Length + UI_SPACING * (materials.Length - 1);
        var position = new Vector2
        (
            x: Position.X + (CraftMenuSize.X - materialsWidth) / 2,
            y: fromTextPos.Y + GlyphSize.Y * 2
        );

        var items = CraftData.Materials.Keys.ToArray();
        var amounts = CraftData.Materials.Values.ToArray();

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = new CraftMenuMaterial(items[i].Texture, position, ItemSize, amounts[i]);
            position.X += ItemSize.X + UI_SPACING;
        }
    }

    public override void Update(GameTime gameTime)
    {
        craftButton.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        craftButton.Draw(spriteBatch);
        spriteBatch.Draw(CraftData.Item.Texture, itemRectangle, Color.White);
        Text.Draw($"{CraftData.Item.Name} (x{CraftData.ItemAmount})", itemNamePos, Color.White,
        spriteBatch, TextDrawingMode.Center);
        Text.Draw("From:", fromTextPos, Color.White,
        spriteBatch, TextDrawingMode.Center);
        foreach (var material in materials)
            material.Draw(spriteBatch);
    }
}