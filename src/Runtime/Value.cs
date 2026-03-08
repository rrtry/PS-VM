namespace Runtime;

using System.Globalization;

/// <summary>
/// Абстракция над типами, которые доступны в языке.
/// int    - целое число (int64_t).
/// float  - вещественное число (double).
/// str    - UTF-8 строка.
/// unit   - отсуствие возвращаемого типа у функции.
/// </summary>
public class Value : IEquatable<Value>
{
    public const double Tolerance = 0.001d;
    public static readonly Value Unit = new(UnitValue.Value);
    private readonly object value;

    /// <summary>
    /// Создаёт строковое значение.
    /// </summary>
    public Value(string v)
    {
        value = v;
    }

    /// <summary>
    /// Создаёт целочисленное значение.
    /// </summary>
    public Value(long v)
    {
        value = v;
    }

    /// <summary>
    /// Создаёт вещественное значение.
    /// </summary>
    public Value(double v)
    {
        value = v;
    }

    /// <summary>
    /// Создаёт значение неуточнённого типа.
    /// </summary>
    private Value(object v)
    {
        value = v;
    }

    public bool IsDouble()
    {
        return value switch
        {
            double => true,
            _ => false,
        };
    }

    public double AsDouble()
    {
        return value switch
        {
            double d => d,
            long l => l,
            _ => throw new InvalidOperationException($"Value {value} is not a double"),
        };
    }

    /// <summary>
    /// Определяет, является ли значение строкой.
    /// </summary>
    public bool IsString()
    {
        return value switch
        {
            string => true,
            _ => false,
        };
    }

    /// <summary>
    /// Возвращает значение как строку либо бросает исключение.
    /// </summary>
    public string AsString()
    {
        return value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Value {value} is not a string"),
        };
    }

    /// <summary>
    /// Определяет, является ли значение целым числом.
    /// </summary>
    public bool IsLong()
    {
        return value switch
        {
            long => true,
            _ => false,
        };
    }

    /// <summary>
    /// Возвращает значение как целое число либо бросает исключение.
    /// </summary>
    public long AsLong()
    {
        return value switch
        {
            long i => i,
            _ => throw new InvalidOperationException($"Value {value} is not numeric"),
        };
    }

    /// <summary>
    /// Печатает значение для отладки.
    /// </summary>
    public override string ToString()
    {
        return value switch
        {
            string s => ValueUtil.EscapeStringValue(s),
            long i => i.ToString(CultureInfo.InvariantCulture),
            double d => d.ToString(CultureInfo.InvariantCulture),
            UnitValue v => v.ToString(),
            _ => throw new InvalidOperationException($"Unexpected value {value} of type {value.GetType()}"),
        };
    }

    /// <summary>
    /// Сравнивает на равенство два значения.
    /// </summary>
    public bool Equals(Value? other)
    {
        if (other is null)
        {
            return false;
        }

        return value switch
        {
            string s => other.AsString() == s,
            long i => other.IsDouble() ? Math.Abs(other.AsDouble() - i) < Tolerance : other.AsLong() == i,
            double d => Math.Abs(other.AsDouble() - d) < Tolerance,
            UnitValue => true,
            _ => throw new NotImplementedException(),
        };
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Value);
    }

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }
}