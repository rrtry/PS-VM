using Ast.Declarations;

namespace Ast;

public class EntryPointNode : AstNode
{
    public EntryPointNode(FunctionDeclaration main)
    {
        Main = main;
    }

    public FunctionDeclaration Main { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}