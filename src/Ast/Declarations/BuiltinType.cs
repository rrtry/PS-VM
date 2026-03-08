using ValueType = Runtime.ValueType;

namespace Ast.Declarations;

public class BuiltinType : AbstractTypeDeclaration
{
    public BuiltinType(string name, ValueType type)
        : base(name, type)
    {
    }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}