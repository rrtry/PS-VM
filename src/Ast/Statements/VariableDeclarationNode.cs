using Ast.Attributes;
using Ast.Expressions;
using Ast.Declarations;

namespace Ast.Statements;

/// <summary>
/// Объявление переменной: <c>let x: int = 42;</c>
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
    /// Аннотация типа (может быть <c>null</c>, если тип выводится из инициализатора).
    /// </summary>
    public BuiltinType? TypeAnnotation { get; }

    /// <summary>
    /// Выражение-инициализатор.
    /// </summary>
    public Expression Initializer { get; }

    /// <summary>
    /// Ссылка на запись в таблице символов (заполняется семантическим анализом).
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