namespace Lexems;

/// <summary>
/// Перечисление всех типов токенов языка.
/// </summary>
public enum TokenType
{
    /// <summary>Тип str.</summary>
    Str,

    /// <summary>Тип bool.</summary>
    Bool,

    /// <summary>Тип int.</summary>
    Int,

    /// <summary>Тип float.</summary>
    Float,

    /// <summary>Тип unit.</summary>
    Unit,

    /// <summary>Определение типа (`:`).</summary>
    Colon,

    /// <summary>Определение функции (`fn`).</summary>
    Fn,

    /// <summary>Объявление переменной (`let`).</summary>
    Let,

    /// <summary>Возврат значения из функции (`return`).</summary>
    Return,

    /// <summary>Условный оператор (`if`).</summary>
    If,

    /// <summary>Альтернативный блок (`else`).</summary>
    Else,

    /// <summary>Цикл с условием (`while`).</summary>
    While,

    /// <summary>Цикл по диапазону (`for`).</summary>
    For,

    /// <summary>Прерывание цикла (`break`).</summary>
    Break,

    /// <summary>Переход к следующей итерации цикла (`continue`).</summary>
    Continue,

    /// <summary>Логическое значение "истина" (`true`).</summary>
    True,

    /// <summary>Логическое значение "ложь" (`false`).</summary>
    False,

    /// <summary>Идентификатор (имя переменной, функции и т.п.).</summary>
    Identifier,

    /// <summary>Целое число (например: `42`, `0x2A`, `0b1010`).</summary>
    IntegerLiteral,

    /// <summary>Число с плавающей точкой (например: `3.14`, `-0.5`).</summary>
    FloatLiteral,

    /// <summary>Строковый литерал (например: `"Hello"`).</summary>
    StringLiteral,

    /// <summary>Присваивание (`=`).</summary>
    Assign,

    /// <summary>Арифметическое сложение (`+`).</summary>
    Plus,

    /// <summary>Арифметическое вычитание (`-`).</summary>
    Minus,

    /// <summary>Умножение (`*`).</summary>
    Star,

    /// <summary>Деление (`/`).</summary>
    Slash,

    /// <summary> Возведение в степень (`**`).</summary>
    StarStar,

    /// <summary>Остаток от деления (`%`).</summary>
    Percent,

    /// <summary>Равно (`==`).</summary>
    Equal,

    /// <summary>Не равно (`!=`).</summary>
    NotEqual,

    /// <summary>Меньше (`<`).</summary>
    Less,

    /// <summary>Меньше или равно (`<=`).</summary>
    LessOrEqual,

    /// <summary>Больше (`>`).</summary>
    Greater,

    /// <summary>Больше или равно (`>=`).</summary>
    GreaterOrEqual,

    /// <summary>Логическое И (`&&`).</summary>
    And,

    /// <summary>Логическое ИЛИ (`||`).</summary>
    Or,

    /// <summary>Логическое отрицание (`!`).</summary>
    Not,

    /// <summary>Левая фигурная скобка (`{`).</summary>
    LeftBrace,

    /// <summary>Правая фигурная скобка (`}`).</summary>
    RightBrace,

    /// <summary>Левая круглая скобка (`(`).</summary>
    LeftParen,

    /// <summary>Правая круглая скобка (`)`).</summary>
    RightParen,

    /// <summary>Запятая (`,`).</summary>
    Comma,

    /// <summary>Точка с запятой (`;`).</summary>
    Semicolon,

    /// <summary>Конец файла.</summary>
    Eof,

    /// <summary>Неизвестный токен или ошибка лексического анализа.</summary>
    Unknown,
}