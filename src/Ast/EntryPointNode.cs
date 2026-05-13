using Ast.Declarations;

namespace Ast;

public class EntryPointNode : AstNode
{
    public EntryPointNode(FunctionDeclaration main, List<FunctionDeclaration> functions)
    {
        Main = main;
        Functions = functions;
    }

    public FunctionDeclaration Main { get; }

    public List<FunctionDeclaration> Functions { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}