using Ast;
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

        if (d.DeclaredType!.ResultType != Runtime.ValueType.Unit &&
            !GuaranteesReturn(d.Body))
        {
            throw new TypeErrorException($"Function '{d.Name}' with return type must guarantee a return statement on all execution paths.");
        }

        currentFunction = null;
    }

    public override void Visit(ReturnStatement s)
    {
        if (currentFunction == null)
        {
            return;
        }

        base.Visit(s);
        Runtime.ValueType returnType = currentFunction.DeclaredType!.ResultType;

        if (s.ReturnValue == null)
        {
            if (returnType != Runtime.ValueType.Unit)
            {
                throw new TypeErrorException($"Function '{currentFunction.Name}' must return a value of type {currentFunction.DeclaredType.ResultType}");
            }
        }
        else
        {
            CheckAreSameTypes("return value", s.ReturnValue, returnType);
        }
    }

    /// <summary>
    /// Проверяет тип переменной и тип выражения, которым она инициализируется.
    /// </summary>
    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);

        Runtime.ValueType inferredType = d.Initializer.ResultType;
        if (inferredType == Runtime.ValueType.Unit)
        {
            throw new TypeErrorException("Unit is not allowed as inferred type");
        }

        if (d.DeclaredType != null && !ValueTypeUtil.AreEqual(d.DeclaredType.ResultType, inferredType))
        {
            throw new TypeErrorException(
                $"Cannot initialize variable of type {d.DeclaredType} with value of type {inferredType}"
            );
        }
    }

    public override void Visit(AssignmentStatement s)
    {
        base.Visit(s);
        if (!ValueTypeUtil.AreEqual(s.Left.ResultType, s.Right.ResultType))
        {
            throw new TypeErrorException(
                $"Cannot assign value of type {s.Right.ResultType} to variable of type {s.Left.ResultType}"
            );
        }
    }

    public override void Visit(IfElseStatement s)
    {
        s.Condition.Accept(this);

        if (s.Condition.ResultType != Runtime.ValueType.Bool)
        {
            throw new TypeErrorException("Condition for `if` statement should have type `bool`");
        }

        s.ThenBranch.Accept(this);
        s.ElseBranch?.Accept(this);
    }

    private static void CheckAreSameTypes(string category, Expression expression, Runtime.ValueType expectedType)
    {
        if (!ValueTypeUtil.AreEqual(expression.ResultType, expectedType))
        {
            throw new TypeErrorException(category, expectedType, expression.ResultType);
        }
    }

    /// <summary>
    /// Проверяет наличие return на всех путях выполнения.
    /// </summary>
    private bool GuaranteesReturn(BlockStatement block)
    {
        return GuaranteesReturnInBlock(block.Statements);
    }

    /// <summary>
    /// Рекурсивная функция для проверки наличия return на всех путях выполнения
    /// </summary>
    private bool GuaranteesReturnInBlock(List<AstNode> statements)
    {
        for (int i = 0; i < statements.Count; i++)
        {
            AstNode statement = statements[i];
            if (statement is ReturnStatement)
            {
                if (i < statements.Count - 1)
                {
                    throw new UnreachableCodeException("Unreachable code after return statement");
                }

                return true;
            }

            if (statement is IfElseStatement ifElse &&
                GuaranteesReturnInIfElse(ifElse))
            {
                if (i < statements.Count - 1)
                {
                    throw new UnreachableCodeException("Unreachable code after if-else that always returns");
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Проверка гарантии того, что конструкция if-else содержит return на всех путях выполнения
    /// </summary>
    private bool GuaranteesReturnInIfElse(IfElseStatement ifElse)
    {
        BlockStatement thenBranch = (BlockStatement)ifElse.ThenBranch;
        BlockStatement? elseBranch = (BlockStatement?)ifElse.ElseBranch;
        bool thenGuarantees = GuaranteesReturnInBlock(thenBranch.Statements);

        if (elseBranch != null)
        {
            bool elseGuarantees = GuaranteesReturnInBlock(elseBranch.Statements);
            return thenGuarantees && elseGuarantees;
        }
        else
        {
            return false;
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
            NativeFunctionParameter parameter = (NativeFunctionParameter)function.Parameters[i]; // приведение безопасно, так как пользовательские функции пока что не могут иметь аругменты

            if (!ValueTypeUtil.AreEqual(parameter.ResultType, argument.ResultType))
            {
                throw new TypeErrorException(
                    $"Cannot apply argument #{i} of type {argument.ResultType} to function {e.Name} parameter {parameter.Name} which has type {parameter.ResultType}"
                );
            }
        }
    }
}