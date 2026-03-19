using Ast.Expressions;

namespace Ast.Statements;

public class ReturnStatement : Statement
{
    public ReturnStatement(Expression returnValue) => ReturnValue = returnValue;

    public Expression ReturnValue { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}