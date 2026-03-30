using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Semantics.Exceptions;
using Semantics.Symbols;

namespace Semantics.Passes;

using ValueType = Runtime.ValueType;

/// <summary>
/// Проход по AST, устанавливающий соответствие имён и символов (объявлений).
/// </summary>
public sealed class ResolveNamesPass : AbstractPass
{
    /// <summary>
    /// В таблицу символов складываются объявления.
    /// </summary>
    private readonly SymbolsTable _symbols;

    public ResolveNamesPass(SymbolsTable globalSymbols)
    {
        _symbols = globalSymbols;
    }

    public override void Visit(FunctionDeclaration d)
    {
        AbstractTypeDeclaration declaredType = _symbols.GetTypeDeclaration(d.DeclaredTypeName ?? "unit");
        d.ResultType = declaredType.ResultType;
        d.DeclaredType = declaredType;

        _symbols.DeclareFunction(d);
        base.Visit(d);
    }

    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);
        e.Function = _symbols.GetFunctionDeclaration(e.Name);
    }

    /// <summary>
    /// Стек областей видимости для локальных переменных.
    /// </summary>
    public override void Visit(BlockStatement s)
    {
        _symbols.EnterLocalScope();

        foreach (AstNode nested in s.Statements)
        {
            nested.Accept(this);
        }

        _symbols.ExitLocalScope();
    }

    public override void Visit(VariableDeclarationNode node)
    {
        node.Initializer.Accept(this);

        ValueType declaredType = node.Initializer.ResultType;

        if (declaredType == ValueType.Unit)
        {
            return;
        }

        VariableSymbol symbol = new VariableSymbol(
            node.Name,
            declaredType,
            _symbols.CurrentLocalScopeLevel);

        if (!_symbols.DeclareLocalVariable(symbol))
        {
            throw DuplicateSymbolException.DuplicateVariableOrFunction(node.Name);
        }

        symbol.IsInitialized = true;
    }

    public override void Visit(AssignmentNode node)
    {
        VariableSymbol? symbol = _symbols.LookupLocalVariable(node.VariableName);

        if (symbol == null)
        {
            throw UnknownSymbolException.UndefinedVariableOrFunction(node.VariableName);
        }

        node.Value.Accept(this);

        symbol.IsInitialized = true;
    }

    public override void Visit(IdentifierNode node)
    {
        VariableSymbol? localSymbol = _symbols.LookupLocalVariable(node.Name);

        if (localSymbol != null)
        {
            if (!localSymbol.IsInitialized)
            {
            }

            node.ResultType = localSymbol.DeclaredType;
            return;
        }

        try
        {
            _symbols.GetFunctionDeclaration(node.Name);
        }
        catch (UnknownSymbolException)
        {
            throw UnknownSymbolException.UndefinedVariableOrFunction(node.Name);
        }
    }
}