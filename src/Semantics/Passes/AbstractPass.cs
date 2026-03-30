using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

namespace Semantics.Passes;

/// <summary>
/// Базовый класс для проходов по AST с целью вычисления атрибутов и семантических проверок.
/// </summary>
public abstract class AbstractPass : IAstVisitor
{
    public virtual void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);
    }

    public virtual void Visit(BlockStatement s)
    {
        foreach (AstNode nested in s.Statements)
        {
            nested.Accept(this);
        }
    }

    public virtual void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
    }

    public virtual void Visit(FunctionCallExpression e)
    {
        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);
        }
    }

    public virtual void Visit(FunctionDeclaration d)
    {
        // Объявление функции не поддерживает параметры
        d.Body.Accept(this);
    }

    public virtual void Visit(ReturnStatement s)
    {
        s.ReturnValue?.Accept(this);
    }

    public void Visit(EntryPointNode n)
    {
        n.Main.Accept(this);
    }

    public virtual void Visit(LiteralExpression e)
    {
    }

    public virtual void Visit(IdentifierNode node)
    {
    }

    public virtual void Visit(VariableDeclarationNode node)
    {
    }

    public virtual void Visit(AssignmentNode node)
    {
    }
}