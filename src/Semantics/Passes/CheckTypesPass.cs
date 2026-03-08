using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Semantics.Exceptions;
using Semantics.Helpers;

using ValueType = Runtime.ValueType;

namespace Semantics.Passes;

/// <summary>
/// Проход по AST для проверки корректности программы с точки зрения совместимости типов данных.
/// </summary>
/// <exception cref="TypeErrorException">Бросается при несоответствии типов данных в процессе проверки.</exception>
public class CheckTypesPass : AbstractPass
{
    private readonly HashSet<Statement> visitedStatements = new();
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
        visitedStatements.Clear();
        base.Visit(d);

        if (d.DeclaredType != null &&
            d.DeclaredType.ResultType != ValueType.Unit)
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
        if (currentFunction != null && currentFunction.DeclaredType != null)
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

    /// <summary>
    /// Проверяет тип переменной и тип выражения, которым она инициализируется.
    /// </summary>
    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);

        ValueType inferredType = d.InitialValue.ResultType;
        if (inferredType == ValueType.Unit)
        {
            throw new TypeErrorException("Cannot initialize variable from expression without value");
        }

        if (d.DeclaredType != null && !ValueTypeUtil.AreExactTypes(d.DeclaredType.ResultType, inferredType))
        {
            throw new TypeErrorException(
                $"Cannot initialize variable of type {d.DeclaredTypeName} with value of type {inferredType}"
            );
        }
    }

    public override void Visit(AssignmentExpression e)
    {
        base.Visit(e);
        if (!ValueTypeUtil.AreExactTypes(e.Left.ResultType, e.Right.ResultType))
        {
            throw new TypeErrorException(
                $"Cannot assign value of type {e.Right.ResultType} to variable of type {e.Left.ResultType}"
            );
        }
    }

    public override void Visit(IfElseStatement e)
    {
        base.Visit(e);
        CheckAreSameTypes("if-else condition", e.Condition, ValueType.Int);
    }

    public override void Visit(WhileLoopStatement e)
    {
        base.Visit(e);
        CheckAreSameTypes("while loop condition", e.Condition, ValueType.Int);
    }

    public override void Visit(ForLoopStatement e)
    {
        base.Visit(e);
        if (e.StartValue is AssignmentExpression)
        {
            CheckAreSameTypes("for loop start value", (AssignmentExpression)e.StartValue, ValueType.Int);
        }
        else if (e.StartValue is VariableDeclaration)
        {
            CheckAreSameTypes("for loop start value", (VariableDeclaration)e.StartValue, ValueType.Int);
        }
        else
        {
            throw new TypeErrorException($"The only allowed type for for-loop iterator is int");
        }
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

    private static void CheckAreSameTypes(string category, Declaration declaration, ValueType expectedType)
    {
        if (!ValueTypeUtil.AreExactTypes(declaration.ResultType, expectedType))
        {
            throw new TypeErrorException(category, expectedType, declaration.ResultType);
        }
    }

    private static void CheckAreSameTypes(string category, Expression expression, ValueType expectedType)
    {
        if (!ValueTypeUtil.AreExactTypes(expression.ResultType, expectedType))
        {
            throw new TypeErrorException(category, expectedType, expression.ResultType);
        }
    }

    private bool GuaranteesReturn(Statement statement)
    {
        if (visitedStatements.Contains(statement))
        {
            return false;
        }

        visitedStatements.Add(statement);
        return statement switch
        {
            BlockStatement block => GuaranteesReturn(block),
            IfElseStatement ifElse => GuaranteesReturn(ifElse),
            ReturnStatement => true,
            _ => false,
        };
    }

    private bool GuaranteesReturn(IfElseStatement ifElse)
    {
        if (ifElse.ElseBranch != null)
        {
            return GuaranteesReturn(ifElse.ThenBranch) &&
                   GuaranteesReturn(ifElse.ElseBranch);
        }

        return false;
    }

    private bool GuaranteesReturn(BlockStatement block)
    {
        foreach (Statement stmt in block.Statements.OfType<Statement>())
        {
            if (GuaranteesReturn(stmt))
            {
                return true;
            }
        }

        return false;
    }
}