using Semantics.Exceptions;
using Tests.TestLibrary.TestDoubles;

namespace Interpreter.IntegrationTests;

public class VariablesTest
{
    [Theory]
    [MemberData(nameof(GetTypeMismatch))]
    public void Can_interpret_variable_type_mismatch(string code, Type expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        Assert.Throws(expected, () => interpreter.Execute(code));
    }

    [Theory]
    [MemberData(nameof(GetDeclarationAndAssignment))]
    public void Can_interpret_variable_decl_and_assignment(string code, string expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    [Theory]
    [MemberData(nameof(GetExpressionsWithVariables))]
    public void Can_interpret_expressions_with_variables(string code, List<string> stdin, string expected)
    {
        FakeEnvironment environment = new();
        stdin.ForEach(environment.AddInput);

        Interpreter interpreter = new(environment);
        interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    public static TheoryData<string, Type> GetTypeMismatch()
    {
        return new TheoryData<string, Type>
        {
            {
                @"fn main(): int {
                    let f: float = 0;
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {
                    let f: float = 0.0;
                    f = """";
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {
                    let i: int = """";
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {
                    let i: int = 0;
                    i = """";
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {
                    let s: str = 0;
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {
                    let s: str = """";
                    s = 0;
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {
                    let i: int = 0.0;
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"fn main(): int {
                    let i: int = 0;
                    i = 0.0;
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
        };
    }

    public static TheoryData<string, List<string>, string> GetExpressionsWithVariables()
    {
        return new TheoryData<string, List<string>, string>
        {
            // ints
            {
                @"fn main(): int {
                    let mod: int = stoi(input());
                    printi(mod % 3 ** 2);
                    return 0;
                }
                ",
                ["4"],
                "4"
            },
            {
                @"fn main(): int { 
                    let num = stoi(input());
                    let power = stoi(input()); 
                    power = power * 2; 
                    printi(num ** power);
                    return 0;
                }
                ",
                ["2", "2"],
                "16"
            },
            {
                @"fn main(): int { 
                    let num = stoi(input());
                    let power = stoi(input()); 
                    printi(num ** power);
                    return 0;
                }
                ",
                ["-5", "2"],
                "25"
            },
            {
                @"fn main(): int { 
                    let num = stoi(input());
                    let power = stoi(input()); 
                    printi(-num ** power);
                    return 0;
                }
                ",
                ["-5", "2"],
                "-25"
            },
            {
                @"fn main(): int { 
                    let num = stoi(input());
                    let power = stoi(input()); 
                    printi((-num) ** power);
                    return 0;
                }
                ",
                ["-5", "2"],
                "25"
            },
            {
                @"fn main(): int { 
                    let a = stoi(input());
                    let b = stoi(input()); 
                    printi((a + b) * (8 / (3 - 1)));
                    return 0;
                }
                ",
                ["1", "2"],
                "12"
            },

            // strings
            {
                @"fn main(): int { 
                    let a = input();
                    let b = input();
                    print(sconcat(a, b));
                    return 0;
                }
                ",
                ["Hello, ", "World!"],
                "Hello, World!"
            },

            // floats
            {
                @"fn main(): int { 
                    let r = stoi(input());
                    let pi = 3.14159265358979323846;
                    printf(pi * r ** 2, 4);
                    return 0;
                }
                ",
                ["3"],
                "28.2743"
            },
        };
    }

    public static TheoryData<string, string> GetDeclarationAndAssignment()
    {
        return new TheoryData<string, string>
        {
            {
                "fn main(): int { let x: int = 0; printi(x); return 0; }", "0"
            },
            {
                "fn main(): int { let x: str = \"Hello World\"; print(x); return 0; }", "Hello World"
            },
            {
                "fn main(): int { let x: float = 3.14; printf(x, 2); return 0; }", "3.14"
            },
            {
                "fn main(): int { let x = 3.14; x = 0.0; printf(x, 1); return 0; }", "0.0"
            },
        };
    }
}