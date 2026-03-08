namespace Execution;

using Runtime;

/// <summary>
/// Контекст выполнения программы (все переменные, константы и другие символы).
/// </summary>
public class Context
{
    private readonly IEnvironment environment;
    private readonly Stack<Scope> scopes = [];

    public Context(IEnvironment environment)
    {
        scopes.Push(new Scope());
        this.environment = environment;
    }

    /// <summary>
    /// Добавляет новую область видимости.
    /// </summary>
    public void PushScope(Scope scope)
    {
        scopes.Push(scope);
    }

    /// <summary>
    /// Извлекает последнюю область видимости.
    /// </summary>
    public void PopScope()
    {
        scopes.Pop();
    }

    /// <summary>
    /// Возвращает значение переменной или константы.
    /// </summary>
    public Value GetValue(string name)
    {
        foreach (Scope s in scopes)
        {
            if (s.TryGetVariable(name, out Value? variable))
            {
                return variable!;
            }
        }

        throw new ArgumentException($"Variable '{name}' is not defined");
    }

    /// <summary>
    /// Присваивает (изменяет) значение переменной.
    /// </summary>
    public void AssignVariable(string name, Value value)
    {
        foreach (Scope s in scopes.Reverse())
        {
            if (s.TryAssignVariable(name, value))
            {
                return;
            }
        }

        throw new ArgumentException($"Variable '{name}' is not defined");
    }

    /// <summary>
    /// Определяет переменную в текущей области видимости.
    /// </summary>
    public void DefineVariable(string name, Value value)
    {
        if (!scopes.Peek().TryDefineVariable(name, value))
        {
            throw new ArgumentException($"Variable '{name}' is already defined in this scope");
        }
    }
}