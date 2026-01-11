using System.Collections.Generic;
using Microsoft.Xna.Framework;
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

    public static void CreateCraftSlots(Atlas atlas, Vector2 position, int lines, int slotsInLine, CraftSlot.OpenCraftMenu openCraftMenu)
    {
        bool stopGenerating = false;

        float yAdder = 0;
        for (int l = 0; l < lines; l++)
        {
            float xAdder = 0;
            for (int s = 0; s < slotsInLine; s++)
            {
                int iteration = l * slotsInLine + s;
                CraftSlots.Add(new CraftSlot(atlas, position, SlotSize, Crafts[iteration], openCraftMenu));
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
}