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

    private readonly Dictionary<string, Value> _locals;

    /// <summary>
    /// Словарь для хранения локальных переменных по имени.
    /// </summary>
    public PsVm(IEnvironment environment, IReadOnlyList<Instruction> instructions)
    {
        ValidateInstructions(instructions);

        _builtinFunctions = new BuiltinFunctions(environment);
        _instructions = instructions;
        _instructionPointer = 0;
        _exitCode = 0;
        _evaluationStack = new Stack<Value>();
        _locals = new Dictionary<string, Value>();
    }

    public int ExitCode => _exitCode;

    public int RunProgram()
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

                case InstructionCode.Negate:
                    {
                        Value operand = _evaluationStack.Pop();
                        _evaluationStack.Push(
                            operand.IsLong() ? new Value(-operand.AsLong()) : new Value(-operand.AsDouble())
                        );
                    }

                    break;

                case InstructionCode.CallBuiltin:
                    CallBuiltin((BuiltinFunctionCode)instruction.Operand.AsLong());
                    break;

                case InstructionCode.Halt:
                    _exitCode = (int)_evaluationStack.Pop().AsLong();
                    return _exitCode;

                case InstructionCode.LoadLocal:
                    {
                        string varName = instruction.Operand.AsString();
                        if (!_locals.TryGetValue(varName, out Value? value))
                        {
                            throw new InvalidOperationException($"Variable '{varName}' is not defined");
                        }

                        _evaluationStack.Push(value);
                        break;
                    }

                case InstructionCode.StoreLocal:
                    {
                        string varName = instruction.Operand.AsString();
                        _locals[varName] = _evaluationStack.Pop();
                        break;
                    }

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
}