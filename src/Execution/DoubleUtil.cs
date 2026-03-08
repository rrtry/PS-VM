namespace Runtime;

public static class DoubleUtil
{
    // Допустимая абсолютная погрешность сравнения чисел с плавающей точкой.
    public const double Tolerance = 0.001d;

    public static bool AreEqual(double a, double b)
    {
        return Math.Abs(a - b) < Tolerance;
    }

    public static bool IsGreaterThan(double a, double b)
    {
        return a > b && !AreEqual(a, b);
    }

    public static bool IsGreaterThanOrEqual(double a, double b)
    {
        return a > b || AreEqual(a, b);
    }

    public static bool IsLessThan(double a, double b)
    {
        return a < b && !AreEqual(a, b);
    }

    public static bool IsLessOrEqual(double a, double b)
    {
        return a < b || AreEqual(a, b);
    }
}