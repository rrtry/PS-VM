using Ast.Attributes;
using Ast.Expressions;
using Ast.Declarations;

namespace Ast.Statements;

/// <summary>
/// Присваивание значения переменной: <c>x = 10;</c>
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
    /// Ссылка на объявление переменной (заполняется семантическим анализом).
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