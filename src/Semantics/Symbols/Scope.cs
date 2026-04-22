using Ast.Declarations;

namespace Semantics.Symbols;

public class Scope
{
    private readonly Dictionary<string, VariableDeclaration> _variables;

    public Scope? Parent { get; }

    public Scope(Scope? parent = null)
    {
        Parent = parent;
        _variables = new();
    }

    public VariableDeclaration? GetVariable(string variable)
    {
        VariableDeclaration? declaration = _variables.GetValueOrDefault(variable);
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

    public bool DeclareVariable(string variable, VariableDeclaration declaration)
    {
        if (_variables.ContainsKey(variable))
        {
            return false;
        }

        _variables[variable] = declaration;
        return true;
    }
}