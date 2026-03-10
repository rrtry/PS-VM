namespace Parser.UnitTests;

using Interpreter;
using Tests.TestLibrary.TestDoubles;
using Xunit.Sdk;

public class ParserTests
{
    [Theory]
    [MemberData(nameof(GetExamplePrograms))]
    [MemberData(nameof(GetNumberLiterals))]
    [MemberData(nameof(GetExpressions))]
    [MemberData(nameof(GetFunctionCalls))]
    public void Can_interpret_simple_programs(string source, List<string> expected)
    {
        List<string> expectedOutput = expected;
        FakeEnvironment environment = new FakeEnvironment();

        List<string> evaluated = environment.Evaluated;
        Interpreter interpreter = new Interpreter(environment);
        interpreter.Execute(source);

        Assert.Equal(expectedOutput.Count, evaluated.Count);
        for (int i = 0; i < evaluated.Count; i++)
        {
            if (expectedOutput[i] != evaluated[i])
            {
                throw new XunitException($"Expected: {expectedOutput[i]}, got: {evaluated[i]}");
            }
        }
    }

    public static TheoryData<string, List<string>> GetExamplePrograms()
    {
        return new TheoryData<string, List<string>>
        {
            {
                "let x = 1; " +
                "let y = 2; " +
                "printi(x + y);",
                new List<string> { "3" }
            },
            {
                "let x = 1; " +
                "let y = 2; " +
                "y = 3; " +
                "printi(x + y);",
                new List<string> { "4" }
            },
            {
                "let x = 2; " +
                "let y = 2; " +
                "let z = x + y;" +
                "let result = sqrt(itof(z)); " +
                "printf(result, 2);",
                new List<string> { "2.00" }
            },
            {
                "let PI = 3.14159265358979323846; " +
                "let radius = 10; " +
                "let area = (radius ** 2) * PI; " +
                "printf(area, 2);",
                new List<string> { "314.16" }
            },
        };
    }

    public static TheoryData<string, List<string>> GetFunctionCalls()
    {
        return new TheoryData<string, List<string>>
        {
            { "printi(abs(-42));",      new List<string> { "42" } },
            { "printi(max(1, 3));",     new List<string> { "3" } },
            { "printi(min(1, 3));",     new List<string> { "1" } },
            { "printi(min(1 - 2, 3));", new List<string> { "-1" } },
            {
                "let x = 1; let y = 2; printi(abs(min(x - y, 3)));",
                new List<string> { "1" }
            },
            {
                "let x = 1; let y = 2; printi(max(x - y, 3));",
                new List<string> { "3" }
            },
            {
                "let x = 1; let y = 2; printi(min(x - y, 3));",
                new List<string> { "-1" }
            },
            {
                "let x = 2; let y = 3; printi(pow(x, y));",
                new List<string> { "8" }
            },
            {
                "let x = 2; let y = 3; let p = pow(x, y); printi(p);",
                new List<string> { "8" }
            },
        };
    }

    public static TheoryData<string, List<string>> GetExpressions()
    {
        return new TheoryData<string, List<string>>
        {
            { "printi(4 % 2);",           new List<string> { "0" } },
            { "printi(1 + 2);",           new List<string> { "3" } },
            { "printi(1 + 6 / 2);",       new List<string> { "4" } },
            { "printi(-12 / -4 / -3);",   new List<string> { "-1" } },
            { "printi(-12 / 4 / 3);",     new List<string> { "-1" } },
            { "printi(12 / 4 / 3);",      new List<string> { "1" } },
            { "printi(12 / 6 / 2);",      new List<string> { "1" } },
            { "printi(12 / (6 / 2));",    new List<string> { "4" } },
            { "printi(2 * 3 + 4);",       new List<string> { "10" } },
            { "printi(2 + 3 * 4);",       new List<string> { "14" } },
            { "printi((2 + 3) * 5);",     new List<string> { "25" } },
            { "printi(2 ** 3 ** 2);",       new List<string> { "512" } },
            { "printi(2 ** 3 + 4 * 5);",   new List<string> { "28" } },
            { "printi(2 ** (2 + 2) * 5);", new List<string> { "80" } },
            { "printi((-5) ** 2);",        new List<string> { "25" } },
            { "printi(-5 + 10);",         new List<string> { "5" } },
        };
    }

    public static TheoryData<string, List<string>> GetNumberLiterals()
    {
        return new TheoryData<string, List<string>>
        {
            { "printi(42);",       new List<string> { "42" } },
            { "printi(0x2a);",     new List<string> { "42" } },
            { "printi(0b101010);", new List<string> { "42" } },
            { "printf(3.14, 2);",     new List<string> { "3.14" } },
            { "printi(-(-42));",   new List<string> { "42" } },
            { "printi(+(-42));",   new List<string> { "-42" } },
        };
    }
}