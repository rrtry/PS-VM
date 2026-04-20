using Parser;

using Semantics.Exceptions;
using Tests.TestLibrary;

namespace Interpreter.IntegrationTests;

public class BooleanExpressionsTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetBooleanExpressions))]
    public void Can_evaluate_expressions(string code, string expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetInvalidExpressions))]
    public void Reject_invalid_unary_expression(string code, Type exception)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        Assert.Throws(exception, () => interpreter.Execute(code));
    }

    public static TheoryData<string, Type> GetInvalidExpressions()
    {
        return new TheoryData<string, Type>
        {
            {
                @"
                fn main(): int {
                    let i: int = 0;
                    printi(!i);
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    let f: float = 0.0;
                    printf(!f, 2);
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    let s: str = ""str"";
                    print(!s);
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    printb(""Hello"" > ""World"");
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    printb(""Hello"" < ""World"");
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    printb(""Hello"" >= ""World"");
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    printb(""Hello"" <= ""World"");
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    printb(true > false);
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    printb(true < false);
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    printb(true >= false);
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    printb(true <= false);
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    let i: bool = false;
                    printb(!!i);
                    return 0;
                }
                ",
                typeof(UnexpectedLexemeException)
            },
            {
                @"
                fn main(): int {
                    let i: bool = false;
                    printb(!+i);
                    return 0;
                }
                ",
                typeof(UnexpectedLexemeException)
            },
            {
                @"
                fn main(): int {
                    let i: bool = false;
                    printb(!-i);
                    return 0;
                }
                ",
                typeof(UnexpectedLexemeException)
            },
        };
    }

    public static TheoryData<string, string> GetBooleanExpressions()
    {
        return new TheoryData<string, string>
        {
            {
                // Логическое "ИЛИ", "И", "НЕ"
                @"
                fn main(): int {

                    let f: bool = true && false;
                    let t: bool = true || false;

                    printb(true == true);
                    printb(true != false);

                    print(""\n"");

                    printb(f); // 0
                    printb(t); // 1

                    print(""\n"");

                    printb(!f); // 1
                    printb(!t); // 0

                    return 0;
                }
                ",
                "TrueTrue\nFalseTrue\nTrueFalse"
            },
            {
                // Операции сравнения (int)
                @"
                fn main(): int {

                    let less = 1 < 0;
                    let greater = 1 > 0;

                    let lessEq = 1 <= 0;
                    let greaterEq = 1 >= 0;

                    let eqFalse = 1 == 0;
                    let neqTrue = 1 != 0;

                    printb(less);
                    printb(greater);
                    printb(lessEq);
                    printb(greaterEq);

                    print(""\n"");
                    printb(eqFalse);
                    printb(neqTrue);

                    return 0;
                }
                ",
                "FalseTrueFalseTrue\nFalseTrue"
            },
            {
                // Операции сравнения (str)
                @"
                fn main(): int {

                    let s1 = ""Hello"";
                    let s2 = ""Hello"";

                    printb(s1 == s2);
                    s2 = ""World"";
                    printb(s1 != s2);

                    return 0;
                }
                ",
                "TrueTrue"
            },
            {
                // Операции сравнения (float)
                @"
                fn main(): int {

                    let euler = 2.718281828459;
                    let pi = 3.14159265358979323846;

                    printb(pi > euler);  // 1
                    printb(pi < euler);  // 0
                    printb(pi >= euler); // 1
                    printb(pi <= euler); // 0
                    printb(euler != pi); // 1
                    printb(euler == pi); // 0
                    printb(euler == euler); // 1
                    printb(pi == pi); // 1

                    return 0;
                }
                ",
                "TrueFalseTrueFalseTrueFalseTrueTrue"
            },
        };
    }
}