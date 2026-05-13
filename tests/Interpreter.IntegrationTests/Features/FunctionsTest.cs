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
                fn fact(n: int): int {  

                    if (n <= 1) {
                        return 1;
                    }

                    return n * fact(n - 1);
                }

                fn main(): int {
                    let x = fact(5);
                    printi(x);
                    return 0;
                }
                
                ", [], "120"
            },
            {
                @"
                fn sum(a: int, b: int): int {
                    let result = a + b;
                    return result;
                }
                
                fn main(): int {
                    printi(sum(10, 20));
                    return 0;
                }
                ", [], "30"
            },
            {
                @"
                fn avg(a: float, b: float): float {
                    return (a + b) / 2.0;
                }

                fn main(): int {
                    printf(avg(5.0, 9.0), 1);
                    return 0;
                }
                ", [], "7.0"
            },
            {
                @"
                fn greet(name: str): unit {
                    print(""Hello, "");
                    print(name);
                }

                fn main(): int {
                    greet(""World"");
                    return 0;
                }
                ", [], "Hello, World"
            },
            {
                @"
                fn double(x: int): int {
                    return x * 2;
                }

                fn square(x: int): int {
                    return x * x;
                }

                fn main(): int {
                    printi(square(double(3)));
                    return 0;
                }
                ", [], "36"
            },
        };
    }
}