using System.Diagnostics;

using Interpreter;
using Parser;
using Runtime;
using Tests.TestLibrary.TestDoubles;

namespace Interpreter.IntegrationTests;

public class ExpressionsTest
{
    [Theory]
    [MemberData(nameof(GetEvaluateExpressionsData))]
    public void Can_evaluate_expressions(string code, Value expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        Value result = interpreter.Execute(code);
        Assert.Equal(expected, result, EqualityComparer<Value>.Default);
    }

    [Theory]
    [MemberData(nameof(GetInvalidExpressionsData))]
    public void Rejects_invalid_expressions(string code)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        Assert.Throws<UnexpectedLexemeException>(() => interpreter.Execute(code));
    }

    public static TheoryData<string, Value> GetEvaluateExpressionsData()
    {
        return new TheoryData<string, Value>
        {
            {
                // Вещественное число
                "fn main() { 3.14 * 2; }", new Value(3.14 * 2)
            },
            {
                // Степень и унарный минус
                "fn main() { -5 ** 2; }", new Value(25)
            },
            {
                // Модуль и возведение в степень
                "fn main() { 4 % 2 ** 2; }", new Value(0)
            },
            {
                // Разбор арифметических выражений с учётом приоритета
                "fn main() { 1 + 2 * 8 / 3 - 1; }", new Value(5)
            },
            {
                // Разбор арифметических выражений с учётом скобок
                "fn main() { (1 + 2) * (8 / (3 - 1)); }", new Value(12)
            },

            // Проверка правоассоциативности арифметических операций
            {
                "fn main() { 2 ** 3 ** 2; }", new Value(512)
            },

            // Проверка левоассоциативности арифметических операций
            {
                "fn main() { 10 - 3 - 2; }", new Value(5)
            },
            {
                "fn main() { 10 / 3 / 2; }", new Value(1)
            },
            {
                "fn main() { 10 - 3 + 2; }", new Value(9)
            },
            {
                "fn main() { 10 / 3 * 2; }", new Value(6)
            },

            // Разбор унарного минуса
            {
                "fn main() { -4; }", new Value(-4)
            },
            {
                "fn main() { 2 * 2 * -5; }", new Value(-20)
            },

            // Разбор операторов сравнения
            {
                "fn main() { 1 + 2 < 5; }", new Value(1)
            },
            {
                "fn main() { 2 * 2 > 5; }", new Value(0)
            },
            {
                "fn main() { 2 * 2 == 5; }", new Value(0)
            },
            {
                "fn main() { 2 / 2 != 4; }", new Value(1)
            },
            {
                "fn main() { 2 * 2 >= 4; }", new Value(1)
            },
            {
                "fn main() { 2 - 1 <= 1; }", new Value(1)
            },
            {
                "fn main() { 1 == (2 == 3); }", new Value(0)
            },

            // Разбор операций сравнения строк
            {
                """
                fn main() { "Hello" == "Hello!"; }
                """,
                new Value(0)
            },
            {
                """
                fn main() { "Hello" != "Hello!"; }
                """,
                new Value(1)
            },
            {
                """
                fn main() { "Bob" > "Alice"; }
                """,
                new Value(1)
            },
            {
                """
                fn main() { "Bob" < "Alice"; }
                """,
                new Value(0)
            },
            {
                """
                fn main() { "Bob" >= "Alice"; }
                """,
                new Value(1)
            },
            {
                """
                fn main() { "Bob" <= "Alice"; }
                """,
                new Value(0)
            },

            // Разбор логических операторов
            {
                "fn main() { 1 && 0; }", new Value(0)
            },
            {
                "fn main() { 1 || 0; }", new Value(1)
            },
        };
    }

    public static TheoryData<string> GetInvalidExpressionsData()
    {
        return
        [
            "fn main() { 1 < 2 < 3; }",
            "fn main() { 1 == 2 == 3; }",
        ];
    }
}