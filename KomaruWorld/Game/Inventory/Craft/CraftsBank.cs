using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static KomaruWorld.GameParameters;

namespace KomaruWorld;

public static class CraftsBank
{
    public static CraftData Log { get; private set; } = new CraftData(ItemsBank.Log, itemAmount: 1, [ItemsBank.Planks], [4]);
    public static CraftData Leaves { get; private set; } = new CraftData(ItemsBank.Leaves, itemAmount: 1, [ItemsBank.Leaf], [4]);
    public static CraftData IronIngot { get; private set; } = new CraftData(ItemsBank.IronIngot, itemAmount: 1, [ItemsBank.IronOre], [1]);
    public static CraftData GoldIngot { get; private set; } = new CraftData(ItemsBank.GoldIngot, itemAmount: 1, [ItemsBank.GoldOre], [1]);
    public static CraftData Emerald { get; private set; } = new CraftData(ItemsBank.Emerald, itemAmount: 1, [ItemsBank.EmeraldOre], [1]);
    public static CraftData Amethyst { get; private set; } = new CraftData(ItemsBank.Amethyst, itemAmount: 1, [ItemsBank.AmethystOre], [1]);
    public static CraftData IronHelmet { get; private set; } = new CraftData(ItemsBank.IronHelmet, itemAmount: 1, [ItemsBank.IronIngot], [3]);
    public static CraftData IronChestplate { get; private set; } = new CraftData(ItemsBank.IronChestplate, itemAmount: 1, [ItemsBank.IronIngot], [8]);
    public static CraftData IronLeggins { get; private set; } = new CraftData(ItemsBank.IronLeggins, itemAmount: 1, [ItemsBank.IronIngot], [5]);

    public static CraftData[] Crafts =
    [
        Log, Leaves, IronIngot, GoldIngot, Emerald,
        Amethyst, IronHelmet, IronChestplate, IronLeggins
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

    public static void UpdateCraftSlots()
    {
        if (!GameScene.Instance.Crafting)
            foreach (var slot in CraftSlots)
                slot.Update(new GameTime());
        else
            foreach (var slot in CraftSlots)
                slot.SetDefaultFrame();
    }

    public static void DrawCraftSlots(SpriteBatch spriteBatch)
    {
        foreach (var slot in CraftSlots)
            slot.Draw(spriteBatch);
    }
}