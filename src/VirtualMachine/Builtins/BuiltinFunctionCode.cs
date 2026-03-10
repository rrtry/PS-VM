namespace VirtualMachine.Builtins;

/// <summary>
/// Код встроенной функции.
/// </summary>
public enum BuiltinFunctionCode
{
    /// <summary>
    /// `print(s: string)` — выводит строку в стандартный поток вывода
    /// </summary>
    Print = 0,

    /// <summary>
    /// `printi(i: int)` — выводит целое число в стандартный поток вывода
    /// </summary>
    PrintI = 1,

    /// <summary>
    /// `printf(i: float, i: int)` — выводит вещественное число в стандартный поток вывода
    /// </summary>
    PrintF = 2,

    /// <summary>
    /// `itos(i: int): string` — возвращает строковое представление целого числа
    /// </summary>
    ItoS = 3,

    /// <summary>
    /// `ftos(f: float): string` — возвращает строковое представление вещественного числа
    /// </summary>
    FtoS = 4,

    /// <summary>
    /// `itof(i: int): float` — перевод целого числа в вещественное
    /// </summary>
    ItoF = 5,

    /// <summary>
    /// `ftoi(f: float): int` — перевод вещественного числа в целое
    /// </summary>
    FtoI = 6,

    /// <summary>
    /// `stoi(s: str): int` — перевод строкового представления числа в целое число
    /// </summary>
    StoI = 7,

    /// <summary>
    /// `stof(i: float, i: int): float` — перевод строкового представления числа в вещественное число
    /// </summary>
    StoF = 8,

    /// <summary>
    /// `sconcat(s1: str, s2: str): str` — конкаценация строк
    /// </summary>
    SConcat = 9,

    /// <summary>
    /// `substr(s: str, from: int, to: int): str` — срез строки
    /// </summary>
    SubStr = 10,

    /// <summary>
    /// `strlen(s: str): int` — длина строки
    /// </summary>
    StrLen = 11,

    /// <summary>
    /// `input(): str` - возвращает строки из stdin
    /// </summary>
    Input = 12,
}