using System;
using System.Collections.Generic;

namespace KomaruWorld;

public class DropData
{
    public Dictionary<Item, int> DropChance { get; private set; }
    public Dictionary<Item, Range> DropAmount { get; private set; }

    public DropData(Item[] items, int[] chances, Range[] amounts)
    {
        DropChance = new Dictionary<Item, int>();
        DropAmount = new Dictionary<Item, Range>();

        for (int i = 0; i < items.Length; i++)
            DropChance.Add(items[i], chances[i]);

        for (int i = 0; i < items.Length; i++)
            DropAmount.Add(items[i], amounts[i]);
    }

    public ItemDropData[] CalculateDrop()
    {
        var drop = new List<ItemDropData>();
        Item tmpItem;
        int tmpAmout;

        foreach (var item in DropChance.Keys)
        {
            if (Random.Shared.Next(0, 101) <= DropChance.GetValueOrDefault(item))
            {
                tmpItem = item;
                Range dropAmountRange = DropAmount.GetValueOrDefault(item);
                tmpAmout = Random.Shared.Next(dropAmountRange.MinimalValue, dropAmountRange.MaximalValue + 1);

                drop.Add(new ItemDropData(tmpItem, tmpAmout));
            }
        }

        return drop.ToArray();
    }
}