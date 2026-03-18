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
}