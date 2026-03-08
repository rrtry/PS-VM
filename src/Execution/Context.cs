namespace Execution;

using Ast.Declarations;
using Runtime;

using ValueType = Runtime.ValueType;

/// <summary>
/// Контекст выполнения программы (все переменные, константы и другие символы).
/// </summary>
public class Context
{
    private readonly IEnvironment environment;
    private readonly Stack<Scope> scopes = [];
    private readonly Dictionary<string, AbstractFunctionDeclaration> functions = [];
    private readonly Dictionary<string, NativeFunction> nativeFunctions;

    public Context(IEnvironment environment)
    {
        scopes.Push(new Scope());
        this.environment = environment;
        this.nativeFunctions = new Dictionary<string, NativeFunction>
        {
            {
                "itos",
                new(
                    "itos",
                    [new NativeFunctionParameter("i", ValueType.Int)],
                    ValueType.String,
                    arguments => new Value(arguments[0].AsLong().ToString())
                )
            },
            {
                "ftos",
                new(
                    "ftos",
                    [new NativeFunctionParameter("f", ValueType.Float), new NativeFunctionParameter("p", ValueType.Int)],
                    ValueType.String,
                    arguments => new Value(arguments[0].AsDouble().ToString($"F{arguments[1]}"))
                )
            },
            {
                "ftoi",
                new(
                    "ftoi",
                    [new NativeFunctionParameter("f", ValueType.Float)],
                    ValueType.Int,
                    arguments =>
                    {
                        double d = arguments[0].AsDouble();
                        return new Value((long)d);
                    }
                )
            },
            {
                "itof",
                new(
                    "itof",
                    [new NativeFunctionParameter("i", ValueType.Int)],
                    ValueType.Float,
                    arguments =>
                    {
                        long l = arguments[0].AsLong();
                        return new Value((double)l);
                    }
                )
            },
            {
                "strconcat",
                new(
                    "sconcat",
                    [new NativeFunctionParameter("s1", ValueType.String), new NativeFunctionParameter("s2", ValueType.String)],
                    ValueType.String,
                    arguments =>
                    {
                        string s1 = arguments[0].AsString();
                        string s2 = arguments[1].AsString();
                        return new Value(s1 + s2);
                    }
                )
            },
            {
                "strlen",
                new(
                    "strlen",
                    [new NativeFunctionParameter("s", ValueType.String)],
                    ValueType.Int,
                    arguments =>
                    {
                        string s = arguments[0].AsString();
                        return new Value(s.Length);
                    }
                )
            },
            {
                "substr",
                new(
                    "substr",
                    [new NativeFunctionParameter("s", ValueType.String), new NativeFunctionParameter("from", ValueType.Int), new NativeFunctionParameter("to", ValueType.Int)],
                    ValueType.String,
                    arguments =>
                    {
                        string s = arguments[0].AsString();
                        return new Value(s.Substring((int)arguments[1].AsLong(), (int)arguments[2].AsLong()));
                    }
                )
            },
            {
                "stoi",
                new(
                    "int",
                    [new NativeFunctionParameter("s", ValueType.String)],
                    ValueType.Int,
                    arguments =>
                    {
                        string s = arguments[0].AsString();
                        long l = long.Parse(s);
                        return new Value(l);
                    }
                )
            },
            {
                "stof",
                new(
                    "stof",
                    [new NativeFunctionParameter("s", ValueType.String)],
                    ValueType.Float,
                    arguments =>
                    {
                        string s = arguments[0].AsString();
                        double d = double.Parse(s);
                        return new Value(d);
                    }
                )
            },
            {
                "input",
                new(
                    "input",
                    [],
                    ValueType.String,
                    _ =>
                    {
                        string s = environment.Input();
                        return new Value(s);
                    }
                )
            },
            {
                "printf",
                new(
                    "printf",
                    [ new NativeFunctionParameter("f", ValueType.Float), new NativeFunctionParameter("p", ValueType.Int),],
                    ValueType.Void,
                    arguments =>
                    {
                        environment.Print(arguments[0].AsDouble().ToString($"F{arguments[1]}"));
                        return Value.Unit;
                    }
                )
            },
            {
                "print",
                new(
                    "print",
                    [ new NativeFunctionParameter("s", ValueType.String),],
                    ValueType.Void,
                    arguments =>
                    {
                        environment.Print(arguments[0].AsString());
                        return Value.Unit;
                    }
                )
            },
            {
                "printi",
                new(
                    "printi",
                    [ new NativeFunctionParameter("n", ValueType.Int),],
                    ValueType.Void,
                    arguments =>
                    {
                        if (arguments[0].IsDouble())
                        {
                            environment.Print(arguments[0].AsDouble().ToString("F2"));
                            return Value.Unit;
                        }

                        environment.Print(arguments[0].AsLong().ToString());
                        return Value.Unit;
                    }
                )
            },
            {
                "abs",
                new(
                    "abs",
                    [new NativeFunctionParameter("x", ValueType.Int)],
                    ValueType.Int,
                    (args) =>
                    {
                        long l = Math.Abs(args[0].AsLong());
                        return new Value(l);
                    }
                )
            },
            {
                "sqrt",
                new(
                    "sqrt",
                    [new NativeFunctionParameter("x", ValueType.Float)],
                    ValueType.Float,
                    (args) =>
                    {
                        double r = Math.Sqrt(args[0].AsDouble());
                        return new Value(r);
                    }
                )
            },
            {
                "pow",
                new(
                    "pow",
                    [new NativeFunctionParameter("x", ValueType.Int), new NativeFunctionParameter("y", ValueType.Int)],
                    ValueType.Int,
                    (args) =>
                    {
                        long l = (long)Math.Pow(args[0].AsLong(), args[1].AsLong());
                        return new Value(l);
                    }
                )
            },
            {
                "min",
                new(
                    "min",
                    [new NativeFunctionParameter("x", ValueType.Int), new NativeFunctionParameter("y", ValueType.Int)],
                    ValueType.Int,
                    (args) =>
                    {
                        long l = Math.Min(args[0].AsLong(), args[1].AsLong());
                        return new Value(l);
                    }
                )
            },
            {
                "max",
                new(
                    "max",
                    [new NativeFunctionParameter("x", ValueType.Int), new NativeFunctionParameter("y", ValueType.Int)],
                    ValueType.Int,
                    (args) =>
                    {
                        long l = Math.Max(args[0].AsLong(), args[1].AsLong());
                        return new Value(l);
                    }
                )
            },
        };
    }

    public AbstractFunctionDeclaration TryGetFunction(string name)
    {
        if (nativeFunctions.TryGetValue(name, out NativeFunction? nativeFunction))
        {
            return nativeFunction;
        }

        if (functions.TryGetValue(name, out AbstractFunctionDeclaration? function))
        {
            return function;
        }

        throw new ArgumentException($"Function '{name}' is not defined");
    }

    public AbstractFunctionDeclaration GetFunction(string name)
    {
        if (functions.TryGetValue(name, out AbstractFunctionDeclaration? function))
        {
            return function;
        }

        throw new ArgumentException($"Function '{name}' is not defined");
    }

    public void DefineFunction(AbstractFunctionDeclaration function)
    {
        if (!functions.TryAdd(function.Name, function))
        {
            throw new ArgumentException($"Function '{function.Name}' is already defined");
        }
    }

    public void PushScope(Scope scope)
    {
        scopes.Push(scope);
    }

    public void PopScope()
    {
        scopes.Pop();
    }

    /// <summary>
    /// Возвращает значение переменной или константы.
    /// </summary>
    public Value GetValue(string name)
    {
        foreach (Scope s in scopes)
        {
            if (s.TryGetVariable(name, out Value? variable))
            {
                return variable!;
            }
        }

        throw new ArgumentException($"Variable '{name}' is not defined");
    }

    /// <summary>
    /// Присваивает (изменяет) значение переменной.
    /// </summary>
    public void AssignVariable(string name, Value value)
    {
        foreach (Scope s in scopes.Reverse())
        {
            if (s.TryAssignVariable(name, value))
            {
                return;
            }
        }

        throw new ArgumentException($"Variable '{name}' is not defined");
    }

    /// <summary>
    /// Определяет переменную в текущей области видимости.
    /// </summary>
    public void DefineVariable(string name, Value value)
    {
        if (!scopes.Peek().TryDefineVariable(name, value))
        {
            throw new ArgumentException($"Variable '{name}' is already defined in this scope");
        }
    }
}