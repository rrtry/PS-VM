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
    private readonly Stack<Context> contextStack;

    public CheckContextSensitiveRulesPass()
    {
        contextStack = [];
        contextStack.Push(Context.TopLevel);
    }

    private enum Context
    {
        TopLevel,
        Loop,
        Function,
    }

    public override void Visit(FunctionDeclaration e)
    {
        contextStack.Push(Context.Function);
        base.Visit(e);
        contextStack.Pop();
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
        contextStack.Push(Context.Loop);
        base.Visit(s);
        contextStack.Pop();
    }

    public override void Visit(ForLoopStatement s)
    {
        contextStack.Push(Context.Loop);
        base.Visit(s);
        contextStack.Pop();
    }

    public override void Visit(ContinueStatement s)
    {
        base.Visit(s);
        if (contextStack.Peek() != Context.Loop)
        {
            throw new InvalidStatementException("\"continue\" statement is allowed only inside of while/for loop body");
        }
    }

    public override void Visit(BreakStatement s)
    {
        base.Visit(s);
        if (contextStack.Peek() != Context.Loop)
        {
            throw new InvalidStatementException("\"break\" statement is allowed only inside of while/for loop body");
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