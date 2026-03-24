using Ast.Declarations;

using ValueType = Runtime.ValueType;

namespace Ast;

/// <summary>
/// Объект, предоставляющий доступ к встроенным символам языка.
/// </summary>
public static class Builtins
{
    public const string Input = "input";
    public const string Print = "print";
    public const string PrintI = "printi";
    public const string PrintF = "printf";
    public const string ItoS = "itos";
    public const string FtoS = "ftos";
    public const string ItoF = "itof";
    public const string FtoI = "ftoi";
    public const string StoI = "stoi";
    public const string StoF = "stof";
    public const string SConcat = "sconcat";
    public const string SubStr = "substr";
    public const string StrLen = "strlen";

    /// <summary>
    /// Список встроенных функций языка.
    /// </summary>
    public static readonly IReadOnlyList<NativeFunction> Functions =
    [
        new(
            "itos",
            [new NativeFunctionParameter("i", ValueType.Int)],
            ValueType.String
        ),
        new(
            "ftos",
            [new NativeFunctionParameter("f", ValueType.Float), new NativeFunctionParameter("p", ValueType.Int)],
            ValueType.String
        ),
        new(
            "ftoi",
            [new NativeFunctionParameter("f", ValueType.Float)],
            ValueType.Int
        ),
        new(
            "itof",
            [new NativeFunctionParameter("i", ValueType.Int)],
            ValueType.Float
        ),
        new(
            "sconcat",
            [new NativeFunctionParameter("s1", ValueType.String), new NativeFunctionParameter("s2", ValueType.String)],
            ValueType.String
        ),
        new(
            "strlen",
            [new NativeFunctionParameter("s", ValueType.String)],
            ValueType.Int
        ),
        new(
            "substr",
            [new NativeFunctionParameter("s", ValueType.String), new NativeFunctionParameter("from", ValueType.Int), new NativeFunctionParameter("to", ValueType.Int)],
            ValueType.String
        ),
        new(
            "stoi",
            [new NativeFunctionParameter("s", ValueType.String)],
            ValueType.Int
        ),
        new(
            "stof",
            [new NativeFunctionParameter("s", ValueType.String)],
            ValueType.Float
        ),
        new(
            "input",
            [],
            ValueType.String
        ),
        new(
            "printf",
            [ new NativeFunctionParameter("f", ValueType.Float), new NativeFunctionParameter("p", ValueType.Int),],
            ValueType.Unit
        ),
        new(
            "print",
            [ new NativeFunctionParameter("s", ValueType.String),],
            ValueType.Unit
        ),
        new(
            "printi",
            [ new NativeFunctionParameter("n", ValueType.Int),],
            ValueType.Unit
        ),
    ];

    /// <summary>
    /// Список встроенных типов языка.
    /// </summary>
    public static readonly IReadOnlyList<BuiltinType> Types =
    [
        new("int", ValueType.Int),
        new("str", ValueType.String),
        new("float", ValueType.Float),
        new("unit", ValueType.Unit)
    ];
}