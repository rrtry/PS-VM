using Runtime;

namespace VirtualMachine;

public sealed class VariablesTable
{
    private readonly VariablesTable? _parent;
    private readonly Dictionary<string, Value> _variables;

    public VariablesTable(VariablesTable? parent = null)
    {
        _parent = parent;
        _variables = [];
    }

    public VariablesTable? Parent => _parent;

    /// <summary>
    /// Получает значение переменной по имени.
    /// </summary>
    public Value GetVariable(string name)
    {
        if (_variables.TryGetValue(name, out Value? value))
        {
            return value;
        }

        if (_parent != null)
        {
            return _parent.GetVariable(name);
        }

        throw new InvalidOperationException($"No variable with name {name}");
    }

    /// <summary>
    /// Объявляет переменную с указанным именем и начальным значением.
    /// </summary>
    public void DefineVariable(string name, Value value)
    {
        if (!_variables.TryAdd(name, value))
        {
            throw new InvalidOperationException($"Variable with name {name} already defined");
        }
    }

    /// <summary>
    /// Присваивает значение переменной с указанным именем.
    /// </summary>
    public void AssignVariable(string name, Value value)
    {
        if (!TryAssignVariable(name, value))
        {
            throw new InvalidOperationException($"No variable with name {name}");
        }
    }

    private bool TryAssignVariable(string name, Value value)
    {
        if (_variables.ContainsKey(name))
        {
            _variables[name] = value;
            return true;
        }

        if (_parent != null)
        {
            return _parent.TryAssignVariable(name, value);
        }

        return false;
    }
}