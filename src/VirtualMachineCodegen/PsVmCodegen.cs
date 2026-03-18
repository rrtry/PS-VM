using Ast;
using Ast.Declarations;
using Ast.Expressions;

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
        };

    private readonly InstructionsBuilder _builder = new();

    public List<Instruction> GenerateCode(BlockStatement program)
    {
        program.Accept(this);

        AstNode last = program.Statements.Last();
        if (last is Expression &&
            ((Expression)last).ResultType != ValueType.Unit)
        {
            _builder.Append(new Instruction(InstructionCode.StoreResult));
        }

        _builder.Append(new Instruction(InstructionCode.Push, 0));
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
            case BinaryOperation.And:
                GenerateLogicalAndCode(e);
                break;
            case BinaryOperation.Or:
                GenerateLogicalOrCode(e);
                break;
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
            case BinaryOperation.Equal:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.Equal);
                break;
            case BinaryOperation.NotEqual:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.NotEqual);
                break;
            case BinaryOperation.LessThan:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.Less);
                break;
            case BinaryOperation.LessThanOrEqual:
                GenerateBinaryOperationCode(e.Left, e.Right, InstructionCode.LessOrEqual);
                break;
            case BinaryOperation.GreaterThan:
                GenerateBinaryOperationCode(e.Right, e.Left, InstructionCode.Less);
                break;
            case BinaryOperation.GreaterThanOrEqual:
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

            case UnaryOperation.Plus:
                _builder.Append(new Instruction(InstructionCode.Push));
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
                Instruction instruction = native.Name switch
                {
                    _ => new Instruction(InstructionCode.CallBuiltin, GetBuiltinFunctionCode(native.Name)),
                };
                _builder.Append(instruction);
                break;

            default:
                throw new NotImplementedException($"Unsupported AST subclass {e.Function.GetType()}");
        }
    }

    public void Visit(AssignmentExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(VariableExpression e)
    {
        throw new NotImplementedException();
    }

    public void Visit(VariableDeclaration d)
    {
        throw new NotImplementedException();
    }

    public void Visit(BlockStatement s)
    {
        GenerateBlockStatementCode(s.Statements);
    }

    private void GenerateBlockStatementCode(IReadOnlyList<AstNode> sequence)
    {
        for (int i = 0, iMax = sequence.Count - 1; i <= iMax; ++i)
        {
            AstNode node = sequence[i];
            node.Accept(this);

            if (node is Expression &&
                i != iMax && ((Expression)node).ResultType != ValueType.Unit)
            {
                _builder.Append(new Instruction(InstructionCode.Pop));
            }
        }
    }

    private void GenerateBinaryOperationCode(Expression left, Expression right, InstructionCode code)
    {
        left.Accept(this);
        right.Accept(this);
        _builder.Append(new Instruction(code));
    }

    private void GenerateLogicalAndCode(BinaryOperationExpression e)
    {
        // Логическое "И" вычисляется по короткой схеме: если первый операнд обращается в "ЛОЖЬ",
        //  то второй операнд не вычисляется.
        BasicBlock shortCircuitBlock = _builder.CreateBasicBlock();
        BasicBlock finalBlock = _builder.CreateBasicBlock();

        // Вычисляем первый операнд.
        e.Left.Accept(this);

        // Переходим к короткой схеме, если первый операнд обращается в "ЛОЖЬ".
        _builder.AppendJump(InstructionCode.JumpIfFalse, shortCircuitBlock);

        // Иначе вычисляем второй операнд.
        // Затем используем операцию "X <> 0", чтобы привести "X" к булеву значению (1 или 0).
        e.Right.Accept(this);
        _builder.Append(new Instruction(InstructionCode.Push, 0));
        _builder.Append(new Instruction(InstructionCode.NotEqual));
        _builder.AppendJump(InstructionCode.Jump, finalBlock);

        // Выполняем короткую схему вычислений: левый операнд обратился в "ЛОЖЬ", и результат будет "ЛОЖЬ".
        _builder.InsertPoint = shortCircuitBlock;
        _builder.Append(new Instruction(InstructionCode.Push, 0));
        _builder.AppendJump(InstructionCode.Jump, finalBlock);

        _builder.InsertPoint = finalBlock;
    }

    private void GenerateLogicalOrCode(BinaryOperationExpression e)
    {
        // Логическое "ИЛИ" вычисляется по короткой схеме: если первый операнд обращается в "ИСТИНУ",
        //  то второй операнд не вычисляется.
        BasicBlock shortCircuitBlock = _builder.CreateBasicBlock();
        BasicBlock finalBlock = _builder.CreateBasicBlock();

        // Вычисляем первый операнд.
        e.Left.Accept(this);

        // Переходим к короткой схеме, если первый операнд в "ИСТИНУ".
        _builder.AppendJump(InstructionCode.JumpIfTrue, shortCircuitBlock);

        // Иначе вычисляем второй операнд.
        // Затем используем операцию "X <> 0", чтобы привести "X" к булеву значению (1 или 0).
        e.Right.Accept(this);
        _builder.Append(new Instruction(InstructionCode.Push, 0));
        _builder.Append(new Instruction(InstructionCode.NotEqual));
        _builder.AppendJump(InstructionCode.Jump, finalBlock);

        // Выполняем короткую схему вычислений: левый операнд обратился в "ИСТИНУ", и результат будет "ИСТИНА".
        _builder.InsertPoint = shortCircuitBlock;
        _builder.Append(new Instruction(InstructionCode.Push, 1));
        _builder.AppendJump(InstructionCode.Jump, finalBlock);

        _builder.InsertPoint = finalBlock;
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