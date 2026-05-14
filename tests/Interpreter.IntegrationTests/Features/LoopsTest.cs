namespace Interpreter.IntegrationTests;

using Tests.TestLibrary;

public class LoopsTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetProgramsWithLoops))]
    public void Can_exec_declared_functions(string code, List<string> input, string expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        input.ForEach(environment.AddInput);

        interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    public static TheoryData<string, List<string>, string> GetProgramsWithLoops()
    {
        return new TheoryData<string, List<string>, string>
        {
            {
                @"
                fn main(): int {
                    for (let i = 0; i < 10; i = i + 1) {
                        if (i == 5) {
                            break;
                        }
                        if (i == 0) {
                            continue;
                        }
                        printi(i);
                    }
                    return 0;
                }
                ", [], "1234"
            },
            {
                @"
                fn main(): int {
                    let i = 0;
                    while (i < 10) {
                        if (i == 5) {
                            break;
                        }
                        if (i == 0) {
                            i = i + 1;
                            continue;
                        }
                        printi(i);
                        i = i + 1;
                    }
                    return 0;
                }
                ", [], "1234"
            },
            {
                @"
                fn main(): int {
                    for (let i = 0; i < 5; i = i + 1) {
                        printi(i);
                    }
                    return 0;
                }
                ", [], "01234"
            },
            {
                @"
                fn main(): int {
                    let i = 0;
                    while (i < 10) {
                        printi(i);
                        i = i + 1;
                    }
                    return 0;
                }
                ", [], "0123456789"
            },
        };
    }
}