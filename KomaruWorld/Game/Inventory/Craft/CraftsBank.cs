using System;
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
    public static CraftData Sword { get; private set; } = new CraftData(ItemsBank.Sword, itemAmount: 1, [ItemsBank.IronIngot], [3]);
    public static CraftData Pickaxe { get; private set; } = new CraftData(ItemsBank.Pickaxe, itemAmount: 1, [ItemsBank.IronIngot], [4]);
    public static CraftData Axe { get; private set; } = new CraftData(ItemsBank.Axe, itemAmount: 1, [ItemsBank.IronIngot], [4]);
    public static CraftData GoldSword { get; private set; } = new CraftData(ItemsBank.GoldSword, itemAmount: 1, [ItemsBank.GoldIngot], [3]);
    public static CraftData GoldPickaxe { get; private set; } = new CraftData(ItemsBank.GoldPickaxe, itemAmount: 1, [ItemsBank.GoldIngot], [4]);
    public static CraftData GoldAxe { get; private set; } = new CraftData(ItemsBank.GoldAxe, itemAmount: 1, [ItemsBank.GoldIngot], [4]);
    public static CraftData EmeraldSword { get; private set; } = new CraftData(ItemsBank.EmeraldSword, itemAmount: 1, [ItemsBank.Emerald], [3]);
    public static CraftData EmeraldPickaxe { get; private set; } = new CraftData(ItemsBank.EmeraldPickaxe, itemAmount: 1, [ItemsBank.Emerald], [4]);
    public static CraftData EmeraldAxe { get; private set; } = new CraftData(ItemsBank.EmeraldAxe, itemAmount: 1, [ItemsBank.Emerald], [4]);
    public static CraftData AmethystSword { get; private set; } = new CraftData(ItemsBank.AmethystSword, itemAmount: 1, [ItemsBank.Amethyst], [3]);
    public static CraftData AmethystPickaxe { get; private set; } = new CraftData(ItemsBank.AmethystPickaxe, itemAmount: 1, [ItemsBank.Amethyst], [4]);
    public static CraftData AmethystAxe { get; private set; } = new CraftData(ItemsBank.AmethystAxe, itemAmount: 1, [ItemsBank.Amethyst], [4]);
    public static CraftData IronHelmet { get; private set; } = new CraftData(ItemsBank.IronHelmet, itemAmount: 1, [ItemsBank.IronIngot], [3]);
    public static CraftData IronChestplate { get; private set; } = new CraftData(ItemsBank.IronChestplate, itemAmount: 1, [ItemsBank.IronIngot], [8]);
    public static CraftData IronLeggins { get; private set; } = new CraftData(ItemsBank.IronLeggins, itemAmount: 1, [ItemsBank.IronIngot], [5]);

    public static CraftData[] Crafts =
    [
        Log, Leaves, IronIngot, GoldIngot, Emerald,
        Amethyst, Sword, Pickaxe, Axe, GoldSword,
        GoldPickaxe, GoldAxe, EmeraldSword, EmeraldPickaxe, EmeraldAxe,
        AmethystSword, AmethystPickaxe, AmethystAxe, IronHelmet, IronChestplate,
        IronLeggins
    ];

    public static List<CraftSlot> CraftSlots = new List<CraftSlot>();

    private static int maximalPage;
    public static int Page { get; private set; } = 0;
    private static SpriteButton pagePlusButton;
    private static SpriteButton pageMinusButton;

    public static void CreateCraftSlots(Atlas atlas, Atlas arrowsAtlas, Vector2 position, CraftSlot.OpenCraftMenu openCraftMenu)
    {
        bool stopGenerating = false;
        int page = 0;

        float yAdder = 0;
        for (int l = 0; l < (int)MathF.Ceiling(Crafts.Length / (float)INV_SLOTS_IN_LINE); l++)
        {
            float xAdder = 0;
            for (int s = 0; s < INV_SLOTS_IN_LINE; s++)
            {
                int iteration = l * INV_SLOTS_IN_LINE + s;
                CraftSlots.Add(new CraftSlot(atlas, new Vector2(position.X + xAdder, position.Y + yAdder), SlotSize,
                Crafts[iteration], openCraftMenu, page));

                if (l == 1 && s == 0)
                {
                    pageMinusButton = new SpriteButton(arrowsAtlas, new Vector2
                    (position.X + xAdder - (SlotSize.X + UI_SPACING), position.Y + yAdder), SlotSize, 0, 1, PageMinus);
                    pageMinusButton.frameAdder = 2;
                }
                else if (l == 1 && s == INV_SLOTS_IN_LINE - 1)
                {
                    pagePlusButton = new SpriteButton(arrowsAtlas, new Vector2
                    (position.X + xAdder + (SlotSize.X + UI_SPACING), position.Y + yAdder), SlotSize, 0, 1, PagePlus);
                    pagePlusButton.frameAdder = 0;
                }

                xAdder += SlotSize.X + UI_SPACING;


                if (iteration + 1 == Crafts.Length)
                {
                    stopGenerating = true;
                    break;
                }
            }

            yAdder += SlotSize.Y + UI_SPACING;

            if ((l + 1) % INV_SLOTS_LINES == 0 && l != 0)
            {
                page++;
                yAdder = 0;
            }

            if (stopGenerating)
            {
                maximalPage = page;
                break;
            }
        }
    }

    public static void PagePlus()
    {
        Page++;
        if (Page > maximalPage)
            Page = 0;
    }
    public static void PageMinus()
    {
        Page--;
        if (Page < 0)
            Page = maximalPage;
    }

    public static void UpdateCraftSlots()
    {
        if (!GameScene.Instance.Crafting)
        {
            foreach (var slot in CraftSlots)
                if (slot.page == Page)
                    slot.Update(new GameTime());
            pagePlusButton.Update(new GameTime());
            pageMinusButton.Update(new GameTime());
        }
        else
            foreach (var slot in CraftSlots)
                slot.SetDefaultFrame();
    }

    public static void DrawCraftSlots(SpriteBatch spriteBatch)
    {
        foreach (var slot in CraftSlots)
            if (slot.page == Page)
                slot.Draw(spriteBatch);

        pagePlusButton.Draw(spriteBatch);
        pageMinusButton.Draw(spriteBatch);
    }
}