namespace Ast.Statements;

using Ast.Declarations;
using Ast.Expressions;

public sealed class ForLoopStatement : Statement
{
    public ForLoopStatement(
        string iteratorName,
        AstNode startValue,
        Expression endCondition,
        Expression? updateExpr,
        BlockStatement body
    )
    {
        Iterator = new ForLoopIteratorDeclaration(iteratorName);
        StartValue = startValue;
        EndCondition = endCondition;
        UpdateExpr = updateExpr;
        Body = body;
    }

    public ForLoopIteratorDeclaration Iterator { get; }

    public AstNode StartValue { get; }

    public Expression EndCondition { get; }

    public Expression? UpdateExpr { get; }

    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}