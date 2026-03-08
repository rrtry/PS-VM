using Ast;
using Ast.Statements;

public class BlockStatement : Statement
{
    public BlockStatement(List<AstNode> stmts) => Statements = stmts;

    public List<AstNode> Statements { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}