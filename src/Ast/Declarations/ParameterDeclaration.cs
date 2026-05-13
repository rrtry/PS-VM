namespace Ast.Declarations;

/// <summary>
/// Объявление параметра функции.
/// </summary>
public class ParameterDeclaration : AbstractParameterDeclaration
{
    public ParameterDeclaration(string name, BuiltinType type)
    : base(name, type)
    {
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}