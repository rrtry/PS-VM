using ValueType = Runtime.ValueType;

namespace Ast.Declarations;

/// <summary>
/// Определение встроенной функции языка.
/// </summary>
public sealed class NativeFunction : AbstractFunctionDeclaration
{
    public NativeFunction(
        string name,
        IReadOnlyList<NativeFunctionParameter> parameters,
        ValueType resultType)
        : base(name, parameters)
    {
        ResultType = resultType;
    }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}