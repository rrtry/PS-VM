using Runtime;

using ValueType = Runtime.ValueType;

namespace Ast.Declarations;

/// <summary>
/// Определение встроенной функции языка.
/// </summary>
public sealed class NativeFunction : AbstractFunctionDeclaration
{
    private readonly Func<IReadOnlyList<Value>, Value>? implementation;

    // No implementation
    public NativeFunction(
        string name,
        IReadOnlyList<NativeFunctionParameter> parameters,
        ValueType resultType)
        : base(name, parameters)
    {
        ResultType = resultType;
    }

    public NativeFunction(
        string name,
        IReadOnlyList<NativeFunctionParameter> parameters,
        ValueType resultType,
        Func<IReadOnlyList<Value>, Value> implementation
    )
        : base(name, parameters)
    {
        ResultType = resultType;
        this.implementation = implementation;
    }

    public Value Invoke(IReadOnlyList<Value> arguments)
    {
        if (implementation != null)
        {
            return implementation(arguments);
        }

        throw new NotImplementedException();
    }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}