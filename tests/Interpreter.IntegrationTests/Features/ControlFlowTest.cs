using Parser;
using Tests.TestLibrary;

namespace Interpreter.IntegrationTests;

public class ControlFlowTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetControlFlowStatements))]
    public void Can_exec_control_flow_statements(string code, List<string> stdin, string expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);

        foreach(string input in stdin)
        {
            environment.AddInput(input);
        }

        interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    public static TheoryData<string, List<string>, string> GetControlFlowStatements()
    {
        return new TheoryData<string, List<string>, string>
        {
            {
                @"fn main(): int {

                    let in = input();

                    if (in == ""True"") {
                        print(""Then branch\n"");
                    } else {
                        print(""Else branch\n"");
                    }

                    in = input();
                    if (in == ""True"") {
                        print(""Then branch\n"");
                    } else {
                        print(""Else branch\n"");
                    }

                    return 0;

                }",
                ["True", "False"],
                "Then branch\nElse branch\n"
            },
            {
                @"fn main(): int {

                    let in = input();

                    if (in == ""True"") {
                        print(""Then branch\n"");
                    } else {
                        print(""Else branch\n"");
                        return 0;
                    }

                    in = input();
                    if (in == ""True"") {
                        print(""Then branch\n"");
                        return 0;
                    } else {
                        print(""Else branch\n"");
                        return 0;
                    }

                }",
                ["True", "False"],
                "Then branch\nElse branch\n"
            },
            {
                @"fn main(): int {

                    let a = true;
                    let b = false;

                    if (a) {
                        if (b) {
                            printi(1);
                            return 1;
                        } else {
                            printi(2);
                            return 2;
                        }
                    } else {
                        printi(3);
                        return 3;
                    }
                }",
                [],
                "2"
            },
        };
    }
}