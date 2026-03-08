namespace Ast.Expressions;

public class WhileLoopStatement : Expression
{
    public WhileLoopStatement(Expression condition, BlockStatement loopBody)
    {
        Condition = condition;
        LoopBody = loopBody;
    }

    public Expression Condition { get; }

    public AstNode LoopBody { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}