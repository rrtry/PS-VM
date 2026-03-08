using ValueType = Runtime.ValueType;

namespace Ast.Declarations;

public abstract class AbstractTypeDeclaration : Declaration
{
    protected AbstractTypeDeclaration(string name, ValueType type)
    {
        Name = name;
        ResultType = type;
    }

    public string Name { get; }
}