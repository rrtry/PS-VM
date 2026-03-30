using Ast.Attributes;
using Ast.Declarations;
using Ast.Expressions;

namespace Ast.Statements;

/// <summary>
/// Объявление переменной.
/// </summary>
public sealed class VariableDeclarationNode : Statement
{
    private AstAttribute<AbstractFunctionDeclaration?>? symbol;

    public VariableDeclarationNode(string name, BuiltinType? typeAnnotation, Expression initializer)
    {
        Name = name;
        TypeAnnotation = typeAnnotation;
        Initializer = initializer;
    }

    public string Name { get; }

    /// <summary>
    /// Аннотация типа.
    /// </summary>
    public BuiltinType? TypeAnnotation { get; }

    /// <summary>
    /// Выражение-инициализатор.
    /// </summary>
    public Expression Initializer { get; }

    /// <summary>
    /// Ссылка на запись в таблице символов.
    /// </summary>
    public AbstractFunctionDeclaration? Symbol
    {
        get => symbol?.Get();
        set => symbol?.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}