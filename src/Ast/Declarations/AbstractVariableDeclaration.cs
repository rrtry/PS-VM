namespace Ast.Declarations;

/// <summary>
/// Абстрактный класс с информацией о переменной или формальном параметре функции.
/// </summary>
public abstract class AbstractVariableDeclaration : Declaration
{
    protected AbstractVariableDeclaration(string name, BuiltinType? type)
    {
        Name = name;
        DeclaredType = type;

        if (type != null)
        {
            ResultType = DeclaredType!.ResultType;
        }
    }

    public string Name { get; }

    public BuiltinType? DeclaredType { get; }
}