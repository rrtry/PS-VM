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

    private readonly Stack<Dictionary<string, string>> _shadowStack = new();
    private readonly Dictionary<string, FunctionInfo> _functions = new();
    private int _uniqueCounter;

    private FunctionInfo? _mainFunction;

    private InstructionsBuilder _builder = new();
    private FunctionInfo? _currentFunction;
    private InstructionsBuilder? _savedBuilder;

    private BasicBlock? _exitBlock;
    private bool _hasReturn = false;

    public List<Instruction> GenerateCode(EntryPointNode program)
    {
        /*
        program.Accept(this);
        return _builder.Finish(); */

        foreach (FunctionDeclaration decl in program.Functions)
        {
            if (decl.Name != "main")
            {
                decl.Accept(this);
            }
        }

        program.Main.Accept(this);
        return GenerateProgram();
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

            case FunctionDeclaration function:
                _builder.Append(new Instruction(InstructionCode.Call, new Value(function.Name)));
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
        bool isEntryPoint = d.Name == "main";
        if (isEntryPoint)
        {
            _currentFunction = new FunctionInfo("main", []);

            _hasReturn = false;
            _builder.Append(new Instruction(InstructionCode.PushVars));

            _exitBlock = _builder.CreateBasicBlock();
            _exitBlock.IsHaltBlock = true;

            d.Body.Accept(this);

            if (!_hasReturn)
            {
                _builder.AppendJump(InstructionCode.Jump, _exitBlock);
            }

            _builder.InsertPoint = _exitBlock;
            _builder.Append(new Instruction(InstructionCode.PopVars));
            _builder.Append(new Instruction(InstructionCode.Halt));

            _mainFunction = _currentFunction;
            _mainFunction.Instructions = _builder.Finish();

            _exitBlock = null;
            _currentFunction = null;

            return;
        }

        GenerateUserFunction(d);
    }

    public void Visit(ReturnStatement s)
    {
        _hasReturn = true;

        if (s.ReturnValue != null)
        {
            s.ReturnValue.Accept(this);
        }
        else
        {
            _builder.Append(new Instruction(InstructionCode.Push, Value.Unit));
        }

        if (_currentFunction == null || _currentFunction.Name == "main")
        {
            _builder.AppendJump(InstructionCode.Jump, _exitBlock!);
        }
        else
        {
            _builder.Append(new Instruction(InstructionCode.Return));
        }
    }

    public void Visit(IdentifierExpression e)
    {
        string mappedName = GetMappedName(e.Name);
        _builder.Append(new Instruction(InstructionCode.LoadLocal, mappedName));
    }

    public void Visit(VariableDeclaration d)
    {
        d.Initializer.Accept(this);
        DefineVariable(d.Name, out string mappedName);
        _builder.Append(new Instruction(InstructionCode.DefineLocal, mappedName));
    }

    public void Visit(AssignmentStatement s)
    {
        IdentifierExpression lvalue = (IdentifierExpression)s.Left;
        string mappedName = GetMappedName(lvalue.Name);
        s.Right.Accept(this);
        _builder.Append(new Instruction(InstructionCode.StoreLocal, mappedName));
    }

    public void Visit(ParameterDeclaration d)
    {
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

    private List<Instruction> GenerateProgram()
    {
        if (_mainFunction == null)
        {
            throw new InvalidOperationException("Entry point 'main' not found");
        }

        List<Instruction> allInstructions = new List<Instruction>();
        Dictionary<string, int> functionAddresses = new Dictionary<string, int>();

        functionAddresses["main"] = 0;
        allInstructions.AddRange(_mainFunction.Instructions);

        foreach (KeyValuePair<string, FunctionInfo> kv in _functions)
        {
            if (kv.Key == "main")
            {
                continue;
            }

            functionAddresses[kv.Key] = allInstructions.Count;
            allInstructions.AddRange(kv.Value.Instructions);
        }

        for (int i = 0; i < allInstructions.Count; i++)
        {
            Instruction instr = allInstructions[i];
            if (instr.Code == InstructionCode.Call && instr.Operand.IsString())
            {
                string funcName = instr.Operand.AsString();

                if (!functionAddresses.TryGetValue(funcName, out int addr))
                {
                    throw new InvalidOperationException($"Undefined function '{funcName}'");
                }

                allInstructions[i] = new Instruction(InstructionCode.Call, new Value(addr));
            }
        }

        return allInstructions;
    }

    private void GenerateUserFunction(FunctionDeclaration d)
    {
        _savedBuilder = _builder;

        InstructionsBuilder funcBuilder = new InstructionsBuilder();
        _builder = funcBuilder;
        _currentFunction = new FunctionInfo(d.Name, d.Parameters);

        EnterBlock();
        _builder.Append(new Instruction(InstructionCode.PushVars));

        foreach (AbstractParameterDeclaration paramDecl in d.Parameters)
        {
            ParameterDeclaration param = (ParameterDeclaration)paramDecl;
            DefineVariable(param.Name, out string mappedName);
            _builder.Append(new Instruction(InstructionCode.DefineLocal, mappedName));
        }

        bool savedHasReturn = _hasReturn;
        _hasReturn = false;
        d.Body.Accept(this);

        if (!_hasReturn)
        {
            _builder.Append(new Instruction(InstructionCode.Push, Value.Unit));
            _builder.Append(new Instruction(InstructionCode.Return));
        }

        _hasReturn = savedHasReturn;
        ExitBlock();

        _currentFunction.Instructions = funcBuilder.Finish();
        _functions[d.Name] = _currentFunction;

        _builder = _savedBuilder;
        _savedBuilder = null;
        _currentFunction = null;
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
        EnterBlock();

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

        ExitBlock();
    }

    private void GenerateBinaryOperationCode(Expression left, Expression right, InstructionCode code)
    {
        left.Accept(this);
        right.Accept(this);
        _builder.Append(new Instruction(code));
    }

    private void EnterBlock()
    {
        _shadowStack.Push(new Dictionary<string, string>());
    }

    private void ExitBlock()
    {
        _shadowStack.Pop();
    }

    private string GetMappedName(string originalName)
    {
        foreach (Dictionary<string, string> map in _shadowStack)
        {
            if (map.TryGetValue(originalName, out string? mapped))
            {
                return mapped;
            }
        }

        throw new InvalidOperationException($"Variable '{originalName}' not found");
    }

    private void DefineVariable(string originalName, out string mappedName)
    {
        if (_shadowStack.Count == 0)
        {
            throw new InvalidOperationException("No block scope");
        }

        // Уникальное имя переменной с индикатором вложенности, вместо вложенных областей видимости для каждого блока
        mappedName = $"{originalName}__{_uniqueCounter++}";
        _shadowStack.Peek()[originalName] = mappedName;
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