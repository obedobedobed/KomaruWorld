using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class Inventory
{
    public Slot[] HotbarSlots { get; private set; } = new Slot[5];
    public Slot[] Slots { get; private set; } = new Slot[15];

    public Inventory(Atlas slotAtlas, Vector2 hotbarSlotsPos, Vector2 slotsPos, int slotsLines)
    {
        {
            float xAdder = 0f;
            for (int i = 0; i < HotbarSlots.Length; i++)
            {
                HotbarSlots[i] = new Slot
                (
                    slotAtlas, new Vector2(hotbarSlotsPos.X + xAdder, hotbarSlotsPos.Y), SlotSize,
                    defaultFrame: 0, choosedFrame: 1, ItemSize
                );

                xAdder += SlotSize.X + UI_SPACING;
            }
        }

        {
            float yAdder = 0f;
            for (int i = 0; i < slotsLines; i++)
            {
                float xAdder = 0f;
                for (int j = 0; j < Slots.Length / slotsLines; j++)
                {
                    Slots[i * (Slots.Length / slotsLines) + j] = new Slot
                    (
                        slotAtlas, new Vector2(slotsPos.X + xAdder, slotsPos.Y + yAdder), SlotSize,
                        defaultFrame: 0, choosedFrame: 1, ItemSize
                    );

                    xAdder += SlotSize.X + UI_SPACING;
                }

                yAdder += SlotSize.Y + UI_SPACING;
            }
        }
    }

    public void DrawHotbar(SpriteBatch spriteBatch)
    {
        foreach (var slot in HotbarSlots)
            slot.Draw(spriteBatch);
    }

    public void DrawInventory(SpriteBatch spriteBatch)
    {
        foreach (var slot in Slots)
            slot.Draw(spriteBatch);
    }
}