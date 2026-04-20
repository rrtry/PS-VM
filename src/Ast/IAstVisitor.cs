using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

namespace Ast;

public interface IAstVisitor
{
    public void Visit(EntryPointNode n);

    public void Visit(BinaryOperationExpression e);

    public void Visit(UnaryOperationExpression e);

    public void Visit(LiteralExpression e);

    public void Visit(FunctionCallExpression e);

    public void Visit(IdentifierExpression e);

    public void Visit(FunctionDeclaration d);

    public void Visit(VariableDeclaration d);

    public void Visit(BlockStatement s);

    public void Visit(ReturnStatement s);

    public void Visit(AssignmentStatement s);

    public void Visit(IfElseStatement s);
}