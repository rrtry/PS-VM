namespace Semantics.Exceptions;

using ValueType = Runtime.ValueType;

#pragma warning disable RCS1194

/// <summary>
/// Исключение из-за несовместимых типов данных в программе.
/// </summary>
public class TypeErrorException : Exception
{
    public TypeErrorException(string message)
        : base(message)
    {
    }

    public TypeErrorException(string category, ValueType expected, ValueType actual)
        : base($"Type mismatch: {category} must be of type {expected}, got {actual}")
    {
    }
}
#pragma warning restore RCS1194