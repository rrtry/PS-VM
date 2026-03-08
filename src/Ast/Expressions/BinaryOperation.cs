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
    Substract,

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

    /// <summary>
    /// Логическое "ИЛИ".
    /// </summary>
    Or,

    /// <summary>
    /// Логическое "И".
    /// </summary>
    And,

    /// <summary>
    /// Оператор сравнения "равно".
    /// </summary>
    Equal,

    /// <summary>
    /// Оператор сравнения "не равно".
    /// </summary>
    NotEqual,

    /// <summary>
    /// Оператор сравнения "меньше".
    /// </summary>
    LessThan,

    /// <summary>
    /// Оператор сравнения "больше".
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Оператор сравнения "меньше или равно".
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Оператор сравнения "больше или равно".
    /// </summary>
    GreaterThanOrEqual,
}