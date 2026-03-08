using Ast.Attributes;

using ValueType = Runtime.ValueType;

namespace Ast.Declarations;

/// <summary>
/// Объявление параметра функции.
/// </summary>
public class ParameterDeclaration : AbstractParameterDeclaration
{
    private AstAttribute<AbstractTypeDeclaration?> declaredType;

    public ParameterDeclaration(string name, string typeName)
        : base(name)
    {
        this.TypeName = typeName;
    }

    public string TypeName { get; }

    public AbstractTypeDeclaration Type
    {
        get => declaredType.Get() ?? throw new InvalidOperationException(
            $"No declaration for parameter type {this.TypeName}"
        );
        set => declaredType.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}