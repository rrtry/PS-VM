using ValueType = Runtime.ValueType;

namespace Semantics.Helpers;

public static class ValueTypeUtil
{
    public static bool AreNumeric(ValueType a, ValueType b)
    {
        return (a == ValueType.Int || a == ValueType.Float) &&
               (b == ValueType.Int || b == ValueType.Float);
    }

    public static bool AreEqual(ValueType a, ValueType b)
    {
        return (a == ValueType.Int && b == ValueType.Int) ||
               (a == ValueType.Float && b == ValueType.Float) ||
               (a == ValueType.Str && b == ValueType.Str);
    }
}