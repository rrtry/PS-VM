using Runtime;

using ValueType = Runtime.ValueType;

namespace Semantics.Symbols;

/// <summary>
/// Символ переменной в таблице символов.
/// </summary>
public sealed class VariableSymbol
{
    public VariableSymbol(string name, ValueType declaredType, int scopeLevel)
    {
        Name = name;
        DeclaredType = declaredType;
        ScopeLevel = scopeLevel;
        IsInitialized = false;
    }

    /// <summary>
    /// Имя переменной.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Тип переменной.
    /// </summary>
    public ValueType DeclaredType { get; }

    /// <summary>
    /// Уровень области видимости.
    /// </summary>
    public int ScopeLevel { get; }

    /// <summary>
    /// Флаг: была ли переменная инициализирована.
    /// </summary>
    public bool IsInitialized { get; set; }
}