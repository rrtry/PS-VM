using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Semantics.Exceptions;
using Semantics.Helpers;

namespace Semantics.Passes;

/// <summary>
/// Проход по AST для проверки корректности программы с точки зрения совместимости типов данных.
/// </summary>
/// <exception cref="TypeErrorException">Бросается при несоответствии типов данных в процессе проверки.</exception>
public class CheckTypesPass : AbstractPass
{
    private FunctionDeclaration? currentFunction;

    /// <summary>
    /// Проверяет соответствие типов параметров функции и аргументов при вызове этой функции.
    /// </summary>
    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);
        CheckFunctionArgumentTypes(e, e.Function);
    }

    public override void Visit(FunctionDeclaration d)
    {
        currentFunction = d;
        base.Visit(d);

        if (d.DeclaredType != null &&
            d.DeclaredType.ResultType != Runtime.ValueType.Unit)
        {
            if (!GuaranteesReturn(d.Body))
            {
                throw new TypeErrorException(
                    $"Function '{d.Name}' with return type must guarantee a return statement on all execution paths."
                );
            }
        }

        currentFunction = null;
    }

    public override void Visit(ReturnStatement s)
    {
        base.Visit(s);
        if (currentFunction != null &&
            currentFunction.DeclaredType != null)
        {
            if (s.ReturnValue == null)
            {
                throw new TypeErrorException(
                    $"Function '{currentFunction.Name}' must return a value of type {currentFunction.DeclaredType.ResultType}"
                );
            }

            CheckAreSameTypes("return value", s.ReturnValue, currentFunction.DeclaredType.ResultType);
        }
    }

    private static void CheckAreSameTypes(string category, Expression expression, Runtime.ValueType expectedType)
    {
        if (!ValueTypeUtil.AreExactTypes(expression.ResultType, expectedType))
        {
            throw new TypeErrorException(category, expectedType, expression.ResultType);
        }
    }

    /// <summary>
    /// Проверяет гаранитию наличия return в теле функции. Упрощенная версия, так как пока не поддерживаются конструкции.
    /// </summary>
    private bool GuaranteesReturn(BlockStatement block)
    {
        foreach (Statement stmt in block.Statements.OfType<Statement>())
        {
            if (stmt is ReturnStatement)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Проверяет соответствие типов формальных параметров и фактических параметров (аргументов) при вызове функции.
    /// </summary>
    private static void CheckFunctionArgumentTypes(FunctionCallExpression e, AbstractFunctionDeclaration function)
    {
        for (int i = 0, iMax = e.Arguments.Count; i < iMax; ++i)
        {
            Expression argument = e.Arguments[i];
            AbstractParameterDeclaration parameter = function.Parameters[i];

            if (!ValueTypeUtil.AreExactTypes(parameter.ResultType, argument.ResultType))
            {
                throw new TypeErrorException(
                    $"Cannot apply argument #{i} of type {argument.ResultType} to function {e.Name} parameter {parameter.Name} which has type {parameter.ResultType}"
                );
            }
        }
    }
}