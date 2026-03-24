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
}