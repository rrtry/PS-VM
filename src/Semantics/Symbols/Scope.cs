using Ast.Declarations;

namespace Semantics.Symbols;

public class Scope
{
    private readonly Dictionary<string, AbstractVariableDeclaration> _variables;

    public Scope(Scope? parent = null)
    {
        Parent = parent;
        _variables = new();
    }

    public Scope? Parent { get; }

    public AbstractVariableDeclaration? GetVariable(string variable)
    {
        AbstractVariableDeclaration? declaration = _variables.GetValueOrDefault(variable);
        if (declaration != null)
        {
            return declaration;
        }

        if (Parent != null)
        {
            return Parent!.GetVariable(variable);
        }

        return null;
    }

    public bool DeclareVariable(string variable, AbstractVariableDeclaration declaration)
    {
        if (_variables.ContainsKey(variable))
        {
            return false;
        }

        _variables[variable] = declaration;
        return true;
    }
}