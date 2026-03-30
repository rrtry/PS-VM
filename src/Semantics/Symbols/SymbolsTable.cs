using System.Diagnostics;

using Ast.Declarations;

using Semantics.Exceptions;

namespace Semantics.Symbols;

/// <summary>
/// Глобальная таблица символов.
/// </summary>
public sealed class SymbolsTable
{
    private readonly Dictionary<string, Declaration> declarations;
    private readonly Dictionary<string, Declaration> types;
    private readonly Stack<Dictionary<string, VariableSymbol>> _localScopes = new();

    public SymbolsTable()
    {
        declarations = [];
        types = [];
    }

    public int CurrentLocalScopeLevel => _localScopes.Count;

    public AbstractFunctionDeclaration GetFunctionDeclaration(string name)
    {
        Declaration? declaration = FindDeclaration(table => table.declarations, name);
        return declaration switch
        {
            AbstractFunctionDeclaration function => function,
            null => throw UnknownSymbolException.UndefinedVariableOrFunction(name),
            _ => throw new UnreachableException(),
        };
    }

    public AbstractTypeDeclaration GetTypeDeclaration(string name)
    {
        Declaration? declaration = FindDeclaration(table => table.types, name);
        if (declaration is null)
        {
            throw UnknownSymbolException.UndefinedType(name);
        }

        return (AbstractTypeDeclaration)declaration;
    }

    public void DeclareFunction(AbstractFunctionDeclaration symbol)
    {
        if (!declarations.TryAdd(symbol.Name, symbol))
        {
            throw DuplicateSymbolException.DuplicateVariableOrFunction(symbol.Name);
        }
    }

    public void DeclareType(AbstractTypeDeclaration symbol)
    {
        if (!types.TryAdd(symbol.Name, symbol))
        {
            throw DuplicateSymbolException.DuplicateType(symbol.Name);
        }
    }

    public void EnterLocalScope()
    {
        _localScopes.Push(new Dictionary<string, VariableSymbol>());
    }

    public void ExitLocalScope()
    {
        if (_localScopes.Count > 0)
        {
            _localScopes.Pop();
        }
    }

    public bool DeclareLocalVariable(VariableSymbol symbol)
    {
        if (_localScopes.Count == 0)
        {
            EnterLocalScope();
        }

        Dictionary<string, VariableSymbol> currentScope = _localScopes.Peek();

        if (currentScope.ContainsKey(symbol.Name))
        {
            return false;
        }

        currentScope[symbol.Name] = symbol;
        return true;
    }

    public VariableSymbol? LookupLocalVariable(string name)
    {
        foreach (Dictionary<string, VariableSymbol> scope in _localScopes)
        {
            if (scope.TryGetValue(name, out VariableSymbol? symbol))
            {
                return symbol;
            }
        }

        return null;
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