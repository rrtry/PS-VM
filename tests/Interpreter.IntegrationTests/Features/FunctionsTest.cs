namespace Interpreter.IntegrationTests;

using Semantics.Exceptions;
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

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetInvalidProgramsWithFunctions))]
    public void Throws_on_invalid_function_usage(string code, Type expectedExceptionType)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        Assert.Throws(expectedExceptionType, () => interpreter.Execute(code));
    }

    public static TheoryData<string, Type> GetInvalidProgramsWithFunctions()
    {
        return new TheoryData<string, Type>
        {
            {
                @"
                fn wrong_return_type(i: int): float {
                    return i;
                }
                fn main(): int {
                    wrong_return_type(1);
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn wrong_return_type(s: str) {
                    return s;
                }
                fn main(): int {
                    wrong_return_type("""");
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn prints(s: str) {
                    print(s);
                }
                fn main(): int {
                    prints(1);
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn prints(s: str, i: int) {
                    print(s);
                }

                fn main(): int {
                    prints("""");
                    return 0;
                }
                ",
                typeof(InvalidFunctionCallException)
            },
            {
                @"
                fn prints(s: str, i: int) {
                    print(s);
                }

                fn main(): int {
                    prints("""", 1, 1);
                    return 0;
                }
                ",
                typeof(InvalidFunctionCallException)
            },
            {
                @"
                fn main(): int {
                    continue; 
                    return 0; 
                }",
                typeof(InvalidStatementException)
            },
            {
                @"
                fn main(): int { 
                    break; 
                    return 0;
                }",
                typeof(InvalidStatementException)
            },
            {
                @"
                fn test(): int { 
                    continue; 
                    return 0;
                } 
                fn main(): int { 
                    return 0; 
                }",
                typeof(InvalidStatementException)
            },
            {
                @"fn main(): int { 
                    if (true) { break; } 
                    return 0;
                }",
                typeof(InvalidStatementException)
            },
            {
                @"
                fn inner(): unit { continue; } 
                fn main(): int { 
                    let i = 0; 
                    while (i < 1) { inner(); i = i + 1; } 
                    return 0;
                }",
                typeof(InvalidStatementException)
            },
        };
    }

    public static TheoryData<string, List<string>, string> GetProgramsWithFunctions()
    {
        return new TheoryData<string, List<string>, string>
        {
            {
                @"
                fn double(x: int): int {
                    x = x * 2; // изменение локальной копии
                    return x;
                }

                fn inc(x: int): int {
                    x = x + 1;
                    return x;
                }

                fn compute(a: int, b: int): int {
                    let da = double(a);
                    let ib = inc(b);
                    return da + ib;
                }

                fn main(): int {
                    let x = 10;
                    let y = 20;
                    let result = compute(x, y);
                    // double(10) = 20, inc(20) = 21, 41
                    printi(result);
                    // x и y не изменились
                    printi(x);
                    printi(y);
                    return 0;
                }
                ", [], "411020"
            },
            {
                @"
                fn multiple_ifs(a: int, b: int): int {
                    if (a > b) {
                        if ((a - b) > 10) {
                            return 1;
                        } else {
                            return 2;
                        }
                    } else {
                        if ((b - a) > 10) {
                            return 3;
                        } else {
                            return 4;
                        }
                    }
                }
                fn main(): int {
                    printi(multiple_ifs(20, 5));  // 20 - 5 = 15 > 10 -> 1
                    printi(multiple_ifs(12, 5));  // 7 не > 10 -> 2
                    printi(multiple_ifs(5, 20));  // 15 > 10 -> 3
                    printi(multiple_ifs(5, 12));  // 7 не > 10 -> 4
                    return 0;
                }
                ", [], "1234"
            },
            {
                @"
                fn nested_blocks(x: int): int {
                    let y = 10;
                    if (x > 0) {
                        let z = 20;
                        return x + y + z;
                    }
                    return x;
                }
                fn main(): int {
                    printi(nested_blocks(5));
                    printi(nested_blocks(-2));
                    return 0;
                }
                ", [], "35-2"
            },
            {
                @"
                fn sum_to_n(n: int): int {
                    if (n <= 0) { return 0; }
                    return n + sum_to_n(n - 1);
                }
                fn main(): int {
                    printi(sum_to_n(30));
                    return 0;
                }
                ", [], "465"
            },
            {
                @"
                fn is_positive(x: int): bool {
                    return x > 0;
                }
                fn main(): int {
                    printb(is_positive(5));
                    printb(is_positive(-3));
                    return 0;
                }
                ", [], "TrueFalse"
            },
            {
                @"
                fn greet_user(name: str): str {
                    return sconcat(""Hello, "", ""Alice!"");
                }
                fn main(): int {
                    print(greet_user(""Alice""));
                    return 0;
                }
                ", [], "Hello, Alice!"
            },
            {
                @"
                fn sign(x: int): int {
                    if (x > 0) { return 1; }
                    if (x < 0) { return -1; }
                    return 0;
                }

                fn main(): int {
                    printi(sign(10));
                    printi(sign(0));
                    printi(sign(-5));
                    return 0;
                }
                ", [], "10-1"
            },
            {
                @"
                fn less_than_ten(n: int): bool {
                    return n < 10;
                }
                fn main(): int {
                    if (less_than_ten(5)) {
                        printi(100);
                    } else {
                        printi(200);
                    }
                    return 0;
                }
                ", [], "100"
            },
            {
                @"
                fn test_shadowing(): int {
                    let x = 1;
                    if (true) {
                        let x = 2;
                        printi(x);
                    }
                    printi(x);
                    return 0;
                }
                fn main(): int {
                    let x = 3;
                    test_shadowing();
                    printi(x);
                    return 0;
                }
                ", [], "213"
            },
            {
                @"
                fn inc(x: int): int {
                    x = x + 1;
                    return x;
                }
                fn main(): int {
                    let a = 10;
                    let b = inc(a);
                    printi(a);   // 10
                    printi(b);   // 11
                    return 0;
                }
                ", [], "1011"
            },
            {
                @"
                fn fib(n: int): int {
                    if (n <= 1) {
                        return n;
                    }
                    return fib(n - 1) + fib(n - 2);
                }

                fn main(): int {
                    printi(fib(7));
                    return 0;
                }
                ", [], "13"
            },
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