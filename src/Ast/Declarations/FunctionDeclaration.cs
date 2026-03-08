using Ast.Attributes;
using Ast.Expressions;

namespace Ast.Declarations;

/// <summary>
/// Объявление пользовательской функции или процедуры.
/// </summary>
public sealed class FunctionDeclaration : AbstractFunctionDeclaration
{
    private AstAttribute<AbstractTypeDeclaration?> declaredType;

    public FunctionDeclaration(
        string name,
        IReadOnlyList<ParameterDeclaration> parameters,
        string? declaredTypeName,
        BlockStatement body
    )
        : base(name, parameters)
    {
        DeclaredTypeName = declaredTypeName;
        Body = body;
    }

    public string? DeclaredTypeName { get; }

    public AbstractTypeDeclaration? DeclaredType
    {
        get => declaredType.Get();
        set => declaredType.Set(value);
    }

    public BlockStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}