namespace Ast.Statements;

using Ast.Expressions;

public sealed class ForLoopStatement : Statement
{
    public ForLoopStatement(
        string iteratorName,
        AstNode startValue,
        Expression endCondition,
        AstNode? updateStmt,
        BlockStatement body
    )
    {
        Iterator = iteratorName;
        Counter = startValue;
        Condition = endCondition;
        Update = updateStmt;
        LoopBody = body;
    }

    public string Iterator { get; }

    public AstNode Counter { get; }

    public Expression Condition { get; }

    public AstNode? Update { get; }

    public BlockStatement LoopBody { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}