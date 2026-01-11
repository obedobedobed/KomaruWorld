using System.Collections.Generic;

namespace KomaruWorld;

public class CraftData
{
    public Item Item { get; private set; }
    public Dictionary<Item, int> Materials;

    public CraftData(Item item, Item[] materials, int[] materialsAmount)
    {
        Item = item;
        Materials = new Dictionary<Item, int>();

        for (int i = 0; i < materials.Length; i++)
            Materials.Add(materials[i], materialsAmount[i]);
    }
}