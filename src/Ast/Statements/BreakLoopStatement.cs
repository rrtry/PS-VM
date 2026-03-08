namespace Ast.Statements;

public class BreakLoopStatement : Statement
{
    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}