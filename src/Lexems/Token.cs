namespace Lexems;

using System.Globalization;
using System.Text;

public class Token
{
    public Token(TokenType type)
    {
        Type = type;
    }

    public Token(TokenType type, TokenValue value)
    {
        Type = type;
        Value = value;
    }

    public Token(TokenType type, string value)
    {
        if (type != TokenType.StringLiteral &&
            type != TokenType.Identifier)
        {
            throw new ArgumentException("Expected token type StringLiternal or Identifier, not " + type.ToString());
        }

        Type = type;
        Value = new TokenValue(value);
    }

    public Token(TokenType type, long value)
    {
        if (type != TokenType.IntegerLiteral)
        {
            throw new ArgumentException("Expected token type IntegeralLiteral, not " + type.ToString());
        }

        Type = type;
        Value = new TokenValue(value);
    }

    public Token(TokenType type, decimal value)
    {
        if (type != TokenType.FloatLiteral)
        {
            throw new ArgumentException("Expected token type FloatLiteral, not " + type.ToString());
        }

        Type = type;
        Value = new TokenValue(value);
    }

    public TokenType Type { get; }

    public TokenValue? Value { get; } = null;

    /// <summary>
    ///  Сравнивает токены по типу и значению.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is Token other)
        {
            return Type == other.Type && Equals(Value, other.Value);
        }

        return false;
    }

    /// <summary>
    ///  Возвращает хеш от свойств токена.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine((int)Type, Value);
    }

    /// <summary>
    /// Форматирует токен в стиле "Type (Value)".
    /// </summary>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(Type.ToString());

        if (Value != null)
        {
            sb.Append($" ({Value})");
        }

        return sb.ToString();
    }
}