namespace Semantics.Exceptions;

#pragma warning disable RCS1194

/// <summary>
/// Исключение из-за некорректного вызова функции.
/// </summary>
public class InvalidFunctionCallException : Exception
{
    public InvalidFunctionCallException(string message)
        : base(message)
    {
    }
}
#pragma warning restore RCS1194