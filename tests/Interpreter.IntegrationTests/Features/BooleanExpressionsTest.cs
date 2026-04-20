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
                    let f = 3.14;
                    printf(!f, 2);
                    return 0;
                }
                ",
                typeof(TypeErrorException)
            },
            {
                @"
                fn main(): int {
                    let i = 0;
                    printi(!!i);
                    return 0;
                }
                ",
                typeof(UnexpectedLexemeException)
            },
            {
                @"
                fn main(): int {
                    let i = 0;
                    printi(!+i);
                    return 0;
                }
                ",
                typeof(UnexpectedLexemeException)
            },
            {
                @"
                fn main(): int {
                    let i = 0;
                    printi(!-i);
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

                    let f = 1 && 0;
                    let t = 1 || 0;

                    printi(f); // 0
                    printi(t); // 1

                    printi(!f); // 1
                    printi(!t); // 0

                    printi(!256); // != 0 -> true
                    printi(!0); // == 0 -> false

                    return 0;
                }
                ",
                "011001"
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

                    printi(less);
                    printi(greater);
                    printi(lessEq);
                    printi(greaterEq);

                    print(""\n"");
                    printi(eqFalse);
                    printi(neqTrue);

                    return 0;
                }
                ",
                "0101\n01"
            },
            {
                // Операции сравнения (str)
                @"
                fn main(): int {

                    let s1 = ""Hello"";
                    let s2 = ""Hello"";

                    printi(s1 == s2);
                    s2 = ""World"";
                    printi(s1 != s2);

                    return 0;
                }
                ",
                "11"
            },
            {
                // Операции сравнения (float)
                @"
                fn main(): int {

                    let euler = 2.718281828459;
                    let pi = 3.14159265358979323846;

                    printi(pi > euler);  // 1
                    printi(pi < euler);  // 0
                    printi(pi >= euler); // 1
                    printi(pi <= euler); // 0
                    printi(euler != pi); // 1
                    printi(euler == pi); // 0
                    printi(euler == euler); // 1
                    printi(pi == pi); // 1

                    return 0;
                }
                ",
                "10101011"
            },
        };
    }
}