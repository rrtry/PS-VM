namespace Execution;

using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Execution.Exceptions;
using Runtime;

public class AstEvaluator : IAstVisitor
{
    private readonly Context context;

    private readonly Stack<Value> values = [];

    public AstEvaluator(Context context)
    {
        this.context = context;
    }

    public Value Evaluate(AstNode node)
    {
        if (values.Count > 0)
        {
            throw new InvalidOperationException(
                $"Evaluation stack must be empty, but contains {values.Count} values: {string.Join(", ", values)}"
            );
        }

        node.Accept(this);
        switch (values.Count)
        {
            case 0:
                throw new InvalidOperationException("Evaluator logical error: the stack has no evaluation result");

            case > 1:
                throw new InvalidOperationException($"Evaluator logical error: expected 1 value, got {values.Count} values: {string.Join(", ", values)}");

            default:
                return values.Pop();
        }
    }

    public void Visit(BinaryOperationExpression e)
    {
        values.Push(EvaluationUtil.ApplyBinaryOperation(e.Operation, EvaluateLeft, EvaluateRight));
        return;

        Value EvaluateLeft()
        {
            e.Left.Accept(this);
            return values.Pop();
        }

        Value EvaluateRight()
        {
            e.Right.Accept(this);
            return values.Pop();
        }
    }

    public void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
        switch (e.Operation)
        {
            case UnaryOperation.Not:
                values.Push(new Value(values.Pop().AsLong() == 0L ? 1L : 0L));
                break;

            case UnaryOperation.Minus:
                Value value = values.Pop();
                if (value.IsDouble())
                {
                    values.Push(new Value(-value.AsDouble()));
                }
                else if (value.IsLong())
                {
                    values.Push(new Value(-value.AsLong()));
                }
                else
                {
                    throw new InvalidOperationException($"Bad unary operator for {value.GetType()}");
                }

                break;

            case UnaryOperation.Plus:
                break;

            default:
                throw new NotImplementedException($"Unknown unary operation {e.Operation}");
        }
    }

    public void Visit(LiteralExpression e)
    {
        values.Push(e.Value);
    }

    public void Visit(VariableExpression e)
    {
        values.Push(context.GetValue(e.Name));
    }

    public void Visit(FunctionCallExpression e)
    {
        switch (e.Function)
        {
            case NativeFunction nativeFunction:
                InvokeNativeFunction(e, nativeFunction);
                break;
            case FunctionDeclaration function:
                InvokeFunction(e, function);
                break;
            default:
                throw new InvalidOperationException($"Unknown function subclass {e.Function.GetType()}");
        }
    }

    public void Visit(AssignmentExpression e)
    {
        e.Right.Accept(this);
        Value value = values.Peek();

        VariableExpression left = (VariableExpression)e.Left;
        context.AssignVariable(left.Name, value);
    }

    public void Visit(BlockStatement s)
    {
        values.Push(Value.Unit);
        context.PushScope(new Scope());

        try
        {
            foreach (AstNode node in s.Statements)
            {
                values.Pop();
                node.Accept(this);
            }
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(IfElseStatement s)
    {
        s.Condition.Accept(this);
        long condition = values.Pop().AsLong();

        if (condition != 0L)
        {
            s.ThenBranch.Accept(this);
        }
        else
        {
            if (s.ElseBranch != null)
            {
                s.ElseBranch.Accept(this);
            }
            else
            {
                values.Push(Value.Unit);
            }
        }
    }

    public void Visit(ForLoopStatement e)
    {
        context.PushScope(new Scope());
        try
        {
            e.StartValue.Accept(this);
            long iteratorValue = values.Pop().AsLong();

            context.AssignVariable(e.Iterator.Name, new Value(iteratorValue)); // Changed: DefineVariable -> AssignVariable
            values.Push(Value.Unit);

            while (true)
            {
                e.EndCondition.Accept(this);
                long condition = values.Pop().AsLong();

                if (condition == 0L)
                {
                    break;
                }

                try
                {
                    values.Pop();
                    e.Body.Accept(this);
                }
                catch (ContinueLoopException)
                {
                }

                e.UpdateExpr!.Accept(this);
                values.Pop();
            }
        }
        catch (BreakLoopException)
        {
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(WhileLoopStatement e)
    {
        values.Push(Value.Unit);
        context.PushScope(new Scope());

        try
        {
            while (true)
            {
                e.Condition.Accept(this);
                long condition = values.Pop().AsLong();

                if (condition == 0L)
                {
                    break;
                }

                try
                {
                    values.Pop();
                    e.LoopBody.Accept(this);
                }
                catch (ContinueLoopException)
                {
                }
            }
        }
        catch (BreakLoopException)
        {
        }
        finally
        {
            context.PopScope();
        }
    }

    public void Visit(VariableDeclaration d)
    {
        d.InitialValue.Accept(this);
        Value value = values.Peek();
        context.DefineVariable(d.Name, value);
    }

    public void Visit(FunctionDeclaration d)
    {
        values.Push(Value.Unit);
    }

    public void Visit(BreakLoopStatement s)
    {
        values.Push(Value.Unit);
        throw new BreakLoopException();
    }

    public void Visit(ContinueLoopStatement s)
    {
        values.Push(Value.Unit);
        throw new ContinueLoopException();
    }

    public void Visit(ReturnStatement s)
    {
        s.ReturnValue.Accept(this);
        values.Push(Value.Unit);
        throw new ReturnException();
    }

    public void Visit(ParameterDeclaration d)
    {
    }

    public void Visit(ForLoopIteratorDeclaration d)
    {
    }

    private void InvokeNativeFunction(FunctionCallExpression e, NativeFunction function)
    {
        List<Value> arguments = [];
        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);
            arguments.Add(values.Pop());
        }

        Value result = function.Invoke(arguments);
        values.Push(result);
    }

    private void InvokeFunction(FunctionCallExpression e, FunctionDeclaration function)
    {
        bool hasReturn = false;
        bool hasReturnType = function.DeclaredType != null &&
                             function.DeclaredType.ResultType != ValueType.Unit;

        Value returnValue = Value.Unit;
        context.PushScope(new Scope());

        try
        {
            for (int i = 0, iMax = function.Parameters.Count; i < iMax; ++i)
            {
                e.Arguments[i].Accept(this);
                Value argument = values.Pop();

                string name = function.Parameters[i].Name;
                context.DefineVariable(name, argument);
            }

            function.Body.Accept(this);
        }
        catch (ReturnException)
        {
            hasReturn = true;
        }
        finally
        {
            if (!hasReturn && hasReturnType)
            {
                throw new InvalidOperationException("Function has to have a return statement in the end");
            }

            if (hasReturn)
            {
                values.Pop(); // First pop return statement value
                returnValue = values.Pop(); // Then the expression we return;
            }

            context.PopScope();
        }

        if (hasReturn)
        {
            values.Push(returnValue);
        }
    }
}