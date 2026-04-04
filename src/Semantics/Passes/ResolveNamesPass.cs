using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

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

    public override void Visit(BlockStatement s)
    {
        foreach (AstNode nested in s.Statements)
        {
            nested.Accept(this);
        }
    }

    public override void Visit(VariableDeclaration d)
    {
        base.Visit(d);
        _symbols.DeclareVariable(d);
    }

    public override void Visit(IdentifierExpression e)
    {
        base.Visit(e);
        e.Variable = _symbols.FindVariable(e.Name);
    }
}