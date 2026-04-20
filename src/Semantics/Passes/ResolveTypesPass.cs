using Ast.Declarations;
using Ast.Expressions;

using Semantics.Exceptions;
using Semantics.Helpers;

using ValueType = Runtime.ValueType;

namespace Semantics.Passes;

/// <summary>
/// Проход по AST для вычисления типов данных.
/// </summary>
/// <exception cref="TypeErrorException">Бросается при несоответствии типов данных в процессе вычисления типов.</exception>
public sealed class ResolveTypesPass : AbstractPass
{
    /// <summary>
    /// Литерал всегда имеет определённый тип.
    /// </summary>
    public override void Visit(LiteralExpression e)
    {
        base.Visit(e);
        e.ResultType = e.Type;
    }

    public override void Visit(IdentifierExpression e)
    {
        base.Visit(e);
        e.ResultType = e.Variable.ResultType;
    }

    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);
        d.ResultType = d.Initializer.ResultType;
    }

    /// <summary>
    /// Выполняет проверки типов для бинарных операций.
    /// </summary>
    public override void Visit(BinaryOperationExpression e)
    {
        base.Visit(e);

        ValueType? resultType = GetBinaryOperationResultType(e.Operation, e.Left.ResultType, e.Right.ResultType);
        if (resultType is null)
        {
            throw new TypeErrorException(
                $"Binary operation {e.Operation} is not allowed for types {e.Left.ResultType} and {e.Right.ResultType}"
            );
        }

        e.ResultType = resultType;
    }

    /// <summary>
    /// Выполняет проверки типов для унарных операций
    /// </summary>
    public override void Visit(UnaryOperationExpression e)
    {
        base.Visit(e);
        ValueType operandType = e.Operand.ResultType;

        if (e.Operation == UnaryOperation.Not &&
            operandType != ValueType.Bool)
        {
            throw new TypeErrorException($"Unary operation {e.Operation} is not allowed for type {operandType}");
        }

        if (e.Operation != UnaryOperation.Not &&
            operandType != ValueType.Int &&
            operandType != ValueType.Float)
        {
            throw new TypeErrorException($"Unary operation {e.Operation} is not allowed for type {operandType}");
        }

        e.ResultType = operandType;
    }

    /// <summary>
    /// Присваивает возвращаемый тип вызванной функции.
    /// </summary>
    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);
        e.ResultType = e.Function.ResultType;
    }

    /// <summary>
    /// Вычисляет тип результата бинарной операции.
    /// Возвращает null, если бинарная операция не может быть выполнена с указанными типами.
    /// </summary>
    private static ValueType? GetBinaryOperationResultType(BinaryOperation operation, ValueType left, ValueType right)
    {
        switch (operation)
        {
            case BinaryOperation.And:
            case BinaryOperation.Or:
                {
                    if (ValueTypeUtil.AreBool(left, right))
                    {
                        return ValueType.Bool;
                    }

                    return null;
                }

            case BinaryOperation.Greater:
            case BinaryOperation.GreaterOrEqual:
            case BinaryOperation.Less:
            case BinaryOperation.LessOrEqual:
                {
                    if (ValueTypeUtil.AreNumeric(left, right) &&
                        ValueTypeUtil.AreEqual(left, right))
                    {
                        return ValueType.Bool;
                    }

                    return null;
                }

            case BinaryOperation.Equal:
            case BinaryOperation.NotEqual:
                {
                    if (ValueTypeUtil.AreEqual(left, right))
                    {
                        return ValueType.Bool;
                    }

                    return null;
                }

            case BinaryOperation.Add:
            case BinaryOperation.Subtract:
            case BinaryOperation.Multiply:
            case BinaryOperation.Divide:
            case BinaryOperation.Modulo:
            case BinaryOperation.Power:
                {
                    if (!ValueTypeUtil.AreNumeric(left, right))
                    {
                        return null;
                    }

                    if (left == ValueType.Float || right == ValueType.Float)
                    {
                        return ValueType.Float;
                    }

                    return ValueType.Int;
                }

            default:
                throw new InvalidOperationException($"Unknown binary operation {operation}");
        }
    }
}