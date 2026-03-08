using Ast.Attributes;
using Ast.Declarations;

namespace Ast.Expressions;

/// <summary>
/// Выражение вызова функции со списком аргументов.
/// </summary>
public class FunctionCallExpression : Expression
{
    private AstAttribute<AbstractFunctionDeclaration> function;

    public FunctionCallExpression(string name, IReadOnlyList<Expression> arguments)
    {
        Name = name;
        Arguments = arguments;
    }

    public string Name { get; }

    public AbstractFunctionDeclaration Function
    {
        get => function.Get();
        set => function.Set(value);
    }

    public IReadOnlyList<Expression> Arguments { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}