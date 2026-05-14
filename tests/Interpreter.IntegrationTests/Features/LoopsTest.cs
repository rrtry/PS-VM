namespace Interpreter.IntegrationTests;

using System.Data;

using Semantics.Exceptions;

using Tests.TestLibrary;

public class LoopsTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetProgramsWithLoops))]
    public void Can_exec_loops(string code, List<string> input, string expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        input.ForEach(environment.AddInput);

        interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetInvalidProgramsWithLoops))]
    public void Throws_on_invalid_loop_statements_usage(string code, Type expectedExceptionType)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        Assert.Throws(expectedExceptionType, () => interpreter.Execute(code));
    }

    public static TheoryData<string, Type> GetInvalidProgramsWithLoops()
    {
        return new TheoryData<string, Type>
        {
            {
                @"
                fn main(): int {
                    let i = 1;
                    while (i) {
                        i = i + 1;
                    }
                    return 0;
                }
                ", typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    for (let i: float = 0.0; i < 1.0; i = i + 1)
                    {
                        printf(i, 2);
                    }
                    return 0;
                }
                ", typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    for (false; i < 1.0; i = i + 1)
                    {
                        printf(i, 2);
                    }
                    return 0;
                }
                ", typeof(SyntaxErrorException)
            },
            {
                @"
                fn main(): int {
                    let i = 0;
                    for (i; i < 1.0; i = i + 1)
                    {
                        printf(i, 2);
                    }
                    return 0;
                }
                ", typeof(SyntaxErrorException)
            },
            {
                @"fn main(): int { continue; return 0; }",
                typeof(InvalidStatementException)
            },
            {
                @"fn main(): int { break; return 0; }",
                typeof(InvalidStatementException)
            },
            {
                @"fn test(): int { continue; return 0; } fn main(): int { return 0; }",
                typeof(InvalidStatementException)
            },
            {
                @"fn main(): int { if (true) { break; } return 0; }",
                typeof(InvalidStatementException)
            },
            {
                @"fn inner(): unit { continue; } 
                fn main(): int { 
                    let i = 0; 
                    while (i < 1) {
                      inner(); 
                      i = i + 1; 
                    } 
                    return 0; 
                }",
                typeof(InvalidStatementException)
            },
        };
    }

    public static TheoryData<string, List<string>, string> GetProgramsWithLoops()
    {
        return new TheoryData<string, List<string>, string>
        {
            {
                @"
                fn loop_outer_counter() {
                    let i = 100;
                    for (i = 0; i < 5; i = i + 1) {
                        printi(i);
                    }
                }

                fn main(): int {
                    loop_outer_counter();
                    return 0;
                }
                ", [], "01234"
            },
            {
                @"
                fn return_from_loop(): int {
                
                    for (let i = 0; i < 10; i = i + 1) {
                        if (i == 5) {
                            return i;
                        }
                    }

                    return -1;
                }

                fn main(): int {
                    let loop = return_from_loop();
                    printi(loop);
                    return 0;
                }
                ", [], "5"
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
            {
                @"
                fn main(): int {
                    for (let i = 0; i < 10; i = i + 1) {
                        if (i == 5) { break; }
                        if (i == 0) { continue; }
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
                        if (i == 5) { break; }
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
                    for (let i = 0; i < 3; i = i + 1) {
                        for (let j = 0; j < 5; j = j + 1) {
                            if (j == 2) { break; }
                            printi(j);
                        }
                    }
                    return 0;
                }
                ", [], "010101"
            },
            {
                @"
                fn main(): int {
                    let i = 0;
                    while (i < 2) {
                        let j = 0;
                        while (j < 4) {
                            j = j + 1;
                            if (j == 2) { continue; }
                            printi(j);
                        }
                        i = i + 1;
                    }
                    return 0;
                }
                ", [], "134134"
            },
            {
                @"
                fn main(): int {
                    let i = 0;
                    for (i = 0; i < 3; i = i + 1) {
                        printi(i);
                    }
                    printi(i);
                    return 0;
                }
                ", [], "0123"
            },
            {
                @"
                fn main(): int {
                    let i = 0;
                    while (i < 5) {
                        i = i + 1;
                    }
                    printi(i);
                    return 0;
                }
                ", [], "5"
            },
            {
                @"
                fn main(): int {
                    let i = 0;
                    while (true) {
                        if (i == 3) { break; }
                        printi(i);
                        i = i + 1;
                    }
                    return 0;
                }
                ", [], "012"
            },
            {
                @"
                fn main(): int {
                    for (let i = 5; i > 0; i = i - 1) {
                        printi(i);
                    }
                    return 0;
                }
                ", [], "54321"
            },
            {
                @"
                fn main(): int {
                    let found = 0;
                    for (let i = 1; i <= 3; i = i + 1) {
                        for (let j = 1; j <= 3; j = j + 1) {
                            if (i * j == 6) {
                                found = 1;
                                break;
                            }
                        }
                        if (found == 1) { break; }
                    }
                    printi(found);
                    return 0;
                }
                ", [], "1"
            },
            {
                @"
                fn main(): int {
                    for (let i = 0; i < 6; i = i + 1) {
                        if (i % 2 == 0) { continue; }
                        printi(i);
                    }
                    return 0;
                }
                ", [], "135"
            },
            {
                @"
                fn main(): int {
                    let i = 0;
                    while (i < 2) {
                        for (let j = 0; j < 2; j = j + 1) {
                            printi(j);
                        }
                        i = i + 1;
                    }
                    return 0;
                }
                ", [], "0101"
            },
            {
                @"
                fn main(): int {
                    let i = 0;
                    while (i < 0) {
                        printi(42);
                    }
                    for (let j = 10; j < 5; j = j + 1) {
                        printi(42);
                    }
                    printi(0);
                    return 0;
                }
                ", [], "0"
            },
            {
                @"
                fn main(): int {
                    let i = 0;
                    while (i < 5) {
                        i = i + 1;
                        if (i == 2) { continue; }
                        printi(i);
                    }
                    return 0;
                }
                ", [], "1345"
            },
            {
                @"
                fn main(): int {
                    for (let i = 0; i < 5; i = i + 1) {
                        if (i == 0) { continue; }
                        if (i == 2) { continue; }
                        if (i == 4) { continue; }
                        printi(i);
                    }
                    return 0;
                }
                ", [], "13"
            },
            {
                @"
                fn main(): int {
                    let result = 0;
                    for (let i = 0; i < 2; i = i + 1) {
                        for (let j = 0; j < 2; j = j + 1) {
                            for (let k = 0; k < 3; k = k + 1) {
                                result = result + 1;
                                if (k == 1) { break; }
                            }
                        }
                    }
                    printi(result);
                    return 0;
                }
                ", [], "8"
            },
        };
    }
}