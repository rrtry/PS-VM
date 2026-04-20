using System.Runtime.CompilerServices;

namespace Runtime;

public class ValueType
{
    /// <summary>
    /// Значение отсутствует.
    /// </summary>
    public static readonly ValueType Unit = new("unit");

    /// <summary>
    /// Целочисленное значение.
    /// </summary>
    public static readonly ValueType Int = new("int");

    /// <summary>
    /// Вещественное число.
    /// </summary>
    public static readonly ValueType Float = new ("float");

    /// <summary>
    /// Строковое значение.
    /// </summary>
    public static readonly ValueType Str = new("str");

    /// <summary>
    /// Булевое значение.
    /// </summary>
    public static readonly ValueType Bool = new("bool");

    private readonly string name;

    protected ValueType(string name)
    {
        this.name = name;
    }

    public static bool operator ==(ValueType a, ValueType b) => a.Equals(b);

    public static bool operator !=(ValueType a, ValueType b) => !a.Equals(b);

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj);
    }

    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);
    }

    public override string ToString()
    {
        return name;
    }
}