namespace Execution.Exceptions;

#pragma warning disable RCS1194

/// <summary>
/// Внутреннее исключение библиотеки, используется для выхода из текущего цикла.
/// </summary>
internal class BreakLoopException : Exception
{
    public BreakLoopException()
        : base("Loop break")
    {
    }
}
#pragma warning restore RCS1194