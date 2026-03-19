using Runtime;
using Semantics.Exceptions;
using Tests.TestLibrary.TestDoubles;

namespace Interpreter.IntegrationTests;

public class BuiltinFunctionsTest
{
    [Theory]
    [MemberData(nameof(GetEvaluateBuiltinFunctionsData))]
    public void Can_evaluate_builtin_functions(string code, Value expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        Value result = interpreter.Execute(code);
        Assert.Equal(expected, result, EqualityComparer<Value>.Default);
    }

    public static TheoryData<string, Value> GetEvaluateBuiltinFunctionsData()
    {
        return new TheoryData<string, Value>
        {
            // Функции преобразования типов
            {
                "fn main() { stoi(\"5\"); }", new Value(5)
            },
            {
                "fn main() { itos(5); }", new Value("5")
            },
            {
                "fn main() { itof(49); }", new Value(49.0)
            },
            {
                "fn main() { ftoi(49.0); }", new Value(49)
            },
            {
                @"fn main() { stof(""49.0""); }", new Value(49.0)
            },
            {
                "fn main() { ftos(49.14, 2); }", new Value("49.14")
            },

            // Функции работы со строками
            {
                "fn main() { strlen(\"Hello!\"); }", new Value(6)
            },
            {
                "fn main() { substr(\"Hello!\", 2, 2); }", new Value("ll")
            },
            {
                "fn main() { substr(\"Hello!\", 2, 4); }", new Value("llo!")
            },
            {
                "fn main() { sconcat(\"Ali\", \"ce\"); }", new Value("Alice")
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetEvaluateOutputFunctionsData))]
    public void Can_evaluate_output_functions(
        string code,
        Value expectedResult,
        string expectedBufferedOutput,
        string expectedFlushedOutput
    )
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        Value result = interpreter.Execute(code);

        Assert.Equal(expectedResult, result);
        Assert.Equal(expectedBufferedOutput, environment.BufferedOutput);
        Assert.Equal(expectedFlushedOutput, environment.FlushedOutput);
    }

    public static TheoryData<string, Value, string, string> GetEvaluateOutputFunctionsData()
    {
        return new TheoryData<string, Value, string, string>
        {
            // Функции вывода
            {
                "fn main() { print(\"Hello!\"); }", Value.Unit, "Hello!", ""
            },
            {
                "fn main() { printi(2 + 7); }", Value.Unit, "9", ""
            },
            {
                "fn main() { printi(2 + 7); print(\"\\n\"); printi(2 - 7); print(\"\\n\"); }", Value.Unit, "9\n-5\n", ""
            },
            {
                "fn main() { printi(7); print(\"\\n\"); printi(4); }", Value.Unit, "7\n4", ""
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetEvaluateInputFunctionsData))]
    public void Can_evaluate_input_functions(string code, string input, string expectedBufferedOutput)
    {
        FakeEnvironment environment = new();
        environment.AddInput(input);

        Interpreter interpreter = new(environment);
        Value result = interpreter.Execute(code);

        Assert.Equal(result, Value.Unit);
        Assert.Equal(expectedBufferedOutput, environment.BufferedOutput);
    }

    public static TheoryData<string, string, string> GetEvaluateInputFunctionsData()
    {
        return new TheoryData<string, string, string>
        {
            {
                "fn main() { print(input()); }", "x", "x"
            },
        };
    }

    [Theory]
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
                "fn main() { length(\"Hello!\"); }", typeof(UnknownSymbolException)
            },

            // Нельзя вызвать встроенную функцию с неправильными типами аргументов
            {
                "fn main() { strlen(10); }", typeof(TypeErrorException)
            },

            // Нельзя вызвать встроенную функцию с неправильным числом аргументов
            {
                "fn main() { strlen(\"Hello!\", \"World\"); }", typeof(InvalidFunctionCallException)
            },
            {
                "fn main() { strlen(); }", typeof(InvalidFunctionCallException)
            },
            {
                "fn main() { sconcat(); }", typeof(InvalidFunctionCallException)
            },
            {
                "fn main() { sconcat(\"a\"); }", typeof(InvalidFunctionCallException)
            },
            {
                "fn main() { sconcat(\"a\", \"b\", \"c\"); }", typeof(InvalidFunctionCallException)
            },
        };
    }
}