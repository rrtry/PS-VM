using Ast.Expressions;
using Runtime;

namespace Execution;

public static class EvaluationUtil
{
    public static Value ApplyBinaryOperation(
        BinaryOperation operation,
        Func<Value> evaluateLeft,
        Func<Value> evaluateRight)
    {
        return operation switch
        {
            BinaryOperation.Power => ApplyArithmeticOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => (long)Math.Pow(i1, i2),
                (d1, d2) => Math.Pow(d1, d2)
            ),
            BinaryOperation.Add => ApplyArithmeticOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 + i2,
                (d1, d2) => d1 + d2
            ),
            BinaryOperation.Substract => ApplyArithmeticOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 - i2,
                (d1, d2) => d1 - d2
            ),
            BinaryOperation.Multiply => ApplyArithmeticOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 * i2,
                (d1, d2) => d1 * d2
            ),
            BinaryOperation.Divide => ApplyArithmeticOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 / i2,
                (d1, d2) => d1 / d2
            ),
            BinaryOperation.Modulo => ApplyArithmeticOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 % i2,
                (d1, d2) => d1 % d2
            ),
            BinaryOperation.Equal => ApplyEqualityOperator(
                evaluateLeft,
                evaluateRight,
                (v1, v2) => v1.Equals(v2)
            ),
            BinaryOperation.NotEqual => ApplyEqualityOperator(
                evaluateLeft,
                evaluateRight,
                (v1, v2) => !v1.Equals(v2)
            ),
            BinaryOperation.LessThan => ApplyOrderingOperator(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 < i2,
                (d1, d2) => d1 < d2,
                (s1, s2) => string.CompareOrdinal(s1, s2) < 0
            ),
            BinaryOperation.GreaterThan => ApplyOrderingOperator(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 > i2,
                (d1, d2) => d1 > d2,
                (s1, s2) => string.CompareOrdinal(s1, s2) > 0
            ),
            BinaryOperation.LessThanOrEqual => ApplyOrderingOperator(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => DoubleUtil.IsLessOrEqual(i1, i2),
                (d1, d2) => DoubleUtil.IsLessOrEqual(d1, d2),
                (s1, s2) => string.CompareOrdinal(s1, s2) <= 0
            ),
            BinaryOperation.GreaterThanOrEqual => ApplyOrderingOperator(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => DoubleUtil.IsGreaterThanOrEqual(i1, i2),
                (d1, d2) => DoubleUtil.IsGreaterThanOrEqual(d1, d2),
                (s1, s2) => string.CompareOrdinal(s1, s2) >= 0
            ),
            BinaryOperation.Or => ApplyLogicalOr(
                evaluateLeft,
                evaluateRight
            ),
            BinaryOperation.And => ApplyLogicalAnd(
                evaluateLeft,
                evaluateRight
            ),
            _ => throw new NotImplementedException($"Unknown binary operation {operation}"),
        };
    }

    /// <summary>
    /// Выполняет арифметическую операцию, если оба операнда являются числами.
    /// Иначе бросает исключение.
    /// </summary>
    private static Value ApplyArithmeticOperation(
        Func<Value> evaluateLeft,
        Func<Value> evaluateRight,
        Func<long, long, long> intOperation,
        Func<double, double, double> floatOperation
    )
    {
        Value left = evaluateLeft();
        Value right = evaluateRight();

        if (left.IsDouble() || right.IsDouble())
        {
            return new Value(floatOperation(left.AsDouble(), right.AsDouble()));
        }

        if (left.IsLong() && right.IsLong())
        {
            return new Value(intOperation(left.AsLong(), right.AsLong()));
        }

        throw new InvalidOperationException($"Values are not comparable: {left} and {right}");
    }

    /// <summary>
    /// Выполняет операцию сравнения значений на равенство / неравенство.
    /// </summary>
    private static Value ApplyEqualityOperator(
        Func<Value> evaluateLeft,
        Func<Value> evaluateRight,
        Func<Value, Value, bool> compare
    )
    {
        Value left = evaluateLeft();
        Value right = evaluateRight();

        bool result = compare(left, right);
        return new Value(result ? 1 : 0);
    }

    /// <summary>
    /// Сравнивает два операнда на относительный порядок, если они оба являются числами или строками.
    /// Иначе бросает исключение.
    /// </summary>
    private static Value ApplyOrderingOperator(
        Func<Value> evaluateLeft,
        Func<Value> evaluateRight,
        Func<long, long, bool> compareInts,
        Func<double, double, bool> compareFloats,
        Func<string, string, bool> compareStrings
    )
    {
        Value left = evaluateLeft();
        Value right = evaluateRight();

        if (left.IsLong() && right.IsLong())
        {
            bool result = compareInts(left.AsLong(), right.AsLong());
            return new Value(result ? 1 : 0);
        }

        if (left.IsDouble() || right.IsDouble())
        {
            bool result = compareFloats(left.AsDouble(), right.AsDouble());
            return new Value(result ? 1 : 0);
        }

        if (left.IsString() && right.IsString())
        {
            bool result = compareStrings(left.AsString(), right.AsString());
            return new Value(result ? 1 : 0);
        }

        throw new InvalidOperationException($"Values are not comparable: {left} and {right}");
    }

    /// <summary>
    /// Вычисляет логическое "ИЛИ".
    /// Реализует вычисление по короткой схеме (short-circuit evaluation).
    /// </summary>
    private static Value ApplyLogicalOr(Func<Value> evaluateLeft, Func<Value> evaluateRight)
    {
        long left = evaluateLeft().AsLong();
        if (left != 0)
        {
            return new Value(1);
        }

        long result = (evaluateRight().AsLong() != 0) ? 1 : 0;
        return new Value(result);
    }

    /// <summary>
    /// Вычисляет логическое "И".
    /// Реализует вычисление по короткой схеме (short-circuit evaluation).
    /// </summary>
    private static Value ApplyLogicalAnd(Func<Value> evaluateLeft, Func<Value> evaluateRight)
    {
        long left = evaluateLeft().AsLong();
        if (left == 0)
        {
            return new Value(0);
        }

        long result = (evaluateRight().AsLong() != 0) ? 1 : 0;
        return new Value(result);
    }
}