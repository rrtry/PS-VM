using Ast.Expressions;

namespace Ast.Declarations;

/// <summary>
/// Объявление переменной.
/// </summary>
public sealed class VariableDeclaration : AbstractVariableDeclaration
{
    public VariableDeclaration(string name, BuiltinType? typeAnnotation, Expression initializer)
    : base(name, typeAnnotation)
    {
        Initializer = initializer;
    }

    /// <summary>
    /// Выражение-инициализатор.
    /// </summary>
    public Expression Initializer { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}