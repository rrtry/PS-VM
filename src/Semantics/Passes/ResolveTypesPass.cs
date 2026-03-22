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

    /// <summary>
    /// Выполняет проверки типов для бинарных операций:
    /// 1. Арифметические и логические операции выполняются над целыми числами и возвращают число.
    /// 2. Операции сравнения выполняются над двумя числами либо двумя строками и возвращают тот же тип.
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
    /// Выполняет проверки типов для унарного минуса.
    /// Унарный минус применяется только к целым числам и возвращает целое число.
    /// </summary>
    public override void Visit(UnaryOperationExpression e)
    {
        base.Visit(e);

        ValueType operandType = e.Operand.ResultType;
        if (operandType != ValueType.Int &&
            operandType != ValueType.Float)
        {
            throw new TypeErrorException($"Unary minus operation is not allowed for type {operandType}");
        }

        e.ResultType = operandType;
    }

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
            case BinaryOperation.Add:
            case BinaryOperation.Subtract:
            case BinaryOperation.Multiply:
            case BinaryOperation.Divide:
            case BinaryOperation.Modulo:
            case BinaryOperation.Power:

                if (!ValueTypeUtil.AreNumeric(left, right))
                {
                    return null;
                }

                if (left == ValueType.Float || right == ValueType.Float)
                {
                    return ValueType.Float;
                }

                return ValueType.Int;

            case BinaryOperation.Or:
            case BinaryOperation.And:

                if (left == ValueType.Int && right == ValueType.Int)
                {
                    return ValueType.Int;
                }

                return null;

            case BinaryOperation.LessThan:
            case BinaryOperation.GreaterThan:
            case BinaryOperation.LessThanOrEqual:
            case BinaryOperation.GreaterThanOrEqual:
            case BinaryOperation.Equal:
            case BinaryOperation.NotEqual:

                if (ValueTypeUtil.AreEqual(left, right))
                {
                    return ValueType.Int;
                }

                return null;

            default:
                throw new InvalidOperationException($"Unknown binary operation {operation}");
        }
    }
}