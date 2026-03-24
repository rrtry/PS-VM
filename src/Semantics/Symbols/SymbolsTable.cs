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

    public SymbolsTable()
    {
        declarations = [];
        types = [];
    }

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

    private Declaration? FindDeclaration(Func<SymbolsTable, Dictionary<string, Declaration>> getTable, string name)
    {
        if (getTable(this).TryGetValue(name, out Declaration? declaration))
        {
            return declaration;
        }

        return null;
    }
}