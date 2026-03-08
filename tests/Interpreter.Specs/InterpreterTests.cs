using Execution;
using Semantics.Exceptions;
using Xunit.Sdk;

namespace Interpreter.Specs;

public class InterpreterTests
{
    [Theory]
    [MemberData(nameof(GetSingleTypePrograms))]
    [MemberData(nameof(GetMixedTypePrograms))]
    public void Can_interpret_simple_programs(string source, Tuple<List<string>, List<string>> tuple)
    {
        List<string> programInput = tuple.Item1;
        List<string> expectedOutput = tuple.Item2;

        FakeEnvironment environment = new FakeEnvironment();
        Context context = new Context(environment);
        environment.SetProgramInput(programInput);

        List<string> evaluated = environment.GetEvaluated();
        Interpreter interpreter = new Interpreter(context, environment);
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

    public List<string> RunProgram(string source, List<string>? input = null)
    {
        FakeEnvironment environment = new FakeEnvironment();
        Context context = new Context(environment);

        if (input != null)
        {
            environment.SetProgramInput(input);
        }

        Interpreter interpreter = new Interpreter(context, environment);
        interpreter.Execute(source);
        return environment.GetEvaluated();
    }

    [Fact]
    public void Incorrect_return_type_in_function()
    {
        string src = @"
            fn add(a: int, b: int): int {
                return a + b + 0.5;
            }
            let a: int = add(1, 2);
            printi(a);
        ";

        Assert.ThrowsAny<TypeErrorException>(() => RunProgram(src));
    }

    [Fact]
    public void Unreachable_return_statement()
    {
        string src = @"
            fn add(a: int, b: int): int {
                if (a > b) {
                    return a + b;
                }
            }
            let a: int = add(1, 2);
            printi(a);
        ";

        Assert.ThrowsAny<TypeErrorException>(() => RunProgram(src));
    }

    [Fact]
    public void Assignment_incorrect_type_float()
    {
        string src = @"
            let x = 1;
            x = 2.0;
        ";

        Assert.ThrowsAny<TypeErrorException>(() => RunProgram(src));
    }

    [Fact]
    public void Declaration_incorrect_type_float()
    {
        string src = @"
            let x: int = 1.0;
        ";

        Assert.ThrowsAny<TypeErrorException>(() => RunProgram(src));
    }

    [Fact]
    public void Declaration_incorrect_type_string()
    {
        string src = @"
            let x: int = ""1.0"";
        ";

        Assert.ThrowsAny<TypeErrorException>(() => RunProgram(src));
    }

    [Fact]
    public void Nested_function_declaration()
    {
        string src = @"
            fn add(a: int, b: int): int {
                fn subtract(c: int, d: int): int {
                    return c - d;
                }
                return a + b;
            }
            let a: int = add(1, 2);
            printi(a);
        ";

        Assert.ThrowsAny<InvalidExpressionException>(() => RunProgram(src));
    }

    [Fact]
    public void Break_in_function_body()
    {
        string src = @"
            fn add(a: int, b: int): int {
                let c = a + b;
                break;
                return c;
            }
            let a: int = add(1, 2);
            printi(a);
        ";

        Assert.ThrowsAny<Exception>(() => RunProgram(src));
    }

    [Fact]
    public void Function_with_return_type()
    {
        string src = @"
            fn add(a: int, b: int): int {
                return a + b;
            }
            let a: int = add(1, 2);
            printi(a);
        ";

        List<string> outp = RunProgram(src);
        Assert.Equal(new List<string> { "3" }, outp);
    }

    [Fact]
    public void Function_without_return_type()
    {
        string src = @"
            fn write(a: int, b: int) {
                printi(a + b);
            }
            write(1, 2);
        ";

        List<string> outp = RunProgram(src);
        Assert.Equal(new List<string> { "3" }, outp);
    }

    [Fact]
    public void Local_variable_not_accessible_outside_function()
    {
        string src = @"
            fn f(): int {
                let a = 5;
                return 0;
            }
            f();
            printi(a);
        ";

        Assert.ThrowsAny<Exception>(() => RunProgram(src));
    }

    [Fact]
    public void Parameter_can_be_shadowed_by_local_variable()
    {
        string src = @"
            fn f(a: int): int {
                let a = a + 1;
                printi(a);
                return 0;
            }
            f(1);
        ";

        Assert.ThrowsAny<Exception>(() => RunProgram(src));
    }

    [Fact]
    public void Parameter_assignment_semantics_value_vs_reference()
    {
        string src = @"
            fn inc(a: int): int {
                a = a + 1;
                return 0;
            }
            let x = 1;
            inc(x);
            printi(x);
        ";

        List<string> outp = RunProgram(src);
        Assert.Equal(new List<string> { "1" }, outp);
    }

    [Fact]
    public void Argument_count_mismatch_raises_error()
    {
        string src1 = @"
            fn f(a: int): int { printi(a); return 0; }
            f();
        ";

        string src2 = @"
            fn f(a: int): int { printi(a); return 0; }
            f(1, 2);
        ";

        Assert.ThrowsAny<Exception>(() => RunProgram(src1));
        Assert.ThrowsAny<Exception>(() => RunProgram(src2));
    }

    [Fact]
    public void Evaluation_order_of_actual_parameters_left_to_right()
    {
        string src = @"
            fn f(a: int, b: int): int { printi(a); printi(b); return 0; }
            f(stoi(input()), stoi(input()));
        ";

        List<string> outp = RunProgram(src, new List<string> { "10", "20" });
        Assert.Equal(new List<string> { "10", "20" }, outp);
    }

    [Fact]
    public void Recursion_and_mutual_recursion()
    {
        string fact = @"
            fn fact(n: int): int {
                if (n == 0) { return 1; }
                return n * fact(n - 1);
            }
            printi(fact(6));
        ";

        List<string> outpFact = RunProgram(fact);
        Assert.Equal(new List<string> { "720" }, outpFact);

        string mutual = @"
            fn even(n: int): int {
                if (n == 0) { return 1; }
                return odd(n - 1);
            }
            fn odd(n: int): int {
                if (n == 0) { return 0; }
                return even(n - 1);
            }
            printi(even(4));
            printi(odd(4));
        ";

        Assert.ThrowsAny<Exception>(() => RunProgram(mutual));
    }

    [Fact]
    public void Condition_truthiness_zero_is_false_nonzero_true()
    {
        string src = @"
            if (0) { printi(1); } else { printi(2); }
            if (1) { printi(3); } else { printi(4); }
        ";

        List<string> outp = RunProgram(src);
        Assert.Equal(new List<string> { "2", "3" }, outp);
    }

    [Fact]
    public void While_accepts_integer_condition_and_break_continue_positions()
    {
        string src = @"
            let i = 0;
            while (i < 5) {
                i = i + 1;
                if (i == 3) { continue; }
                if (i == 5) { break; }
                printi(i);
            }
        ";

        List<string> outp = RunProgram(src);
        Assert.Equal(new List<string> { "1", "2", "4" }, outp);
    }

    [Fact]
    public void Invalid_assignment_targets_raise_error()
    {
        string src1 = @"1 = 2;";
        string src2 = @"print() = 1;";

        Assert.ThrowsAny<Exception>(() => RunProgram(src1));
        Assert.ThrowsAny<Exception>(() => RunProgram(src2));
    }

    [Fact]
    public void Using_uninitialized_variable_raises_error()
    {
        string src = @"
            let x: int;
            printi(x);
        ";

        Assert.ThrowsAny<Exception>(() => RunProgram(src));
    }

    public static TheoryData<string, Tuple<List<string>, List<string>>> GetMixedTypePrograms()
    {
        return new TheoryData<string, Tuple<List<string>, List<string>>>
        {
            {
                @"fn is_vowel(ch: str): int
                {
                    if (ch == ""A"" || ch == ""E"" || ch == ""I"" || ch == ""O"" || ch == ""U"" || ch == ""Y"") 
                    {
                        return 1;
                    }
                    if (ch == ""a"" || ch == ""e"" || ch == ""i"" || ch == ""o"" || ch == ""u"" || ch == ""y"") 
                    {
                        return 1;
                    }
                    return 0;
                }

                fn count_vowels(s: str): int 
                {
                    let count = 0;
                    let len = strlen(s);
                    let i = 0;

                    while (i < len) 
                    {
                        let sub = substr(s, i, 1);
                        if (is_vowel(sub)) 
                        {
                            count = count + 1;
                        }
                        i = i + 1;
                    }
                    return count;
                }

                let text = input();
                let vowels = count_vowels(text);
                print(itos(vowels));",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "Hello" },
                    new List<string> { "2" }
                )
            },
            {
                @"
                fn reverse(s: str): str
                {
                    let len = strlen(s);
                    let result = """";

                    let i = len - 1;
                    while (i >= 0) 
                    {
                        let ch = substr(s, i, 1);
                        result = sconcat(result, ch);
                        i = i - 1;
                    }

                    return result;
                }

                let text = input();
                let reversed = reverse(text);
                print(reversed);
                ",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "123" },
                    new List<string> { "321" }
                )
            },
            {
                @"
                let x = 1;
                while (x) 
                {
                    let n = stoi(input());
                    if (n == 0) 
                    {
                        break;
                    }

                    if (n % 15 == 0) 
                    {
                        print(""FizzBuzz"");
                        continue;
                    }
                    if (n % 3 == 0) 
                    {
                        print(""Fizz"");
                        continue;
                    }
                    if (n % 5 == 0) 
                    {
                        print(""Buzz"");
                        continue;
                    }
                    print(itos(n));
                }
                ",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "15", "3", "5", "0" },
                    new List<string> { "FizzBuzz", "Fizz", "Buzz" }
                )
            },
        };
    }

    public static TheoryData<string, Tuple<List<string>, List<string>>> GetSingleTypePrograms()
    {
        return new TheoryData<string, Tuple<List<string>, List<string>>>
        {
            {
                @"
                let x: float = stof(input());
                let y: float = stof(input());
                let z: float = stof(input());

                fn solve(a: float, b: float, c: float): int
                {
                    if (a == 0) 
                    {
                        if (b != 0) 
                        {
                            let root1 = -c / b;
                            printf(root1, 2);
                            return 1;
                        }
                    }
                    else 
                    {
                        let disc = b * b - 4 * a * c;
                        if (disc > 0) 
                        {
                            let sqrt_disc = sqrt(disc);
                            let root1 = (-b + sqrt_disc) / (2 * a);
                            let root2 = (-b - sqrt_disc) / (2 * a);
                            printf(root1, 2);
                            printf(root2, 2);
                            return 2;
                        }
                        if (disc == 0) 
                        {
                            let root1 = -b / (2 * a);
                            printf(root1, 2);
                            return 1;
                        }
                    }
                    return -1;
                }
                let result = solve(x, y, z);
                printi(result);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "2", "3", "-2" },
                    new List<string> { "0.50", "-2.00", "2" }
                )
            },
            {
                @"fn is_prime(n: int): int {
                    if (n < 2) 
                    {
                        return 0;
                    }
                    if (n == 2) 
                    {
                        return 1;
                    }

                    let limit = sqrt(itof(n));
                    let i = 3;
                    while (i <= limit) 
                    {
                        if (n % i == 0) 
                        {
                            return 0;
                        }
                        i = i + 2;
                    }
                    return 1;
                }
                printi(is_prime(139));
                ",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "1" }
                )
            },
            {
                @"fn factorial(n: int): int {
                    let fact = 1;
                    for (let i: int = 1; i <= n; i = i + 1) 
                    {
                        fact = fact * i;
                    }
                    return fact;
                }
                printi(factorial(5));",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "120" }
                )
            },
            {
                @"fn fibonacci(n: int): int {

                    let prev1 = 1;
                    let prev2 = 0;
                    let curr  = 0;

                    for (let i = 2; i <= n; i = i + 1) {
                        curr = prev1 + prev2;
                        prev2 = prev1;
                        prev1 = curr;
                    }

                    return curr;
                }
                printi(fibonacci(10));",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "55" }
                )
            },
            {
                @"let i = 0;
                  if (!i) {
                    printi(!i);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "1" }
                )
            },
            {
                @"fn add(a: int, b: int): int {
                    return a + b;
                }
                let z = add(2, 3);
                printi(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "5" }
                )
            },
            {
                @"let x = 0;
                  for (let j = 0; j < 5; j = j + 1) {
                      x = x + 1;
                      if (x == 3) {
                        continue;
                      }
                      printi(x);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "1", "2", "4", "5" }
                )
            },
            {
                @"let x = 0;
                  for (let j = 0; j < 10; j = j + 1) {
                      x = x + 1;
                      if (x == 5) {
                          break;
                      }
                  }
                  printi(x);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "5" }
                )
            },
            {
                @"let i = 0;
                  while (i < 10) {
                      let temp = i + 1;
                      i = temp;
                      if (i == 5) {
                          break;
                      }
                  }
                  printi(i);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "5" }
                )
            },
            {
                @"let i = 0;
                  while (i < 5) {
                      i = i + 1;
                      if (i == 3) {
                          continue;
                      }
                      printi(i);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "1", "2", "4", "5" }
                )
            },
            {
                @"let x = 4;
                  let y = 2;
                  if (x + y < x) {
                      let z = 2;
                      printi((x + y) * z);
                  } else {
                    printi(x - y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "2" }
                )
            },
            {
                @"let x = 6;
                  let y = 2;
                  if ((x + y) == 8 && pow(2, 3) == 8) {
                      printi(x + y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "8" }
                )
            },
            {
                @"let x = 6;
                  let y = 2;
                  if ((x + y) == 8 || (x - y) == 2) {
                      printi(x + y);
                  } else {
                    printi(x - y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "8" }
                )
            },
            {
                @"let x = 2;
                  let y = 2;
                  if (x == y) {
                      printi(x + y);
                  } else {
                      printi(x - y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "4" }
                )
            },
            {
                @"let x = 2;
                  let y = 2;
                  if (x != y) {
                      printi(x + y);
                  } else {
                      printi(x - y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "0" }
                )
            },
            {
                @"let x = 1;
                  let y = 2;
                  if (x < y) {
                      printi(x + y);
                  } else {
                      printi(x - y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "3" }
                )
            },
            {
                @"let x = 1;
                  let y = 2;
                  if (x > y) {
                      printi(x + y);
                  } else {
                      printi(x - y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "-1" }
                )
            },
            {
                @"let x = 2;
                  let y = 1;
                  if (x > y) {
                      printi(x + y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "3" }
                )
            },
            {
                @"let x = 2;
                  let y = 1;
                  if (x < y) {
                      printi(x + y);
                  }",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { }
                )
            },
            {
                "let x = 1;" +
                "let y = 2;" +
                "let z = y > x;" +
                "printi(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "1" }
                )
            },
            {
                "let x = 1;" +
                "let y = 2;" +
                "let z = y < x;" +
                "printi(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "0" }
                )
            },
            {
                "let x = 1;" +
                "let y = 2;" +
                "let z = y <= x;" +
                "printi(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "0" }
                )
            },
            {
                "let x = 1;" +
                "let y = 2;" +
                "let z = y >= x;" +
                "printi(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { },
                    new List<string> { "1" }
                )
            },
            {
                "let x = stoi(input());" +
                "let y = stoi(input());" +
                "let z = x != y;" +
                "printi(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "2" },
                    new List<string> { "1" }
                )
            },
            {
                "let x = stoi(input());" +
                "let y = stoi(input());" +
                "let z = x == y;" +
                "printi(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "1" },
                    new List<string> { "1" }
                )
            },
            {
                "let x = stoi(input());" +
                "let y = stoi(input());" +
                "let z = x != y && x == 0;" +
                "printi(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "2" },
                    new List<string> { "0" }
                )
            },
            {
                "let x = stoi(input());" +
                "let y = stoi(input());" +
                "let z = x != y || x == 0;" +
                "printi(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "2" },
                    new List<string> { "1" }
                )
            },
            {
                "let x = stoi(input());" +
                "let y = stoi(input());" +
                "let z = (x != y) || (x == 0);" +
                "printi(z);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "2" },
                    new List<string> { "1" }
                )
            },
            {
                "let x = stoi(input()); " +
                "let y = stoi(input()); " +
                "printi(x + y);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "2" },
                    new List<string> { "3" }
                )
            },
            {
                "let x = stoi(input()); " +
                "let y = stoi(input()); " +
                "y = 3; " +
                "printi(x + y);",
                new Tuple<List<string>, List<string>>(
                    new List<string> { "1", "2" },
                    new List<string> { "4" }
                )
            },
        };
    }
}