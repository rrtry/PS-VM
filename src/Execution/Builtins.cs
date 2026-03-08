using Ast.Declarations;
using Runtime;

using ValueType = Runtime.ValueType;

namespace Execution;

/// <summary>
/// Объект, предоставляющий доступ к встроенным символам языка.
/// </summary>
public class Builtins
{
    public Builtins(IEnvironment environment)
    {
        Functions =
        [
            new(
                "itos",
                [new NativeFunctionParameter("i", ValueType.Int)],
                ValueType.String,
                arguments => new Value(arguments[0].AsLong().ToString())
            ),
            new(
                "ftos",
                [new NativeFunctionParameter("f", ValueType.Float), new NativeFunctionParameter("p", ValueType.Int)],
                ValueType.String,
                arguments => new Value(arguments[0].AsDouble().ToString($"F{arguments[1]}"))
            ),
            new(
                "ftoi",
                [new NativeFunctionParameter("f", ValueType.Float)],
                ValueType.Int,
                arguments =>
                {
                    double d = arguments[0].AsDouble();
                    return new Value((long)d);
                }
            ),
            new(
                "itof",
                [new NativeFunctionParameter("i", ValueType.Int)],
                ValueType.Float,
                arguments =>
                {
                    long l = arguments[0].AsLong();
                    return new Value((double)l);
                }
            ),
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
            ),
            new(
                "strlen",
                [new NativeFunctionParameter("s", ValueType.String)],
                ValueType.Int,
                arguments =>
                {
                    string s = arguments[0].AsString();
                    return new Value(s.Length);
                }
            ),
            new(
                "substr",
                [new NativeFunctionParameter("s", ValueType.String), new NativeFunctionParameter("from", ValueType.Int), new NativeFunctionParameter("to", ValueType.Int)],
                ValueType.String,
                arguments =>
                {
                    string s = arguments[0].AsString();
                    return new Value(s.Substring((int)arguments[1].AsLong(), (int)arguments[2].AsLong()));
                }
            ),
            new(
                "stoi",
                [new NativeFunctionParameter("s", ValueType.String)],
                ValueType.Int,
                arguments =>
                {
                    string s = arguments[0].AsString();
                    long l = long.Parse(s);
                    return new Value(l);
                }
            ),
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
            ),
            new(
                "input",
                [],
                ValueType.String,
                _ =>
                {
                    string s = environment.Input();
                    return new Value(s);
                }
            ),
            new(
                "printf",
                [ new NativeFunctionParameter("f", ValueType.Float), new NativeFunctionParameter("p", ValueType.Int),],
                ValueType.Void,
                arguments =>
                {
                    environment.Print(arguments[0].AsDouble().ToString($"F{arguments[1]}"));
                    return Value.Unit;
                }
            ),
            new(
                "print",
                [ new NativeFunctionParameter("s", ValueType.String),],
                ValueType.Void,
                arguments =>
                {
                    environment.Print(arguments[0].AsString());
                    return Value.Unit;
                }
            ),
            new(
                "printi",
                [ new NativeFunctionParameter("n", ValueType.Int),],
                ValueType.Void,
                arguments =>
                {
                    environment.Print(arguments[0].AsLong().ToString());
                    return Value.Unit;
                }
            ),
            new(
                "abs",
                [new NativeFunctionParameter("x", ValueType.Int)],
                ValueType.Int,
                (args) =>
                {
                    long l = Math.Abs(args[0].AsLong());
                    return new Value(l);
                }
            ),
            new(
                "sqrt",
                [new NativeFunctionParameter("x", ValueType.Float)],
                ValueType.Float,
                (args) =>
                {
                    double r = Math.Sqrt(args[0].AsDouble());
                    return new Value(r);
                }
            ),
            new(
                "pow",
                [new NativeFunctionParameter("x", ValueType.Int), new NativeFunctionParameter("y", ValueType.Int)],
                ValueType.Int,
                (args) =>
                {
                    long l = (long)Math.Pow(args[0].AsLong(), args[1].AsLong());
                    return new Value(l);
                }
            ),
            new(
                "min",
                [new NativeFunctionParameter("x", ValueType.Int), new NativeFunctionParameter("y", ValueType.Int)],
                ValueType.Int,
                (args) =>
                {
                    long l = Math.Min(args[0].AsLong(), args[1].AsLong());
                    return new Value(l);
                }
            ),
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
        ];

        Types =
        [
            new("int", ValueType.Int),
            new("float", ValueType.Float),
            new("str", ValueType.String),
            new("void", ValueType.Void)
        ];
    }

    /// <summary>
    /// Список встроенных функций языка.
    /// </summary>
    public IReadOnlyList<NativeFunction> Functions { get; }

    /// <summary>
    /// Список встроенных типов языка.
    /// </summary>
    public IReadOnlyList<BuiltinType> Types { get; }
}