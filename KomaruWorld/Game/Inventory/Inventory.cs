using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public class Inventory
{
    public Slot[] HotbarSlots { get; private set; } = new Slot[5];
    public Slot[] Slots { get; private set; } = new Slot[15];
    public ArmorSlot[] ArmorSlots { get; private set; } = new ArmorSlot[3];
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
                    defaultFrame: 0, choosedFrame: 1, slotId: i
                );

                xAdder += SlotSize.X + UI_SPACING;
            }
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
                        Slots[iteration] = new Slot(slotAtlas, pos, SlotSize, defaultFrame: 0,
                        choosedFrame: 1, slotId: iteration);
                    else
                        DeleteSlot = new DeleteSlot(slotAtlas, pos, SlotSize);

                    xAdder += SlotSize.X + UI_SPACING;
                }

                yAdder += SlotSize.Y + UI_SPACING;
            }
        }

        {
            float yAdder = 0f;
            for (int i = 0; i < ArmorSlots.Length; i++)
            {
                ArmorElement element = i switch
                {
                    0 => ArmorElement.Helmet,  
                    1 => ArmorElement.Chestplate,  
                    _ => ArmorElement.Leggins,  
                };

                var pos = new Vector2(slotsPos.X - (SlotSize.X - UI_SPACING) * 2, slotsPos.Y + yAdder);
                ArmorSlots[i] = new ArmorSlot(slotAtlas, pos, SlotSize, frame: 3 + i, element);
                yAdder += SlotSize.X + UI_SPACING;
            }
        }
    }

    public void DrawHotbar(SpriteBatch spriteBatch)
    {
        foreach (var slot in HotbarSlots)
            slot.Draw(spriteBatch);
    }

    public bool TryCollectItem(Item item)
    {
        var sameItemSlots = SearchItemSlots(item);

        foreach (var slot in sameItemSlots)
            if (ItemCollecting_CheckSlot(slot, item))
                return true;

        foreach (var slot in HotbarSlots)
            if (ItemCollecting_CheckSlot(slot, item))
                return true;

        foreach (var slot in Slots)
            if (ItemCollecting_CheckSlot(slot, item))
                return true;

        return false;
    }

    private void CollectItem(Item item, Slot slot, bool count = false)
    {
        if (count)
            slot.CountItem();
        else
            slot.UpdateItem(item);
    }

    private bool ItemCollecting_CheckSlot(Slot slot, Item item)
    {
        if (slot.Item?.ID == item.ID && slot.ItemAmount < slot.Item?.MaxStack)
        {
            CollectItem(item, slot, true);
            return true;
        }
        else if (slot.Item == null)
        {
            CollectItem(item, slot, false);
            return true;
        }

        return false;
    }

    public Slot SearchItemSlot(Item item, int amount)
    {
        foreach (var slot in HotbarSlots)
            if (CheckSlot(slot, item, amount))
                return slot;

        foreach (var slot in Slots)
            if (CheckSlot(slot, item, amount))
                return slot;

        return null;
    }

    public Slot[] SearchItemSlots(Item item)
    {
        var founded = new List<Slot>();

        foreach (var slot in HotbarSlots)
            if (CheckSlot(slot, item))
                founded.Add(slot);

        foreach (var slot in Slots)
            if (CheckSlot(slot, item))
                founded.Add(slot);

        return founded.ToArray();
    }

    private bool CheckSlot(Slot slot, Item item, int amount)
    {
        return slot.Item?.ID == item.ID && slot.ItemAmount >= amount; 
    }

    private bool CheckSlot(Slot slot, Item item)
    {
        return slot.Item?.ID == item.ID;
    }

    public void DrawInventory(SpriteBatch spriteBatch)
    {
        foreach (var slot in Slots)
            slot.Draw(spriteBatch);

        foreach (var slot in ArmorSlots)
            slot.Draw(spriteBatch);

        var armorTextPos = new Vector2
        (
            ArmorSlots[0].Position.X + SlotSize.X / 2,
            ArmorSlots[ArmorSlots.Length - 1].Position.Y + SlotSize.Y + UI_SPACING
        );

        int armor = 0;
        foreach (var armorSlot in ArmorSlots)
            armor += armorSlot.Item?.Armor ?? 0;

        Text.Draw($"Armor: {armor}", armorTextPos, Color.White, spriteBatch, TextDrawingMode.Center);

        DeleteSlot.Draw(spriteBatch);
    }
}