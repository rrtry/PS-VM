namespace Ast.Statements;

using Ast.Expressions;

public class WhileLoopStatement : Statement
{
    public WhileLoopStatement(Expression condition, BlockStatement loopBody)
    {
        Condition = condition;
        LoopBody = loopBody;
    }

    public Expression Condition { get; }

    public BlockStatement LoopBody { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}