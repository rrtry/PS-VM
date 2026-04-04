using Tests.TestLibrary.TestDoubles;

namespace Interpreter.IntegrationTests;

public class VariablesTest
{
    [Theory]
    [MemberData(nameof(GetVariablesData))]
    public void Can_interpret_variables(string code, string expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    public static TheoryData<string, string> GetVariablesData()
    {
        return new TheoryData<string, string>
        {
            {
                "fn main(): int { let x = 0; printi(x); return 0; }", "0"
            },
            {
                "fn main(): int { let x = \"Hello World\"; print(x); return 0; }", "Hello World"
            },
            {
                "fn main(): int { let x = 3.14; printf(x, 2); return 0; }", "3.14"
            },
        };
    }
}