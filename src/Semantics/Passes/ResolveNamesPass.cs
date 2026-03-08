using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Runtime;

using Semantics.Symbols;

namespace Semantics.Passes;

/// <summary>
/// Проход по AST, устанавливающий соответствие имён и символов (объявлений).
/// </summary>
public sealed class ResolveNamesPass : AbstractPass
{
    /// <summary>
    /// В таблицу символов складываются объявления.
    /// </summary>
    private SymbolsTable symbols;

    public ResolveNamesPass(SymbolsTable globalSymbols)
    {
        symbols = globalSymbols;
    }

    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);
        e.Function = symbols.GetFunctionDeclaration(e.Name);
    }

    public override void Visit(VariableExpression e)
    {
        base.Visit(e);
        e.Variable = symbols.GetVariableDeclaration(e.Name);
    }

    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);
        d.DeclaredType = d.DeclaredTypeName != null ? symbols.GetTypeDeclaration(d.DeclaredTypeName) : null;
        symbols.DeclareVariable(d);
    }

    public override void Visit(FunctionDeclaration d)
    {
        d.ResultType = d.DeclaredTypeName != null ? symbols.GetTypeDeclaration(d.DeclaredTypeName).ResultType : symbols.GetTypeDeclaration("void").ResultType;
        d.DeclaredType = d.DeclaredTypeName != null ? symbols.GetTypeDeclaration(d.DeclaredTypeName) : null;

        symbols.DeclareFunction(d);
        symbols = new SymbolsTable(symbols);

        try
        {
            base.Visit(d);
        }
        finally
        {
            symbols = symbols.Parent!;
        }
    }

    public override void Visit(ParameterDeclaration d)
    {
        base.Visit(d);
        d.Type = symbols.GetTypeDeclaration(d.TypeName);
        symbols.DeclareVariable(d);
    }

    public override void Visit(IfElseStatement e)
    {
        symbols = new SymbolsTable(symbols);
        try
        {
            base.Visit(e);
        }
        finally
        {
            symbols = symbols.Parent!;
        }
    }

    public override void Visit(ForLoopStatement e)
    {
        symbols = new SymbolsTable(symbols);
        try
        {
            base.Visit(e);
        }
        finally
        {
            symbols = symbols.Parent!;
        }
    }

    public override void Visit(ForLoopIteratorDeclaration d)
    {
        base.Visit(d);
        symbols.DeclareVariable(d);
    }
}