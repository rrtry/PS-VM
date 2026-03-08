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
        expressionContextStack.Push(ExpressionContext.Default);
    }

    private enum ExpressionContext
    {
        Default,
        InsideLoop,
        InsideFunction,
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

    public override void Visit(AssignmentExpression e)
    {
        base.Visit(e);
        if (!IsLvalue(e.Left))
        {
            throw new InvalidAssignmentException("Left side of assignment must be a lvalue");
        }
    }

    public override void Visit(FunctionDeclaration d)
    {
        if (expressionContextStack.Peek() != ExpressionContext.Default)
        {
            throw new InvalidExpressionException("Function declaration is only allowed on the top level");
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

    public override void Visit(WhileLoopStatement e)
    {
        expressionContextStack.Push(ExpressionContext.InsideLoop);
        try
        {
            base.Visit(e);
        }
        finally
        {
            expressionContextStack.Pop();
        }
    }

    public override void Visit(ForLoopStatement e)
    {
        expressionContextStack.Push(ExpressionContext.InsideLoop);
        try
        {
            base.Visit(e);
        }
        finally
        {
            expressionContextStack.Pop();
        }
    }

    public override void Visit(ContinueLoopStatement e)
    {
        base.Visit(e);
        if (expressionContextStack.Peek() != ExpressionContext.InsideLoop)
        {
            throw new InvalidExpressionException("The \"continue\" expression is allowed only inside the loop");
        }
    }

    public override void Visit(BreakLoopStatement e)
    {
        base.Visit(e);
        if (expressionContextStack.Peek() != ExpressionContext.InsideLoop)
        {
            throw new InvalidExpressionException("The \"break\" expression is allowed only inside the loop");
        }
    }

    public override void Visit(ReturnStatement e)
    {
        base.Visit(e);
        if (expressionContextStack.Peek() != ExpressionContext.InsideFunction)
        {
            List<ExpressionContext> contextList = expressionContextStack.ToList();
            ExpressionContext outerContext = ExpressionContext.Default;

            foreach (ExpressionContext context in contextList)
            {
                if (context is not ExpressionContext.InsideLoop)
                {
                    outerContext = context;
                    break;
                }
            }

            if (outerContext is not ExpressionContext.InsideFunction)
            {
                throw new InvalidExpressionException("The \"return\" expression is allowed only inside the function");
            }
        }
    }

    /// <summary>
    /// Проверяет, является ли выражение lvalue-выражением.
    /// Термин lvalue означает «значение слева от присваивания».
    /// </summary>
    private static bool IsLvalue(Expression e)
    {
        if (e is VariableExpression)
        {
            return true;
        }

        return false;
    }
}