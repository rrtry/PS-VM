namespace Execution;

/// <summary>
/// Представляет окружение для выполнения программы.
/// Прежде всего это функции ввода/вывода.
/// </summary>
public interface IEnvironment
{
    /// <summary>
    /// Список результатов выражений.
    /// </summary>
    public List<string> GetEvaluated();

    /// <summary>
    /// Функция чтения строки из stdin.
    /// </summary>
    public string Input();

    /// <summary>
    /// Функция записи в строки stdout.
    /// </summary>
    public void Print(string result);
}