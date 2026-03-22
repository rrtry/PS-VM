namespace Ast.Expressions;

public enum BinaryOperation
{
    /// <summary>
    /// Сложение чисел.
    /// </summary>
    Add,

    /// <summary>
    /// Вычитание чисел.
    /// </summary>
    Subtract,

    /// <summary>
    /// Умножение чисел.
    /// </summary>
    Multiply,

    /// <summary>
    /// Возведение числа в степень.
    /// </summary>
    Power,

    /// <summary>
    /// Деление чисел.
    /// </summary>
    Divide,

    /// <summary>
    /// Остаток от деления чисел.
    /// </summary>
    Modulo,
}