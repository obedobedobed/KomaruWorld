namespace KomaruWorld;

public struct Range(int minimal, int maximal)
{
    public int MinimalValue { get; private set; } = minimal;
    public int MaximalValue { get; private set; } = maximal;
}