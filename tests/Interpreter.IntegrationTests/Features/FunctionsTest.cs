namespace Interpreter.IntegrationTests;

using Tests.TestLibrary;

public class FunctionsTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetProgramsWithFunctions))]
    public void Can_exec_declared_functions(string code, List<string> input, string expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        input.ForEach(environment.AddInput);

        interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    public static TheoryData<string, List<string>, string> GetProgramsWithFunctions()
    {
        return new TheoryData<string, List<string>, string>
        {
            {
                @"
                fn func(): unit {
                    print(""User defined function"");
                }

                fn main(): int {
                    func();
                    return 0;
                }
                ", [], "User defined function"
            },
        };
    }
}