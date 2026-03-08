namespace Ast.Declarations;

public sealed class ForLoopIteratorDeclaration : AbstractVariableDeclaration
{
    public ForLoopIteratorDeclaration(string name)
        : base(name)
    {
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}