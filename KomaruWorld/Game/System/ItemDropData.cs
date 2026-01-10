namespace KomaruWorld;

public struct ItemDropData(Item item, int amout)
{
    public Item Item { get; private set; } = item;
    public int Amount { get; private set; } = amout;
}