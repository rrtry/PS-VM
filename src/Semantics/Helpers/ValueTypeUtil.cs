using Runtime;

using ValueType = Runtime.ValueType;

namespace Semantics.Helpers;

public static class ValueTypeUtil
{
    public static bool AreNumeric(ValueType a, ValueType b)
    {
        return (a == ValueType.Int || a == ValueType.Float) &&
               (b == ValueType.Int || b == ValueType.Float);
    }

    public static bool AreExactTypes(ValueType a, ValueType b)
    {
        return (a == ValueType.Int && b == ValueType.Int) ||
               (a == ValueType.Float && b == ValueType.Float) ||
               (a == ValueType.String && b == ValueType.String);
    }

    public static bool CanApplyBinaryOperation(ValueType a, ValueType b)
    {
        return AreNumeric(a, b) || (a == ValueType.String && b == ValueType.String);
    }

    public static bool CanBeAssigned(ValueType left, ValueType right)
    {
        return (left == ValueType.String && right == ValueType.String) ||
               ((left == ValueType.Float) &&
               (right == ValueType.Float || right == ValueType.Int));
    }
}