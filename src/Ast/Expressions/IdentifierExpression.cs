using Ast.Attributes;
using Ast.Declarations;

namespace Ast.Expressions;

/// <summary>
/// Выражение-идентификатор: обращение к переменной по имени.
/// </summary>
public sealed class IdentifierExpression : Expression
{
    private AstAttribute<VariableDeclaration> variable;

    public IdentifierExpression(string name)
    {
        Name = name;
    }

    public string Name { get; }

    /// <summary>
    /// Ссылка на объявление переменной (заполняется на этапе семантического анализа).
    /// </summary>
    public VariableDeclaration Variable
    {
        get => variable.Get();
        set => variable.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}