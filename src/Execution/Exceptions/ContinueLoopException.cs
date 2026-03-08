namespace Execution.Exceptions;

#pragma warning disable RCS1194

/// <summary>
/// Внутреннее исключение библиотеки, используется для пропуска итерации цикла.
/// </summary>
internal class ContinueLoopException : Exception
{
    public ContinueLoopException()
        : base("Loop continue")
    {
    }
}
#pragma warning restore RCS1194