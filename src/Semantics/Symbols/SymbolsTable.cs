using Ast.Declarations;
using Ast.Statements;

using Semantics.Exceptions;

namespace Semantics.Symbols;

/// <summary>
/// Глобальная таблица символов.
/// </summary>
public sealed class SymbolsTable
{
    private readonly Dictionary<string, Declaration> _functions;
    private readonly Dictionary<string, Declaration> _types;
    private readonly Dictionary<string, Declaration> _variables = new();

    public SymbolsTable()
    {
        _functions = [];
        _types = [];
        _variables = new();
    }

    public int CurrentLocalScopeLevel => _variables.Count;

    public AbstractFunctionDeclaration GetFunctionDeclaration(string name)
    {
        Declaration? function = FindDeclaration(table => table._functions, name);
        if (function == null)
        {
            throw UnknownSymbolException.UndefinedVariableOrFunction(name);
        }

        return (AbstractFunctionDeclaration)function;
    }

    public AbstractTypeDeclaration GetTypeDeclaration(string name)
    {
        Declaration? type = FindDeclaration(table => table._types, name);
        if (type is null)
        {
            throw UnknownSymbolException.UndefinedType(name);
        }

        return (AbstractTypeDeclaration)type;
    }

    public void DeclareFunction(AbstractFunctionDeclaration symbol)
    {
        if (!_functions.TryAdd(symbol.Name, symbol))
        {
            throw DuplicateSymbolException.DuplicateVariableOrFunction(symbol.Name);
        }
    }

    public void DeclareType(AbstractTypeDeclaration symbol)
    {
        if (!_types.TryAdd(symbol.Name, symbol))
        {
            throw DuplicateSymbolException.DuplicateType(symbol.Name);
        }
    }

    public void DeclareVariable(VariableDeclaration decl)
    {
        if (!_variables.TryAdd(decl.Name, decl))
        {
            throw DuplicateSymbolException.DuplicateType(decl.Name);
        }
    }

    public VariableDeclaration FindVariable(string name)
    {
        Declaration? variable = FindDeclaration(table => table._variables, name);
        if (variable == null)
        {
            throw UnknownSymbolException.UndefinedVariableOrFunction(name);
        }

        return (VariableDeclaration)variable;
    }

    private Declaration? FindDeclaration(Func<SymbolsTable, Dictionary<string, Declaration>> getTable, string name)
    {
        if (getTable(this).TryGetValue(name, out Declaration? declaration))
        {
            return declaration;
        }

        return null;
    }
}