namespace Semantics.Exceptions;

#pragma warning disable RCS1194

/// <summary>
/// Исключение, которое выбрасывается при нахождении недостижимого кода
/// </summary>
public class UnreachableCodeException : Exception
{
    public UnreachableCodeException(string message)
        : base(message)
    {
    }
}
#pragma warning restore RCS1194