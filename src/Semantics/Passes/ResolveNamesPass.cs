using Ast.Declarations;
using Ast.Expressions;
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

    public override void Visit(FunctionDeclaration d)
    {
        d.ResultType = d.DeclaredTypeName != null ? symbols.GetTypeDeclaration(d.DeclaredTypeName).ResultType :
                       symbols.GetTypeDeclaration("unit").ResultType;
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

    public override void Visit(FunctionCallExpression e)
    {
        base.Visit(e);
        e.Function = symbols.GetFunctionDeclaration(e.Name);
    }
}