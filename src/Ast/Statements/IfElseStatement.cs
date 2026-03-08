namespace Ast.Statements;

using Ast;
using Ast.Expressions;

public sealed class IfElseStatement : Statement
{
    public IfElseStatement(Expression condition, BlockStatement thenBranch, BlockStatement? elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public Expression Condition { get; }

    public Statement ThenBranch { get; }

    public Statement? ElseBranch { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}