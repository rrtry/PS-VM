using Ast.Expressions;
using Ast.Statements;

using Semantics.Exceptions;

namespace Semantics.Passes;

/// <summary>
/// Проверяет соблюдение контекстно-зависимых правил языка.
/// </summary>
/// <remarks>
/// Контекстно-зависимые правила не могли быть проверены при синтаксическом анализе, поскольку синтаксический анализатор
///  разбирает контекстно-свободную грамматику.
/// </remarks>
public sealed class CheckContextSensitiveRulesPass : AbstractPass
{
    private readonly Stack<ExpressionContext> expressionContextStack;

    public CheckContextSensitiveRulesPass()
    {
        expressionContextStack = [];
        expressionContextStack.Push(ExpressionContext.TopLevel);
    }

    private enum ExpressionContext
    {
        TopLevel,
        Loop,
        Function,
    }

    public override void Visit(AssignmentStatement e)
    {
        base.Visit(e);
        if (e.Left is not IdentifierExpression)
        {
            throw new InvalidAssignmentException("Left side of assignment must be variable name");
        }
    }

    public override void Visit(WhileLoopStatement s)
    {
        expressionContextStack.Push(ExpressionContext.Loop);
        base.Visit(s);
        expressionContextStack.Pop();
    }

    public override void Visit(ForLoopStatement s)
    {
        expressionContextStack.Push(ExpressionContext.Loop);
        base.Visit(s);
        expressionContextStack.Pop();
    }

    public override void Visit(ContinueStatement e)
    {
        base.Visit(e);
        if (expressionContextStack.Peek() != ExpressionContext.Loop)
        {
            throw new InvalidOperationException("The \"continue\" expression is allowed only inside the loop");
        }
    }

    public override void Visit(BreakStatement e)
    {
        base.Visit(e);
        if (expressionContextStack.Peek() != ExpressionContext.Loop)
        {
            throw new InvalidOperationException("The \"break\" expression is allowed only inside the loop");
        }
    }

    /// <summary>
    /// Проверяет корректность программы с точки зрения использования функций.
    /// </summary>
    /// <exception cref="InvalidFunctionCallException">Бросается при неправильном вызове функций.</exception>
    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);
        if (e.Arguments.Count != e.Function.Parameters.Count)
        {
            throw new InvalidFunctionCallException(
                $"Function {e.Name} requires {e.Function.Parameters.Count} arguments, got {e.Arguments.Count}"
            );
        }
    }
}