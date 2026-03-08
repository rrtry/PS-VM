namespace Execution.Exceptions;

#pragma warning disable RCS1194

/// <summary>
/// Внутреннее исключение библиотеки, используется для выхода из функции.
/// </summary>
internal class ReturnException : Exception
{
    public ReturnException()
        : base("Return statement")
    {
    }
}
#pragma warning restore RCS1194