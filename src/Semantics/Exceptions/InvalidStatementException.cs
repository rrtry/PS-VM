namespace Semantics.Exceptions;

#pragma warning disable RCS1194

/// <summary>
/// Исключение выбрасывается при некорректном использовании инструкции
/// </summary>
public class InvalidStatementException : Exception
{
    public InvalidStatementException(string message)
        : base(message)
    {
    }
}
#pragma warning restore RCS1194