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

    /// <summary>
    /// Логическиое "ИЛИ"
    /// </summary>
    Or,

    /// <summary>
    /// Логическое "И"
    /// </summary>
    And,

    /// <summary>
    /// Операция "БОЛЬШЕ" >
    /// </summary>
    Greater,

    /// <summary>
    /// Операция "МЕНЬШЕ" <
    /// </summary>
    Less,

    /// <summary>
    /// Операция "БОЛЬШЕ ИЛИ РАВНО" >=
    /// </summary>
    GreaterOrEqual,

    /// <summary>
    /// Операция "МЕНЬШЕ ИЛИ РАВНО" <=
    /// </summary>
    LessOrEqual,

    /// <summary>
    /// Операция "РАВНО" ==
    /// </summary>
    Equal,

    /// <summary>
    /// Операция "НЕ РАВНО" !=
    /// </summary>
    NotEqual,
}