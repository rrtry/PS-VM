using Ast.Declarations;
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
    // Стек контекстов выражений используется для проверки контекстно-зависимых правил.
    private readonly Stack<ExpressionContext> expressionContextStack;

    public CheckContextSensitiveRulesPass()
    {
        expressionContextStack = [];
        expressionContextStack.Push(ExpressionContext.TopLevel);
    }

    private enum ExpressionContext
    {
        TopLevel,
        InsideFunction,
    }

    public override void Visit(FunctionDeclaration d)
    {
        if (d.Name != "main")
        {
            throw new InvalidDeclarationException("Currently only 'main' entry point function is supported");
        }

        if (d.ResultType != Runtime.ValueType.Int)
        {
            throw new InvalidDeclarationException("'main' function signature: fn main(): int");
        }

        expressionContextStack.Push(ExpressionContext.InsideFunction);
        try
        {
            base.Visit(d);
        }
        finally
        {
            expressionContextStack.Pop();
        }
    }

    public override void Visit(ReturnStatement e)
    {
        if (expressionContextStack.Peek() != ExpressionContext.InsideFunction)
        {
            throw new InvalidStatementException("'return' statement is allowed only within function");
        }

        base.Visit(e);
    }

    /// <summary>
    /// Проверяет корректность программы с точки зрения использования функций.
    /// </summary>
    /// <exception cref="InvalidFunctionCallException">Бросается при неправильном вызове функций.</exception>
    public override void Visit(FunctionCallExpression e)
    {
        if (expressionContextStack.Peek() != ExpressionContext.TopLevel)
        {
            base.Visit(e);
            if (e.Arguments.Count != e.Function.Parameters.Count)
            {
                throw new InvalidFunctionCallException(
                    $"Function {e.Name} requires {e.Function.Parameters.Count} arguments, got {e.Arguments.Count}"
                );
            }
        }
        else
        {
            throw new InvalidExpressionException("Top-level expressions are not allowed");
        }
    }
}