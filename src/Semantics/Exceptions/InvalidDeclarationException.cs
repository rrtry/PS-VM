namespace Semantics.Exceptions;

#pragma warning disable RCS1194

/// <summary>
/// Исключение из-за некорректного обращения к переменной.
/// </summary>
public class InvalidDeclarationException : Exception
{
    public InvalidDeclarationException(string message)
        : base(message)
    {
    }
}
#pragma warning restore RCS1194