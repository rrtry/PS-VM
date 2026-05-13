namespace Ast.Declarations;

/// <summary>
/// Объявление параметра функции.
/// </summary>
public class ParameterDeclaration : AbstractParameterDeclaration
{
    public ParameterDeclaration(string name, BuiltinType type)
    {
        Name = name;
        DeclaredType = type;
    }

    /// <summary>
    /// Название параметра
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Аннотация типа.
    /// </summary>
    public BuiltinType? DeclaredType { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}