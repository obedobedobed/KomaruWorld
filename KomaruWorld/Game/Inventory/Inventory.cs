using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class Inventory
{
    private Vector2 itemNamePos;
    public Slot[] HotbarSlots { get; private set; } = new Slot[5];
    public Slot[] Slots { get; private set; } = new Slot[15];
    public DeleteSlot DeleteSlot { get; private set; }

    public Inventory(Atlas slotAtlas, Vector2 hotbarSlotsPos, Vector2 slotsPos, int slotsLines)
    {
        {
            float xAdder = 0f;
            for (int i = 0; i < HotbarSlots.Length; i++)
            {
                HotbarSlots[i] = new Slot
                (
                    slotAtlas, new Vector2(hotbarSlotsPos.X + xAdder, hotbarSlotsPos.Y), SlotSize,
                    defaultFrame: 0, choosedFrame: 1, ItemSize, slotId: i
                );

                xAdder += SlotSize.X + UI_SPACING;
            }

            itemNamePos.X = 0;
            itemNamePos.Y = HotbarSlots[0].Position.Y - GlyphSize.Y - UI_SPACING;
        }

        {
            float yAdder = 0f;
            for (int i = 0; i < slotsLines; i++)
            {
                float xAdder = 0f;
                for (int j = 0; j < Slots.Length / slotsLines + (i == slotsLines - 1 ? 1 : 0); j++)
                {
                    var pos = new Vector2(slotsPos.X + xAdder, slotsPos.Y + yAdder);
                    int iteration = i * (Slots.Length / slotsLines) + j;

                    if (j != Slots.Length / slotsLines)
                        Slots[iteration] = new Slot(slotAtlas, pos, SlotSize, defaultFrame: 0, choosedFrame: 1, ItemSize,
                        slotId: iteration);
                    else
                        DeleteSlot = new DeleteSlot(slotAtlas, pos, SlotSize);

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

        var player = GameScene.Instance.Player;
        string slotItemName = HotbarSlots[player.HotbarSlot].Item?.Name;
        itemNamePos.X = HotbarSlots[player.HotbarSlot].Position.X + SlotSize.X / 2;
        if (slotItemName == null)
            return;
        Text.Draw(slotItemName, itemNamePos, Color.White, spriteBatch, TextDrawingMode.Center);
    }

    public bool CollectItem(Item item)
    {
        foreach (var slot in HotbarSlots)
        {
            if (slot.Item?.ID == item.ID && slot.ItemAmount < slot.Item?.MaxStack)
            {
                slot.CountItem();
                return true;
            }
            else if (slot.Item == null)
            {
                slot.UpdateItem(item);
                return true;
            }
        }

        foreach (var slot in Slots)
        {
            if (slot.Item?.ID == item.ID && slot.ItemAmount < slot.Item?.MaxStack)
            {
                slot.CountItem();
                return true;
            }
            else if (slot.Item == null)
            {
                slot.UpdateItem(item);
                return true;
            }
        }

        return false;
    }

    public void DrawInventory(SpriteBatch spriteBatch)
    {
        foreach (var slot in Slots)
            slot.Draw(spriteBatch);

        DeleteSlot.Draw(spriteBatch);
    }
}