using Ast.Expressions;

namespace Ast.Declarations;

/// <summary>
/// Объявление переменной.
/// </summary>
public sealed class VariableDeclaration : Declaration
{
    public VariableDeclaration(string name, BuiltinType? typeAnnotation, Expression initializer)
    {
        Name = name;
        DeclaredType = typeAnnotation;
        Initializer = initializer;
    }

    public string Name { get; }

    /// <summary>
    /// Аннотация типа.
    /// </summary>
    public BuiltinType? DeclaredType { get; }

    /// <summary>
    /// Выражение-инициализатор.
    /// </summary>
    public Expression Initializer { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}