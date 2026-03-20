namespace Semantics.Exceptions;

#pragma warning disable RCS1194

/// <summary>
/// Исключение из-за некорректного обращения к переменной.
/// </summary>
public class InvalidStatementException : Exception
{
    public InvalidStatementException(string message)
        : base(message)
    {
    }
}
#pragma warning restore RCS1194