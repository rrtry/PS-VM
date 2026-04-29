using Semantics.Exceptions;

using Tests.TestLibrary;

namespace Interpreter.IntegrationTests;

public class ControlFlowTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetControlFlowStatements))]
    public void Can_exec_control_flow_statements(string code, List<string> input, string expected)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        input.ForEach(environment.AddInput);

        interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetControlFlowErrors))]
    public void Can_handle_control_flow_errors(string code, List<string> input, Type exception)
    {
        FakeEnvironment environment = new();
        input.ForEach(environment.AddInput);

        Interpreter interpreter = new(environment);
        Assert.Throws(exception, () => interpreter.Execute(code));
    }

    public static TheoryData<string, List<string>, Type> GetControlFlowErrors()
    {
        return new TheoryData<string, List<string>, Type>
        {
            {
                // Неверное сравнение типов (str с int)
                @"fn main(): int {
                    let x = 10;
                    let s = ""10"";
                    if (x == s) {
                        print(""equal"");
                    }
                    return 0;
                }",
                [],
                typeof(TypeErrorException)
            },
            {
                // Неверное сравнение типов (float с int)
                @"fn main(): int {
                    let x = 10;
                    let f = 10.0;
                    if (x == f) {
                        print(""equal"");
                    }
                    return 0;
                }",
                [],
                typeof(TypeErrorException)
            },
            {
                // Неверное сравнение типов (float с str)
                @"fn main(): int {
                    let f = 10.0;
                    let s = ""10.0"";
                    if (f == s) {
                        print(""equal"");
                    }
                    return 0;
                }",
                [],
                typeof(TypeErrorException)
            },
            {
                // Условие не булевого типа (int)
                @"fn main(): int {
                    let x = 42;
                    if (x) {
                        print(""true"");
                    }
                    return 0;
                }",
                [],
                typeof(TypeErrorException)
            },
            {
                // Условие не булевого типа (float)
                @"fn main(): int {
                    let x = 42.0;
                    if (x) {
                        print(""true"");
                    }
                    return 0;
                }",
                [],
                typeof(TypeErrorException)
            },
            {
                // Условие не булевого типа (str)
                @"fn main(): int {
                    let s = ""hello"";
                    if (s) {
                        print(""true"");
                    }
                    return 0;
                }",
                [],
                typeof(TypeErrorException)
            },
            {
                // Недостижимый код
                @"fn main(): int {

                    let in = input();

                    if (in == ""True"") {
                        print(""Then branch\n"");
                        return 0;
                    } else {
                        print(""Else branch\n"");
                        return 0;
                    }

                    // dead code
                    in = input();
                    if (in == ""True"") {
                        print(""Then branch\n"");
                        return 0;
                    } else {
                        print(""Else branch\n"");
                        return 0;
                    }
                }",
                ["True"],
                typeof(UnreachableCodeException)
            },
            {
                // Отсутствие return в одной из ветвей (функция возвращает int)
                @"fn main(): int {
                    let x = stoi(input());
                    if (x > 0) {
                        return 1;
                    }
                    // нет return для x <= 0
                }",
                ["-1"],
                typeof(TypeErrorException)
            },
            {
                // Отсутствие return в else-ветви (при наличии return в then)
                @"fn main(): int {
                    let x = stoi(input());
                    if (x > 0) {
                        return 1;
                    } else {
                        print(""no return"");
                    }
                }",
                ["-1"],
                typeof(TypeErrorException)
            },
            {
                // Попытка использовать переменную, объявленную внутри if, снаружи
                @"fn main(): int {
                    if (true) {
                        let inside = 100;
                    }
                    printi(inside);  // inside не видна
                    return 0;
                }",
                [],
                typeof(UnknownSymbolException)
            },
            {
                // Попытка использовать переменную, объявленную в else, после if
                @"fn main(): int {
                    if (false) {
                        let x = 1;
                    } else {
                        let y = 2;
                    }
                    printi(y);  // y не видна за пределами else
                    return 0;
                }",
                [],
                typeof(UnknownSymbolException)
            },
            {
                // return не на всех путях выполнения
                @"fn main(): int {

                    let a = true;
                    let b = false;

                    if (a) {
                        if (b) {
                            printi(1);
                            return 1;
                        }
                        // return 1
                    } else {
                        printi(3);
                        return 3;
                    }
                    // return 1
                }",
                [],
                typeof(TypeErrorException)
            },
        };
    }

    public static TheoryData<string, List<string>, string> GetControlFlowStatements()
    {
        return new TheoryData<string, List<string>, string>
        {
            {
                // Вложенные if, все ветви возвращают значение
                @"fn main(): int {
                    let x = stoi(input());
                    if (x > 0) {
                        print(""positive"");
                        return 1;
                    } else {
                        if (x < 0) {
                            print(""negative"");
                            return -1;
                        } else {
                            print(""zero"");
                            return 0;
                        }
                    }
                }",
                ["-5"],
                "negative"
            },
            {
                // Логические операторы &&, ||, !
                @"fn main(): int {
                    let a = input() == ""true"";
                    let b = input() == ""true"";
                    if (a && b) {
                        print(""both true"");
                    } else {
                        if (a || b) {
                            print(""at least one true"");
                        } else {
                            print(""none are true"");
                        }
                    }
                    return 0;
                }",
                ["true", "false"],
                "at least one true"
            },
            {
                // Числовые сравнения
                @"fn main(): int {
                    let x = stoi(input());
                    if (x >= 10) {
                        print(""big"");
                    } else {
                        if (x > 5) {
                            print(""medium"");
                        } else {
                            print(""small"");
                        }
                    }
                    return 0;
                }",
                ["7"],
                "medium"
            },
            {
                // Сравнение строк
                @"fn main(): int {
                    let s = input();
                    if (s == ""Hello"") {
                        print(""Greeting"");
                    } else {
                        print(""Other"");
                    }
                    return 0;
                }",
                ["hello"],
                "Other"
            },
            {
                // Пустой then-блок
                @"fn main(): int {
                    let x = stoi(input());
                    if (x > 0) {
                        // пусто
                    } else {
                        print(""not positive"");
                    }
                    return 0;
                }",
                ["5"],
                ""
            },
            {
                // Пустой else-блок
                @"fn main(): int {
                    let x = stoi(input());
                    if (x > 0) {
                        print(""positive"");
                    } else {
                        // пусто
                    }
                    return 0;
                }",
                ["-3"],
                ""
            },
            {
                // Досрочный return внутри if – остальной код main не выполняется
                @"fn main(): int {
                    let b = input() == ""True"";
                    if (b) {
                        print(""return from if"");
                        return 0;
                    }
                    print(""after if"");
                    return 0;
                }",
                ["True"],
                "return from if"
            },
            {
                // Сложное вложенное условие с разными return
                @"fn main(): int {
                    let a = true;
                    let b = false;
                    let c = true;
                    if (a) {
                        if (b) {
                            print(""ab"");
                            return 1;
                        } else {
                            if (c) {
                                print(""ac"");
                                return 2;
                            }
                        }
                    }
                    print(""default"");
                    return 0;
                }",
                [],
                "ac"
            },
            {
                // Использование переменной, объявленной до if, внутри и после
                @"fn main(): int {
                    let x = 10;
                    if (x > 5) {
                        let y = x + 5;
                        printi(y);
                    } else {
                        printi(x);
                    }
                    // y здесь недоступна, но x доступна
                    printi(x);
                    return 0;
                }",
                [],
                "1510" // y = 15, потом x = 10
            },
            {
                // Проверка выполнения двух ветвей if/else
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
        };
    }
}