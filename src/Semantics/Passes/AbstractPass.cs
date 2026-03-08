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

    public virtual void Visit(AssignmentExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);
    }

    public virtual void Visit(IfElseStatement e)
    {
        e.Condition.Accept(this);
        e.ThenBranch.Accept(this);
        e.ElseBranch?.Accept(this);
    }

    public virtual void Visit(WhileLoopStatement e)
    {
        e.Condition.Accept(this);
        e.LoopBody.Accept(this);
    }

    public virtual void Visit(ForLoopStatement e)
    {
        e.StartValue.Accept(this);
        e.EndCondition.Accept(this);
        e.UpdateExpr?.Accept(this);
        e.Body.Accept(this);
    }

    public virtual void Visit(VariableDeclaration d)
    {
        d.InitialValue.Accept(this);
    }

    public virtual void Visit(FunctionDeclaration d)
    {
        foreach (AbstractParameterDeclaration declaration in d.Parameters)
        {
            declaration.Accept(this);
        }

        d.Body.Accept(this);
    }

    public virtual void Visit(ReturnStatement s)
    {
        s.ReturnValue.Accept(this);
    }

    public virtual void Visit(LiteralExpression e)
    {
    }

    public virtual void Visit(ContinueLoopStatement s)
    {
    }

    public virtual void Visit(ParameterDeclaration d)
    {
    }

    public virtual void Visit(BreakLoopStatement e)
    {
    }

    public virtual void Visit(VariableExpression e)
    {
    }

    public virtual void Visit(ForLoopIteratorDeclaration d)
    {
    }
}