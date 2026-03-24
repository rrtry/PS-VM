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
        };

    private readonly InstructionsBuilder _builder = new();

    public List<Instruction> GenerateCode(EntryPointNode program)
    {
        program.Main.Accept(this);
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

    public void Visit(BlockStatement s)
    {
        GenerateBlockStatementCode(s.Statements);
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
        if (s.ReturnValue == null)
        {
            _builder.Append(new Instruction(InstructionCode.Push, Value.Unit));
        }
        else
        {
            s.ReturnValue?.Accept(this);
        }
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

    private static int GetBuiltinFunctionCode(string name)
    {
        if (!BuiltinFunctionsMap.TryGetValue(name, out BuiltinFunctionCode code))
        {
            throw new NotImplementedException($"Unsupported builtin function {name}");
        }

        return (int)code;
    }
}