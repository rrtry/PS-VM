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
    public override void Visit(FunctionDeclaration d)
    {
        if (d.Name != "main")
        {
            throw new InvalidDeclarationException("Currently only 'main' entry point function is supported");
        }

        if (d.ResultType != Runtime.ValueType.Int)
        {
            throw new InvalidDeclarationException("'main' signature: fn main(): int");
        }

        base.Visit(d);
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