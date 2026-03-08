namespace Runtime;

/// <summary>
/// Специальный тип, обозначающий отсутствие значения.
/// </summary>
public record struct UnitValue
{
    public static readonly UnitValue Value = default;

    public override string ToString()
    {
        return "unit";
    }
}