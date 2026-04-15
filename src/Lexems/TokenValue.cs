using System.Globalization;

namespace Lexems;

public class TokenValue
{
    private readonly object val;

    public TokenValue(string value)
    {
        val = value;
    }

    public TokenValue(decimal value)
    {
        val = value;
    }

    public TokenValue(long value)
    {
        val = value;
    }

    /// <summary>
    ///  Возвращает значение токена в виде строки.
    /// </summary>
    /// <remarks>
    ///  Имя метода пересекается со стандартным методом ToString(), поэтому добавлен `override`.
    /// </remarks>
    public override string ToString()
    {
        return val switch
        {
            string s => s,
            long l => l.ToString(CultureInfo.InvariantCulture),
            decimal d => d.ToString(CultureInfo.InvariantCulture),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    ///  Возвращает значение токена в виде числа.
    /// </summary>
    public double ToDouble()
    {
        return val switch
        {
            string s => double.Parse(s, CultureInfo.InvariantCulture),
            long l => (double)l,
            decimal d => decimal.ToDouble(d),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    ///  Проверяет равенство значений токенов. Значения разных типов всегда считаются разными.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is TokenValue other)
        {
            return val switch
            {
                string s => (string)other.val == s,
                decimal d => (decimal)other.val == d,
                long l => (long)other.val == l,
                _ => throw new NotImplementedException(),
            };
        }

        return false;
    }

    public override int GetHashCode()
    {
        return val.GetHashCode();
    }
}