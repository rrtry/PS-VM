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
                "stoi(\"5\");", new Value(5)
            },
            {
                "itos(5);", new Value("5")
            },
            {
                "itof(49);", new Value(49.0)
            },
            {
                "ftoi(49.0);", new Value(49)
            },
            {
                @"stof(""49.0"");", new Value(49.0)
            },
            {
                "ftos(49.14, 2);", new Value("49.14")
            },

            // Функции работы со строками
            {
                "strlen(\"Hello!\");", new Value(6)
            },
            {
                "substr(\"Hello!\", 2, 2);", new Value("ll")
            },
            {
                "substr(\"Hello!\", 2, 4);", new Value("llo!")
            },
            {
                "sconcat(\"Ali\", \"ce\");", new Value("Alice")
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
                "print(\"Hello!\");", Value.Unit, "Hello!", ""
            },
            {
                "printi(2 + 7);", Value.Unit, "9", ""
            },
            {
                "printi(2 + 7); print(\"\\n\"); printi(2 - 7); print(\"\\n\");", Value.Unit, "9\n-5\n", ""
            },
            {
                "printi(7); print(\"\\n\"); printi(4);", Value.Unit, "7\n4", ""
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
                "print(input());", "x", "x"
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
                "length(\"Hello!\");", typeof(UnknownSymbolException)
            },

            // Нельзя вызвать встроенную функцию с неправильными типами аргументов
            {
                "strlen(10);", typeof(TypeErrorException)
            },

            // Нельзя вызвать встроенную функцию с неправильным числом аргументов
            {
                "strlen(\"Hello!\", \"World\");", typeof(InvalidFunctionCallException)
            },
            {
                "strlen();", typeof(InvalidFunctionCallException)
            },
            {
                "sconcat();", typeof(InvalidFunctionCallException)
            },
            {
                "sconcat(\"a\");", typeof(InvalidFunctionCallException)
            },
            {
                "sconcat(\"a\", \"b\", \"c\");", typeof(InvalidFunctionCallException)
            },
        };
    }
}