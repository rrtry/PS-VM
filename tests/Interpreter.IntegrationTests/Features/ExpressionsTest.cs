using Parser;
using Tests.TestLibrary;

namespace Interpreter.IntegrationTests;

public class ExpressionsTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetArithmeticExpressions))]
    [MemberData(nameof(GetLogicalExpressions))]
    public void Can_evaluate_expressions(string code, string expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetInvalidExpressionsData))]
    public void Rejects_invalid_expressions(string code)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        Assert.Throws<UnexpectedLexemeException>(() => interpreter.Execute(code));
    }

    public static TheoryData<string, string> GetLogicalExpressions()
    {
        return new TheoryData<string, string>
        {
            {
                // Логическое "ИЛИ", "И", "НЕ"
                @"
                fn main(): int {
                    let f = 1 && 0;
                    let t = 1 || 0;
                    printi(f);
                    printi(t);
                    printi(!f); // not
                    printi(!t);
                    return 0;
                }
                ",
                "0110"
            },
            {
                // Операции сравнения (int)
                @"
                fn main(): int {

                    let less = 1 < 0;
                    let greater = 1 > 0;

                    let lessEq = 1 <= 0;
                    let greaterEq = 1 >= 0;

                    let eqFalse = 1 == 0;
                    let neqTrue = 1 != 0;

                    printi(less);
                    printi(greater);
                    printi(lessEq);
                    printi(greaterEq);

                    print(""\n"");
                    printi(eqFalse);
                    printi(neqTrue);

                    return 0;
                }
                ",
                "0101\n01"
            },
            {
                // Операции сравнения (str)
                @"
                fn main(): int {

                    let s1 = ""Hello"";
                    let s2 = ""Hello"";

                    printi(s1 == s2);
                    s2 = ""World"";
                    printi(s1 != s2);

                    return 0;
                }
                ",
                "11"
            },
            {
                // Операции сравнения (float)
                @"
                fn main(): int {

                    let euler = 2.718281828459;
                    let pi = 3.14159265358979323846;

                    printi(pi > euler);  // 1
                    printi(pi < euler);  // 0
                    printi(pi >= euler); // 1
                    printi(pi <= euler); // 0
                    printi(euler != pi); // 1
                    printi(euler == pi); // 0
                    printi(euler == euler); // 1
                    printi(pi == pi); // 1

                    return 0;
                }
                ",
                "10101011"
            },
        };
    }

    public static TheoryData<string, string> GetArithmeticExpressions()
    {
        return new TheoryData<string, string>
        {
            {
                // Вещественное число
                "fn main(): int { printf(3.14 * 2, 2); return 0; }", "6.28"
            },
            {
                // Степень и унарный минус
                "fn main(): int { printi(-5 ** 2); return 0; }", "-25"
            },
            {
                // Степень и унарный минус
                "fn main(): int { printi((-5) ** 2); return 0; }", "25"
            },
            {
                // Модуль и возведение в степень
                "fn main(): int { printi(4 % 3 ** 2); return 0; }", "4"
            },
            {
                // Модуль и возведение в степень с приоритетом
                "fn main(): int { printi((4 % 3) ** 2); return 0; }", "1"
            },
            {
                // Разбор арифметических выражений с учётом приоритета
                "fn main(): int { printi(1 + 2 * 8 / 3 - 1); return 0; }", "5"
            },
            {
                // Разбор арифметических выражений с учётом скобок
                "fn main(): int { printi((1 + 2) * (8 / (3 - 1))); return 0; }", "12"
            },

            // Проверка правоассоциативности арифметических операций
            {
                "fn main(): int { printi(2 ** 3 ** 2); return 0; }", "512"
            },

            // Проверка левоассоциативности арифметических операций
            {
                "fn main(): int { printi(10 - 3 - 2); return 0; }", "5"
            },
            {
                "fn main(): int { printi(10 / 3 / 2); return 0; }", "1"
            },
            {
                "fn main(): int { printi(10 - 3 + 2); return 0; }", "9"
            },
            {
                "fn main(): int { printi(10 / 3 * 2); return 0; }", "6"
            },

            // Унарные операторы
            {
                "fn main(): int { printi(+1); return 0; }", "1"
            },
            {
                "fn main(): int { printi(-4); return 0; }", "-4"
            },
            {
                "fn main(): int { printi(2 * 2 * -5); return 0; }", "-20"
            },
            {
                "fn main(): int { printi(1+-1); return 0; }", "0"
            },
            {
                "fn main(): int { printi(1++1); return 0; }", "2"
            },
            {
                "fn main(): int { printi(-(-5)); return 0; }", "5"
            },
            {
                "fn main(): int { printi(+(-5)); return 0; }", "-5"
            },
        };
    }

    public static TheoryData<string> GetInvalidExpressionsData()
    {
        return
        [
            "fn main(): int { printi(++1); return 0; }",
            "fn main(): int { printi(--1); return 0; }",
            "fn main(): int { printi(1++); return 0; }",
            "fn main(): int { printi(1--); return 0; }",

            "fn main(): int { printi(-+1); return 0; }",
            "fn main(): int { printi(1+++1); return 0; }",
            "fn main(): int { printi(1---1); return 0; }",
            "fn main(): int { printi(1+-+1); return 0; }",
            "fn main(): int { printi(1-+-1); return 0; }",

            "fn main(): int { printi(((5 + 5) * 2); return 0; }"
        ];
    }
}