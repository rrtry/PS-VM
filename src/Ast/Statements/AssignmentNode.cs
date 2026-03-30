using Ast.Attributes;
using Ast.Declarations;
using Ast.Expressions;

namespace Ast.Statements;

/// <summary>
/// Присваивание значения переменной.
/// </summary>
public sealed class AssignmentNode : Statement
{
    private AstAttribute<AbstractFunctionDeclaration?>? symbol;

    public AssignmentNode(string variableName, Expression value)
    {
        VariableName = variableName;
        Value = value;
    }

    public string VariableName { get; }

    public Expression Value { get; }

    /// <summary>
    /// Ссылка на объявление переменной.
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