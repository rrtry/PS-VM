namespace Runtime;

using System.Globalization;

/// <summary>
/// Абстракция над типами, которые доступны в языке.
/// int    - целое число (int64_t).
/// float  - вещественное число (double).
/// str    - Unicode строка (UTF-16)
/// unit   - отсуствие возвращаемого типа у функции.
/// </summary>
public class Value
{
    public static readonly Value Unit = new(UnitValue.Value);
    private readonly object _value;

    /// <summary>
    /// Создаёт строковое значение.
    /// </summary>
    public Value(string v)
    {
        _value = v;
    }

    /// <summary>
    /// Создаёт целочисленное значение.
    /// </summary>
    public Value(long v)
    {
        _value = v;
    }

    /// <summary>
    /// Создаёт вещественное значение.
    /// </summary>
    public Value(double v)
    {
        _value = v;
    }

    /// <summary>
    /// Создаёт значение неуточнённого типа.
    /// </summary>
    private Value(object v)
    {
        _value = v;
    }

    public bool IsDouble()
    {
        return _value switch
        {
            double => true,
            _ => false,
        };
    }

    public double AsDouble()
    {
        return _value switch
        {
            double d => d,
            long l => l,
            _ => throw new InvalidOperationException($"Value {_value} is not a double"),
        };
    }

    /// <summary>
    /// Определяет, является ли значение строкой.
    /// </summary>
    public bool IsString()
    {
        return _value switch
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
        return _value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Value {_value} is not a string"),
        };
    }

    /// <summary>
    /// Определяет, является ли значение целым числом.
    /// </summary>
    public bool IsLong()
    {
        return _value switch
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
        return _value switch
        {
            long i => i,
            _ => throw new InvalidOperationException($"Value {_value} is not numeric, {_value.GetType()}"),
        };
    }

    /// <summary>
    /// Определяет, является ли значение void-подобным типом.
    /// </summary>
    public bool IsUnit()
    {
        return _value switch
        {
            UnitValue v => true,
            _ => false,
        };
    }

    /// <summary>
    /// Сравнивает два значения, возвращая истину, если текущее значение меньше переданного.
    /// </summary>
    public bool LessThan(Value other)
    {
        return _value switch
        {
            long i => i < other.AsLong(),
            double d => d < other.AsDouble(),
            _ => throw new InvalidOperationException($"Cannot compare value {this} with {other}"),
        };
    }

    /// <summary>
    /// Сравнивает два значения, возвращая истину, если текущее значение меньше переданного.
    /// </summary>
    public bool LessThanOrEqual(Value other)
    {
        return _value switch
        {
            long i => i <= other.AsLong(),
            double d => d <= other.AsDouble(),
            _ => throw new InvalidOperationException($"Cannot compare value {this} with {other}"),
        };
    }

    /// <summary>
    /// Печатает значение для отладки.
    /// </summary>
    public override string ToString()
    {
        return _value switch
        {
            string s => s,
            long i => i.ToString(CultureInfo.InvariantCulture),
            double d => d.ToString(CultureInfo.InvariantCulture),
            UnitValue v => v.ToString(),
            _ => throw new InvalidOperationException($"Unexpected value {_value} of type {_value.GetType()}"),
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

        return _value switch
        {
            string s => other.AsString() == s,
            long i => other.AsLong() == i,
            double d => other.AsDouble() == d,
            UnitValue => true,
            _ => throw new NotImplementedException(),
        };
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }
}