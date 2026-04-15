using Semantics.Exceptions;
using Tests.TestLibrary;

namespace Interpreter.IntegrationTests;

public class BuiltinFunctionsTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetEvaluateBuiltinFunctionsData))]
    public void Can_evaluate_builtin_functions(string code, string expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);

        interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    public static TheoryData<string, string> GetEvaluateBuiltinFunctionsData()
    {
        return new TheoryData<string, string>
        {
            // Функции преобразования типов
            {
                "fn main(): int { printi(stoi(\"5\")); return 1; }", "5"
            },
            {
                "fn main(): int { print(itos(5)); return 0; }", "5"
            },
            {
                "fn main(): int { printf(itof(49), 1); return 0; }", "49.0"
            },
            {
                "fn main(): int { printi(ftoi(49.0)); return 0; }", "49"
            },
            {
                @"fn main(): int { printf(stof(""49.0""), 1); return 0; }", "49.0"
            },
            {
                "fn main(): int { print(ftos(49.14, 2)); return 0; }", "49.14"
            },

            // Функции работы со строками
            {
                "fn main(): int { printi(strlen(\"Hello!\")); return 0; }", "6"
            },
            {
                "fn main(): int { print(substr(\"Hello!\", 2, 2)); return 0; }", "ll"
            },
            {
                "fn main(): int { print(substr(\"Hello!\", 2, 4)); return 0; }", "llo!"
            },
            {
                "fn main(): int { print(sconcat(\"Ali\", \"ce\")); return 0; }", "Alice"
            },
        };
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetEvaluateOutputFunctionsData))]
    public void Can_evaluate_output_functions(
        string code,
        int expectedResult,
        string expectedBufferedOutput
    )
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        interpreter.Execute(code);

        Assert.Equal(expectedResult, interpreter.ExitCode);
        Assert.Equal(expectedBufferedOutput, environment.OutputBuffer);
    }

    public static TheoryData<string, int, string> GetEvaluateOutputFunctionsData()
    {
        return new TheoryData<string, int, string>
        {
            // Функции вывода
            {
                "fn main(): int { print(\"Hello!\"); return 0; }", 0, "Hello!"
            },
            {
                "fn main(): int { printi(2 + 7); return 0; }", 0, "9"
            },
            {
                "fn main(): int { printi(2 + 7); print(\"\\n\"); printi(2 - 7); print(\"\\n\"); return 0; }", 0, "9\n-5\n"
            },
            {
                "fn main(): int { printi(7); print(\"\\n\"); printi(4); return 0; }", 0, "7\n4"
            },
        };
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetEvaluateInputFunctionsData))]
    public void Can_evaluate_input_functions(string code, string input, string expectedBufferedOutput)
    {
        FakeEnvironment environment = new();
        environment.AddInput(input);

        Interpreter interpreter = new(environment);
        interpreter.Execute(code);

        Assert.Equal(expectedBufferedOutput, environment.OutputBuffer);
    }

    public static TheoryData<string, string, string> GetEvaluateInputFunctionsData()
    {
        return new TheoryData<string, string, string>
        {
            {
                "fn main(): int { print(input()); return 0; }", "x", "x"
            },
        };
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetInvalidFunctionCallsData))]
    public void Throws_on_invalid_function_calls(string code, Type expectedExceptionType)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        Assert.Throws(expectedExceptionType, () => interpreter.Execute(code));
    }

    public static TheoryData<string, Type> GetInvalidFunctionCallsData()
    {
        return new TheoryData<string, Type>
        {
            // Нельзя вызвать неизвестную функцию
            {
                "fn main(): int { length(\"Hello!\"); return 0; }", typeof(UnknownSymbolException)
            },

            // Нельзя вызвать встроенную функцию с неправильными типами аргументов
            {
                "fn main(): int { strlen(10); return 0; }", typeof(TypeErrorException)
            },

            // Нельзя вызвать встроенную функцию с неправильным числом аргументов
            {
                "fn main(): int { strlen(\"Hello!\", \"World\"); return 0; }", typeof(InvalidFunctionCallException)
            },
            {
                "fn main(): int { strlen(); return 0; }", typeof(InvalidFunctionCallException)
            },
            {
                "fn main(): int { sconcat(); return 0; }", typeof(InvalidFunctionCallException)
            },
            {
                "fn main(): int { sconcat(\"a\"); return 0; }", typeof(InvalidFunctionCallException)
            },
            {
                "fn main(): int { sconcat(\"a\", \"b\", \"c\"); return 0; }", typeof(InvalidFunctionCallException)
            },
        };
    }
}