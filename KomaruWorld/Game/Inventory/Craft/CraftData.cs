using System.Collections.Generic;

namespace KomaruWorld;

public class CraftData
{
    public Item Item { get; private set; }
    public int ItemAmount { get; private set; }
    public Dictionary<int, int> Materials { get; private set; }

    public CraftData(Item item, int itemAmount, Item[] materials, int[] materialsAmount)
    {
        Item = item;
        ItemAmount = itemAmount;
        Materials = new Dictionary<int, int>();

        for (int i = 0; i < materials.Length; i++)
            Materials.Add(materials[i].ID, materialsAmount[i]);
    }
}