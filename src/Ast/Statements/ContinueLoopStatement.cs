namespace Ast.Statements;

public class ContinueLoopStatement : Statement
{
    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}