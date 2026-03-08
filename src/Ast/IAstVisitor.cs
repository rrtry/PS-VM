using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

namespace Ast;

public interface IAstVisitor
{
    public void Visit(BinaryOperationExpression e);

    public void Visit(UnaryOperationExpression e);

    public void Visit(LiteralExpression e);

    public void Visit(VariableExpression e);

    public void Visit(FunctionCallExpression e);

    public void Visit(AssignmentExpression e);

    public void Visit(WhileLoopStatement e);

    public void Visit(VariableDeclaration d);

    public void Visit(FunctionDeclaration d);

    public void Visit(ParameterDeclaration d);

    public void Visit(ForLoopIteratorDeclaration d);

    public void Visit(IfElseStatement s);

    public void Visit(BlockStatement s);

    public void Visit(ForLoopStatement s);

    public void Visit(BreakLoopStatement s);

    public void Visit(ContinueLoopStatement s);

    public void Visit(ReturnStatement s);
}