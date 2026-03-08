using System.Diagnostics;

using Ast.Declarations;
using Semantics.Exceptions;

namespace Semantics.Symbols;

/// <summary>
/// Таблица символов, основанная на лексических областях видимости (областях действия) символов в коде.
/// </summary>
public sealed class SymbolsTable
{
    private readonly SymbolsTable? parent;

    private readonly Dictionary<string, Declaration> variablesAndFunctions;
    private readonly Dictionary<string, Declaration> types;

    public SymbolsTable(SymbolsTable? parent)
    {
        this.parent = parent;
        variablesAndFunctions = [];
        types = [];
    }

    public SymbolsTable? Parent => parent;

    public AbstractVariableDeclaration GetVariableDeclaration(string name)
    {
        Declaration? declaration = FindDeclaration(table => table.variablesAndFunctions, name);
        return declaration switch
        {
            AbstractVariableDeclaration variable => variable,
            AbstractFunctionDeclaration _ => throw new InvalidSymbolException(name, "function", "variable"),
            null => throw UnknownSymbolException.UndefinedVariableOrFunction(name),
            _ => throw new UnreachableException(),
        };
    }

    public AbstractFunctionDeclaration GetFunctionDeclaration(string name)
    {
        Declaration? declaration = FindDeclaration(table => table.variablesAndFunctions, name);
        return declaration switch
        {
            AbstractFunctionDeclaration function => function,
            AbstractVariableDeclaration _ => throw new InvalidSymbolException(name, "function", "variable"),
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

    public void DeclareVariable(AbstractVariableDeclaration symbol)
    {
        if (!variablesAndFunctions.TryAdd(symbol.Name, symbol))
        {
            throw DuplicateSymbolException.DuplicateVariableOrFunction(symbol.Name);
        }
    }

    public void DeclareFunction(AbstractFunctionDeclaration symbol)
    {
        if (!variablesAndFunctions.TryAdd(symbol.Name, symbol))
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

    private Declaration? FindDeclaration(Func<SymbolsTable, Dictionary<string, Declaration>> getTable, string name)
    {
        if (getTable(this).TryGetValue(name, out Declaration? declaration))
        {
            return declaration;
        }

        return parent?.FindDeclaration(getTable, name);
    }
}