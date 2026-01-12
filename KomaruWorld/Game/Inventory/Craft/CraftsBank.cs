using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class CraftsBanks
{
    public static CraftData Log { get; private set; } = new CraftData(ItemsBank.Log, itemAmount: 1, [ItemsBank.Planks], [4]);
    public static CraftData Leaves { get; private set; } = new CraftData(ItemsBank.Leaves, itemAmount: 1, [ItemsBank.Leaf, ItemsBank.Stick], [4, 1]);

    public static CraftData[] Crafts =
    [
        Log, Leaves
    ];

    public static List<CraftSlot> CraftSlots = new List<CraftSlot>();

    public static void CreateCraftSlots(Atlas atlas, Vector2 position, CraftSlot.OpenCraftMenu openCraftMenu)
    {
        bool stopGenerating = false;

        float yAdder = 0;
        for (int l = 0; l < INV_SLOTS_LINES; l++)
        {
            float xAdder = 0;
            for (int s = 0; s < INV_SLOTS_IN_LINE; s++)
            {
                int iteration = l * INV_SLOTS_IN_LINE + s;
                CraftSlots.Add(new CraftSlot(atlas, new Vector2(position.X + xAdder, position.Y + yAdder), SlotSize,
                Crafts[iteration], openCraftMenu));
                xAdder += SlotSize.X + UI_SPACING;

                if (iteration + 1 == Crafts.Length)
                {
                    stopGenerating = true;
                    break;
                }
            }
            yAdder += SlotSize.Y + UI_SPACING;
            if (stopGenerating)
                break;
        }
    }

    public static void DrawCraftSlots(SpriteBatch spriteBatch)
    {
        foreach (var slot in CraftSlots)
            slot.Draw(spriteBatch);
    }
}