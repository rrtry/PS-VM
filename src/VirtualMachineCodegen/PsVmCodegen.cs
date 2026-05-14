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

    /// <summary>
    /// Стек со ссылками на блоки после текущих циклов (while и for).
    /// Используется для генерации прерывания цикла (break).
    /// </summary>
    private readonly Stack<BasicBlock> _currentLoopFinalBlockStack = new();

    /// <summary>
    /// Стек со ссылками на блоки после текущих циклов (while и for).
    /// Используется для генерации прерывания цикла (continue).
    /// </summary>
    private readonly Stack<BasicBlock> _currentLoopContinueBlockStack = new();
    private readonly Stack<Dictionary<string, string>> _shadowStack = new();
    private readonly Dictionary<string, BasicBlock> _functions = new();
    private readonly InstructionsBuilder _builder = new();
    private int _uniqueCounter;

    private string _currentFunction;
    private bool _hasReturn;

    public List<Instruction> GenerateCode(EntryPointNode program)
    {
        program.Accept(this);
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
        foreach (Expression argument in e.Function is FunctionDeclaration ? e.Arguments.Reverse() : e.Arguments)
        {
            argument.Accept(this);
        }

        switch (e.Function)
        {
            case NativeFunction native:
                _builder.Append(new Instruction(InstructionCode.CallBuiltin, GetBuiltinFunctionCode(native.Name)));
                break;

            case FunctionDeclaration:
                BasicBlock functionBlock = _functions[e.Function.Name];
                _builder.AppendJump(InstructionCode.Call, functionBlock);
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
        foreach (FunctionDeclaration decl in n.Functions)
        {
            if (decl.Name != "main")
            {
                decl.Accept(this);
            }
        }

        n.Main!.Accept(this);
    }

    public void Visit(FunctionDeclaration d)
    {
        bool isEntryPoint = d.Name == "main";

        _currentFunction = d.Name;
        _hasReturn = false;

        BasicBlock previousBlock = _builder.InsertPoint;
        BasicBlock functionBlock = _builder.CreateBasicBlock();

        functionBlock.IsEntryPoint = isEntryPoint;

        _builder.InsertPoint = functionBlock;
        _functions[d.Name] = functionBlock;

        PushScope();
        _builder.Append(new Instruction(InstructionCode.PushVars));

        foreach (AbstractParameterDeclaration paramDecl in d.Parameters)
        {
            ParameterDeclaration param = (ParameterDeclaration)paramDecl;
            DefineVariable(param.Name, out string mappedName);
            _builder.Append(new Instruction(InstructionCode.DefineLocal, mappedName));
        }

        d.Body.Accept(this);

        if (!_hasReturn && !isEntryPoint)
        {
            _builder.Append(new Instruction(InstructionCode.Push, Value.Unit));
            _builder.Append(new Instruction(InstructionCode.PopVars));
            _builder.Append(new Instruction(InstructionCode.Return));
        }

        PopScope(); // PopVars выполняется в ReturnStatement
        _builder.InsertPoint = previousBlock;
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

        _builder.Append(new Instruction(InstructionCode.PopVars));

        if (_currentFunction == "main")
        {
            _builder.Append(new Instruction(InstructionCode.Halt));
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

    public void Visit(WhileLoopStatement s)
    {
        BasicBlock loopBlock = _builder.CreateBasicBlock();
        BasicBlock finalBlock = _builder.CreateBasicBlock();

        _currentLoopFinalBlockStack.Push(finalBlock);
        _currentLoopContinueBlockStack.Push(loopBlock);

        // Переход в начало цикла.
        _builder.AppendJump(InstructionCode.Jump, loopBlock);
        _builder.InsertPoint = loopBlock;

        // Проверяем условие и завершаем цикл, если оно ложно.
        s.Condition.Accept(this);
        _builder.AppendJump(InstructionCode.JumpIfFalse, finalBlock);

        // Генерируем тело цикла и переход к началу цикла.
        s.LoopBody.Accept(this);
        _builder.AppendJump(InstructionCode.Jump, loopBlock);

        _currentLoopFinalBlockStack.Pop();
        _currentLoopContinueBlockStack.Pop();

        _builder.InsertPoint = finalBlock;
    }

    public void Visit(ForLoopStatement s)
    {
        BasicBlock loopBlock = _builder.CreateBasicBlock();      // проверка условия
        BasicBlock continueBlock = _builder.CreateBasicBlock();  // инкремент и переход
        BasicBlock finalBlock = _builder.CreateBasicBlock();

        _currentLoopFinalBlockStack.Push(finalBlock);
        _currentLoopContinueBlockStack.Push(continueBlock);

        PushScope();

        // Инициализация итератора
        s.Counter.Accept(this);
        string iteratorName = GetMappedName(s.Iterator);

        _builder.AppendJump(InstructionCode.Jump, loopBlock);
        _builder.InsertPoint = loopBlock;

        // Проверка условия
        _builder.Append(new Instruction(InstructionCode.LoadLocal, iteratorName));
        s.Condition.Accept(this);
        _builder.AppendJump(InstructionCode.JumpIfFalse, finalBlock);

        // Тело цикла
        s.LoopBody.Accept(this);

        // Переход на блок continue (инкремент и повтор)
        _builder.AppendJump(InstructionCode.Jump, continueBlock);

        // Блок continue: инкремент и переход к проверке условия
        _builder.InsertPoint = continueBlock;
        s.Update?.Accept(this);
        _builder.AppendJump(InstructionCode.Jump, loopBlock);

        _currentLoopFinalBlockStack.Pop();
        _currentLoopContinueBlockStack.Pop();
        _builder.InsertPoint = finalBlock;

        PopScope();
    }

    public void Visit(BreakStatement s)
    {
        BasicBlock loopFinalBlock = _currentLoopFinalBlockStack.Peek();
        _builder.AppendJump(InstructionCode.Jump, loopFinalBlock);
    }

    public void Visit(ContinueStatement s)
    {
        BasicBlock continueBlock = _currentLoopContinueBlockStack.Peek();
        _builder.AppendJump(InstructionCode.Jump, continueBlock);
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

    private void GenerateBinaryOperationCode(Expression left, Expression right, InstructionCode code)
    {
        left.Accept(this);
        right.Accept(this);
        _builder.Append(new Instruction(code));
    }

    private void PushScope()
    {
        _shadowStack.Push(new Dictionary<string, string>());
    }

    private void PopScope()
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