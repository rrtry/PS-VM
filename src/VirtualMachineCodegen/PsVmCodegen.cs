using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Runtime;

using VirtualMachine.Builtins;
using VirtualMachine.Instructions;

using ValueType = Runtime.ValueType;

namespace VirtualMachineCodegen;

/// <summary>
/// Генерирует инструкции виртуальной машины путём обхода абстрактного синтаксического дерева (AST) программы.
/// </summary>
public class PsVmCodegen : IAstVisitor
{
    private static readonly IReadOnlyDictionary<string, BuiltinFunctionCode> BuiltinFunctionsMap =
        new Dictionary<string, BuiltinFunctionCode>
        {
            {
                Builtins.Print, BuiltinFunctionCode.Print
            },
            {
                Builtins.PrintI, BuiltinFunctionCode.PrintI
            },
            {
                Builtins.PrintF, BuiltinFunctionCode.PrintF
            },
            {
                Builtins.ItoS, BuiltinFunctionCode.ItoS
            },
            {
                Builtins.FtoS, BuiltinFunctionCode.FtoS
            },
            {
                Builtins.ItoF, BuiltinFunctionCode.ItoF
            },
            {
                Builtins.FtoI, BuiltinFunctionCode.FtoI
            },
            {
                Builtins.StoI, BuiltinFunctionCode.StoI
            },
            {
                Builtins.StoF, BuiltinFunctionCode.StoF
            },
            {
                Builtins.SConcat, BuiltinFunctionCode.SConcat
            },
            {
                Builtins.SubStr, BuiltinFunctionCode.SubStr
            },
            {
                Builtins.StrLen, BuiltinFunctionCode.StrLen
            },
            {
                Builtins.Input, BuiltinFunctionCode.Input
            },
            {
                Builtins.PrintB, BuiltinFunctionCode.PrintB
            },
        };

    private readonly InstructionsBuilder _builder = new();

    public List<Instruction> GenerateCode(EntryPointNode program)
    {
        program.Accept(this);
        _builder.Append(new Instruction(InstructionCode.Halt));
        return _builder.Finish();
    }

    public void Visit(LiteralExpression e)
    {
        _builder.Append(new Instruction(InstructionCode.Push, e.Value));
    }

    public void Visit(BinaryOperationExpression e)
    {
        switch (e.Operation)
        {
            case BinaryOperation.Add:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.Add);
                break;

            case BinaryOperation.Subtract:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.Subtract);
                break;

            case BinaryOperation.Multiply:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.Multiply);
                break;

            case BinaryOperation.Power:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.Power);
                break;

            case BinaryOperation.Divide:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.Divide);
                break;

            case BinaryOperation.Modulo:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.Modulo);
                break;

            case BinaryOperation.And:
                GenerateLogicalAndCode(e);
                break;

            case BinaryOperation.Or:
                GenerateLogicalOrCode(e);
                break;

            case BinaryOperation.Equal:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.Equal);
                break;

            case BinaryOperation.NotEqual:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.NotEqual);
                break;

            case BinaryOperation.Less:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.Less);
                break;

            case BinaryOperation.LessOrEqual:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.LessOrEqual);
                break;

            case BinaryOperation.Greater:
                GenerateBinaryOperationCode(e.Right, e.Left, InstructionCode.Less);
                break;

            case BinaryOperation.GreaterOrEqual:
                GenerateBinaryOperationCode(e.Right, e.Left, InstructionCode.LessOrEqual);
                break;

            default:
                throw new NotImplementedException($"Unsupported binary operation type {e.Operation}");
        }
    }

    public void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
        switch (e.Operation)
        {
            case UnaryOperation.Minus:
                _builder.Append(new Instruction(InstructionCode.Negate));
                break;

            case UnaryOperation.Not:
                _builder.Append(new Instruction(InstructionCode.Not));
                break;

            case UnaryOperation.Plus:
                break;

            default:
                throw new NotImplementedException($"Unsupported unary operation");
        }
    }

    public void Visit(FunctionCallExpression e)
    {
        foreach (Expression argument in e.Arguments)
        {
            argument.Accept(this);
        }

        switch (e.Function)
        {
            case NativeFunction native:
                _builder.Append(new Instruction(InstructionCode.CallBuiltin, GetBuiltinFunctionCode(native.Name)));
                break;

            default:
                throw new NotImplementedException($"Unsupported AST subclass {e.Function.GetType()}");
        }
    }

    public void Visit(BlockStatement s)
    {
        GenerateBlockStatementCode(s);
    }

    public void Visit(EntryPointNode n)
    {
        n.Main.Accept(this);
    }

    public void Visit(FunctionDeclaration d)
    {
        d.Body.Accept(this);
    }

    public void Visit(ReturnStatement s)
    {
        s.ReturnValue!.Accept(this);
    }

    public void Visit(IdentifierExpression e)
    {
        _builder.Append(new Instruction(InstructionCode.LoadLocal, e.Name));
    }

    public void Visit(VariableDeclaration d)
    {
        d.Initializer.Accept(this);
        _builder.Append(new Instruction(InstructionCode.DefineLocal, d.Name));
    }

    public void Visit(AssignmentStatement s)
    {
        s.Right.Accept(this);
        IdentifierExpression lvalue = (IdentifierExpression)s.Left;
        _builder.Append(new Instruction(InstructionCode.StoreLocal, lvalue.Name));
    }

    public void Visit(IfElseStatement s)
    {
        if (s.ElseBranch != null)
        {
            BasicBlock elseBlock = _builder.CreateBasicBlock();
            BasicBlock finalBlock = _builder.CreateBasicBlock();

            s.Condition.Accept(this);
            _builder.AppendJump(InstructionCode.JumpIfFalse, elseBlock);

            s.ThenBranch.Accept(this);
            _builder.AppendJump(InstructionCode.Jump, finalBlock);

            _builder.InsertPoint = elseBlock;
            s.ElseBranch.Accept(this);
            _builder.AppendJump(InstructionCode.Jump, finalBlock);

            _builder.InsertPoint = finalBlock;
        }
        else
        {
            BasicBlock finalBlock = _builder.CreateBasicBlock();

            s.Condition.Accept(this);
            _builder.AppendJump(InstructionCode.JumpIfFalse, finalBlock);

            s.ThenBranch.Accept(this);
            _builder.AppendJump(InstructionCode.Jump, finalBlock);

            _builder.InsertPoint = finalBlock;
        }
    }

    private void GenerateLogicalAndCode(BinaryOperationExpression e)
    {
        BasicBlock shortCircuitBlock = _builder.CreateBasicBlock();
        BasicBlock finalBlock = _builder.CreateBasicBlock();

        e.Left.Accept(this);
        _builder.AppendJump(InstructionCode.JumpIfFalse, shortCircuitBlock);

        e.Right.Accept(this);
        _builder.Append(new Instruction(InstructionCode.Push, new Value(false)));
        _builder.Append(new Instruction(InstructionCode.NotEqual));
        _builder.AppendJump(InstructionCode.Jump, finalBlock);

        _builder.InsertPoint = shortCircuitBlock;
        _builder.Append(new Instruction(InstructionCode.Push, new Value(false)));
        _builder.AppendJump(InstructionCode.Jump, finalBlock);

        _builder.InsertPoint = finalBlock;
    }

    private void GenerateLogicalOrCode(BinaryOperationExpression e)
    {
        BasicBlock shortCircuitBlock = _builder.CreateBasicBlock();
        BasicBlock finalBlock = _builder.CreateBasicBlock();

        e.Left.Accept(this);

        _builder.AppendJump(InstructionCode.JumpIfTrue, shortCircuitBlock);

        e.Right.Accept(this);
        _builder.Append(new Instruction(InstructionCode.Push, new Value(false)));
        _builder.Append(new Instruction(InstructionCode.NotEqual));
        _builder.AppendJump(InstructionCode.Jump, finalBlock);

        _builder.InsertPoint = shortCircuitBlock;
        _builder.Append(new Instruction(InstructionCode.Push, new Value(true)));
        _builder.AppendJump(InstructionCode.Jump, finalBlock);

        _builder.InsertPoint = finalBlock;
    }

    private void GenerateBlockStatementCode(BlockStatement statement)
    {
        PushScope();

        IReadOnlyList<AstNode> sequence = statement.Statements;
        for (int i = 0, iMax = sequence.Count - 1; i <= iMax; ++i)
        {
            AstNode node = sequence[i];
            node.Accept(this);

            if (node is Expression && i != iMax)
            {
                Expression expression = (Expression)node;
                if (expression.ResultType != ValueType.Unit)
                {
                    _builder.Append(new Instruction(InstructionCode.Pop));
                }
            }
        }

        PopScope();
    }

    private void PushScope()
    {
        _builder.Append(new Instruction(InstructionCode.PushVars));
    }

    private void PopScope()
    {
        _builder.Append(new Instruction(InstructionCode.PopVars));
    }

    private void GenerateBinaryOperationCode(Expression left, Expression right, InstructionCode code)
    {
        left.Accept(this);
        right.Accept(this);
        _builder.Append(new Instruction(code));
    }

    private static int GetBuiltinFunctionCode(string name)
    {
        if (!BuiltinFunctionsMap.TryGetValue(name, out BuiltinFunctionCode code))
        {
            throw new NotImplementedException($"Unsupported builtin function {name}");
        }

        return (int)code;
    }
}