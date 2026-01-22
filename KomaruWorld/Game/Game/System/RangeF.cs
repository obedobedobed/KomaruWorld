namespace KomaruWorld;

public struct RangeF(float minimal, float maximal)
{
    public float MinimalValue { get; private set; } = minimal;
    public float MaximalValue { get; private set; } = maximal;
}