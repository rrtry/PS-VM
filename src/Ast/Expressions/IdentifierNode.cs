using Ast.Attributes;
using Runtime;

using ValueType = Runtime.ValueType;

namespace Ast.Expressions;

/// <summary>
/// Выражение-идентификатор: обращение к переменной по имени.
/// </summary>
public sealed class IdentifierNode : Expression
{
    private AstAttribute<Declarations.AbstractFunctionDeclaration?>? symbol;

    public IdentifierNode(string name)
    {
        Name = name;
    }

    public string Name { get; }

    /// <summary>
    /// Ссылка на объявление переменной (заполняется на этапе семантического анализа).
    /// </summary>
    public Declarations.AbstractFunctionDeclaration? Symbol
    {
        get => symbol?.Get();
        set => symbol?.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}