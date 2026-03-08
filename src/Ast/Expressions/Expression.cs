using Ast.Attributes;

using ValueType = Runtime.ValueType;

namespace Ast.Expressions;

public abstract class Expression : AstNode
{
    private AstAttribute<ValueType> resultType;

    /// <summary>
    /// Тип результата выражения.
    /// </summary>
    public ValueType ResultType
    {
        get => resultType.Get();

        set => resultType.Set(value);
    }
}