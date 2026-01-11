namespace KomaruWorld;

public static class CraftsBanks
{
    public static CraftData Log { get; private set; } = new CraftData(ItemsBank.Log, [ItemsBank.Planks], [4]);
    public static CraftData Leaves { get; private set; } = new CraftData(ItemsBank.Leaves, [ItemsBank.Leaf, ItemsBank.Stick], [4, 1]);
}