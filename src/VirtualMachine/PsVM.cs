using Runtime;
using VirtualMachine.Builtins;
using VirtualMachine.Instructions;

namespace VirtualMachine;

public class PsVm
{
    private readonly BuiltinFunctions _builtinFunctions;
    private readonly IReadOnlyList<Instruction> _instructions;

    /// <summary>
    /// Указатель на текущую инструкцию.
    /// </summary>
    private int _instructionPointer;

    /// <summary>
    /// Код завершения программы.
    /// </summary>
    private int _exitCode;

    /// <summary>
    /// Стек для вычисления выражений и передачи аргументов функций.
    /// </summary>
    private readonly Stack<Value> _evaluationStack;

    /// <summary>
    /// Текущая таблица переменных.
    /// </summary>
    private VariablesTable? _variables;

    /// <summary>
    /// Стек с номерами инструкций, сохранённых перед вызовами незавершённых функций.
    /// </summary>
    private readonly Stack<ReturnContext> _returnStack;

    /// <summary>
    /// Результат работы программы (произвольное значение либо отсутствие значения).
    /// </summary>
    private Value _result;

    public PsVm(IEnvironment environment, IReadOnlyList<Instruction> instructions)
    {
        ValidateInstructions(instructions);

        _builtinFunctions = new BuiltinFunctions(environment);
        _instructions = instructions;
        _instructionPointer = 0;
        _exitCode = 0;
        _evaluationStack = new Stack<Value>();
        _variables = new VariablesTable();
        _returnStack = [];
        _result = Value.Unit;
    }

    public int ExitCode => _exitCode;

    public Value RunProgram()
    {
        while (true)
        {
            Instruction instruction = _instructions[_instructionPointer++];
            switch (instruction.Code)
            {
                case InstructionCode.Push:
                    _evaluationStack.Push(instruction.Operand);
                    break;

                case InstructionCode.Pop:
                    _evaluationStack.Pop();
                    break;

                case InstructionCode.StoreVar:
                    {
                        Value value = _evaluationStack.Pop();
                        string variableName = instruction.Operand.AsString();
                        _variables!.AssignVariable(variableName, value);
                    }

                    break;

                case InstructionCode.DefineVar:
                    {
                        Value value = _evaluationStack.Pop();
                        string variableName = instruction.Operand.AsString();
                        _variables!.DefineVariable(variableName, value);
                    }

                    break;

                case InstructionCode.LoadVar:
                    {
                        string variableName = instruction.Operand.AsString();
                        Value value = _variables!.GetVariable(variableName);
                        _evaluationStack.Push(value);
                    }

                    break;

                case InstructionCode.Add:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();

                        _evaluationStack.Push(
                            left.IsDouble() || right.IsDouble()
                                ? new Value(left.AsDouble() + right.AsDouble())
                                : new Value(left.AsLong() + right.AsLong())
                        );
                        break;
                    }

                case InstructionCode.Subtract:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();

                        _evaluationStack.Push(
                            left.IsDouble() || right.IsDouble()
                                ? new Value(left.AsDouble() - right.AsDouble())
                                : new Value(left.AsLong() - right.AsLong())
                        );
                        break;
                    }

                case InstructionCode.Multiply:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();

                        _evaluationStack.Push(
                            left.IsDouble() || right.IsDouble()
                                ? new Value(left.AsDouble() * right.AsDouble())
                                : new Value(left.AsLong() * right.AsLong())
                        );
                        break;
                    }

                case InstructionCode.Divide:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();

                        _evaluationStack.Push(
                            left.IsDouble() || right.IsDouble()
                                ? new Value(left.AsDouble() / right.AsDouble())
                                : new Value(left.AsLong() / right.AsLong())
                        );
                        break;
                    }

                case InstructionCode.Modulo:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();

                        _evaluationStack.Push(
                            left.IsDouble() || right.IsDouble()
                                ? new Value(left.AsDouble() % right.AsDouble())
                                : new Value(left.AsLong() % right.AsLong())
                        );
                        break;
                    }

                case InstructionCode.Power:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();

                        double result = Math.Pow(
                            left.AsDouble(),
                            right.AsDouble()
                        );

                        _evaluationStack.Push(
                            left.IsDouble() || right.IsDouble()
                                ? new Value(result)
                                : new Value((long)result)
                        );
                        break;
                    }

                case InstructionCode.And:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value((left.AsLong() != 0 && right.AsLong() != 0) ? 1 : 0));
                    }

                    break;

                case InstructionCode.Or:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value((left.AsLong() != 0 || right.AsLong() != 0) ? 1 : 0));
                    }

                    break;

                case InstructionCode.Not:
                    {
                        Value operand = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(operand.AsLong() == 0 ? 1 : 0));
                    }

                    break;

                case InstructionCode.Equal:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.Equals(right) ? 1 : 0));
                    }

                    break;

                case InstructionCode.NotEqual:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.Equals(right) ? 0 : 1));
                    }

                    break;

                case InstructionCode.Less:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.LessThan(right) ? 1 : 0));
                    }

                    break;

                case InstructionCode.LessOrEqual:
                    {
                        Value right = _evaluationStack.Pop();
                        Value left = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(left.LessThanOrEqual(right) ? 1 : 0));
                    }

                    break;

                case InstructionCode.Negate:
                    {
                        Value operand = _evaluationStack.Pop();
                        _evaluationStack.Push(new Value(
                            operand.IsLong() ? -operand.AsLong() : -operand.AsDouble()
                        ));
                    }

                    break;

                case InstructionCode.Jump:
                    {
                        _instructionPointer = (int)instruction.Operand.AsLong();
                    }

                    break;

                case InstructionCode.JumpIfTrue:
                    {
                        Value condition = _evaluationStack.Pop();
                        if (condition.AsLong() != 0)
                        {
                            _instructionPointer = (int)instruction.Operand.AsLong();
                        }
                    }

                    break;

                case InstructionCode.JumpIfFalse:
                    {
                        Value condition = _evaluationStack.Pop();
                        if (condition.AsLong() == 0)
                        {
                            _instructionPointer = (int)instruction.Operand.AsLong();
                        }
                    }

                    break;

                case InstructionCode.CallBuiltin:
                    CallBuiltin((BuiltinFunctionCode)instruction.Operand.AsLong());
                    break;

                case InstructionCode.StoreResult:
                    // Сохраняем результат работы всей программы.
                    _result = _evaluationStack.Pop();
                    break;

                case InstructionCode.Halt:
                    // Получаем код возврата программы.
                    _exitCode = (int)_evaluationStack.Pop().AsLong();
                    return _result;

                case InstructionCode.PushVars:
                    {
                        // Выполняем поиск родительской таблицы переменных.
                        int variableTableDepth = (int)instruction.Operand.AsLong();
                        VariablesTable? parentTable = (variableTableDepth != 0)
                            ? _variables!.GetAncestor(variableTableDepth)
                            : null;
                        _variables = new VariablesTable(parentTable);
                    }

                    break;

                case InstructionCode.PopVars:
                    _variables = _variables!.Parent;
                    break;

                default:
                    throw new NotImplementedException($"Unsupported instruction code: {instruction.Code}");
            }
        }
    }

    /// <summary>
    /// Выполняет вызов встроенной функции.
    /// </summary>
    private void CallBuiltin(BuiltinFunctionCode code)
    {
        switch (code)
        {
            case BuiltinFunctionCode.Print:
                _builtinFunctions.Print(_evaluationStack.Pop());
                break;

            case BuiltinFunctionCode.PrintI:
                _builtinFunctions.Printi(_evaluationStack.Pop());
                break;

            case BuiltinFunctionCode.PrintF:
                {
                    Value precision = _evaluationStack.Pop();
                    Value value = _evaluationStack.Pop();
                    _builtinFunctions.Printf(value, precision);
                }

                break;

            case BuiltinFunctionCode.ItoS:
                _evaluationStack.Push(_builtinFunctions.Itos(_evaluationStack.Pop()));
                break;

            case BuiltinFunctionCode.FtoS:
                {
                    Value precision = _evaluationStack.Pop();
                    Value value = _evaluationStack.Pop();
                    _evaluationStack.Push(_builtinFunctions.Ftos(value, precision));
                }

                break;

            case BuiltinFunctionCode.FtoI:
                _evaluationStack.Push(_builtinFunctions.Ftoi(_evaluationStack.Pop()));
                break;

            case BuiltinFunctionCode.ItoF:
                _evaluationStack.Push(_builtinFunctions.Itof(_evaluationStack.Pop()));
                break;

            case BuiltinFunctionCode.SConcat:
                {
                    Value s2 = _evaluationStack.Pop();
                    Value s1 = _evaluationStack.Pop();
                    _evaluationStack.Push(_builtinFunctions.Sconcat(s1, s2));
                }

                break;

            case BuiltinFunctionCode.StrLen:
                _evaluationStack.Push(_builtinFunctions.Strlen(_evaluationStack.Pop()));
                break;

            case BuiltinFunctionCode.SubStr:
                {
                    Value to = _evaluationStack.Pop();
                    Value from = _evaluationStack.Pop();
                    Value s = _evaluationStack.Pop();
                    _evaluationStack.Push(_builtinFunctions.Substr(s, from, to));
                }

                break;

            case BuiltinFunctionCode.StoI:
                _evaluationStack.Push(_builtinFunctions.Stoi(_evaluationStack.Pop()));
                break;

            case BuiltinFunctionCode.StoF:
                _evaluationStack.Push(_builtinFunctions.Stof(_evaluationStack.Pop()));
                break;

            case BuiltinFunctionCode.Input:
                _evaluationStack.Push(_builtinFunctions.Input());
                break;

            default:
                throw new ArgumentException($"Unknown builtin function: {code}");
        }
    }

    private static void ValidateInstructions(IReadOnlyList<Instruction> instructions)
    {
        if (instructions.Count == 0)
        {
            throw new InvalidOperationException("Invalid empty VM program");
        }

        InstructionCode lastInstructionCode = instructions[^1].Code;
        if (lastInstructionCode != InstructionCode.Halt)
        {
            throw new InvalidOperationException($"Last instruction must be {InstructionCode.Halt},");
        }
    }

    private record struct ReturnContext(
        int InstructionPointer,
        VariablesTable? Variables
    );
}