using ValueType = Runtime.ValueType;

namespace Ast.Declarations;

/// <summary>
/// Объявляет параметр встроенной функции.
/// </summary>
public class NativeFunctionParameter : AbstractParameterDeclaration
{
    public NativeFunctionParameter(string name, ValueType type)
        : base(name)
    {
        ResultType = type;
    }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}