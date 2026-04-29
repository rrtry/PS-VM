using Parser;

using Semantics.Exceptions;

using Tests.TestLibrary;

namespace Interpreter.IntegrationTests;

public class ArithmeticExpressionsTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetArithmeticExpressions))]
    public void Can_evaluate_expressions(string code, string expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetInvalidSyntaxExpressions))]
    public void Reject_invalid_syntax_expressions(string code)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        Assert.Throws<UnexpectedLexemeException>(() => interpreter.Execute(code));
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetInvalidSematicExpressions))]
    public void Reject_invalid_semantic_expressions(string code, Type exception)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        Assert.Throws(exception, () => interpreter.Execute(code));
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

    public static TheoryData<string, Type> GetInvalidSematicExpressions()
    {
        return new TheoryData<string, Type>
        {
            // str и int
            {
                @"fn main(): int {""0"" + 0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" - 0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" * 0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" / 0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" ** 0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" > 0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" >= 0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" < 0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" <= 0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" == 0; return 0; }",
                typeof(TypeErrorException)
            },

            // bool и int
            {
                @"fn main(): int {true + 1; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true - 1; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true * 1; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true / 1; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true ** 1; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true > 1; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true >= 1; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true < 1; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true <= 1; return 0; }",
                typeof(TypeErrorException)
            },

            // bool и float
            {
                @"fn main(): int {true + 1.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true - 1.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true * 1.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true / 1.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true ** 1.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true > 1.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true >= 1.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true < 1.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true <= 1.0; return 0; }",
                typeof(TypeErrorException)
            },

            // bool и str
            {
                @"fn main(): int {true + ""1.0""; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true - ""1.0""; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true * ""1.0""; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true ** ""1.0""; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true / ""1.0""; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true > ""1.0""; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true >= ""1.0""; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true < ""1.0""; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {true <= ""1.0""; return 0; }",
                typeof(TypeErrorException)
            },

            // str и float
            {
                @"fn main(): int {""0"" + 0.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" - 0.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" * 0.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" / 0.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" ** 0.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" > 0.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" >= 0.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" < 0.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" <= 0.0; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" == 0.0; return 0; }",
                typeof(TypeErrorException)
            },

            // str и str
            {
                @"fn main(): int {""0"" + """"; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" - """"; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" * """"; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" / """"; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" ** """"; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" > """"; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" >= """"; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" < """"; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {""0"" <= """"; return 0; }",
                typeof(TypeErrorException)
            },

            // unary
            {
                @"fn main(): int {-true; return 0; }",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {-""""; return 0; }",
                typeof(TypeErrorException)
            },
        };
    }

    public static TheoryData<string> GetInvalidSyntaxExpressions()
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