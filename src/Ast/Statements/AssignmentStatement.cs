using Ast.Attributes;
using Ast.Declarations;
using Ast.Expressions;

namespace Ast.Statements;

/// <summary>
/// Присваивание значения переменной.
/// </summary>
public sealed class AssignmentStatement : Statement
{
    public AssignmentStatement(Expression left, Expression right)
    {
        Left = left;
        Right = right;
    }

    public Expression Left { get; }

    public Expression Right { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}