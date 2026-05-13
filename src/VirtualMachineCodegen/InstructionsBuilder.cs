using VirtualMachine.Instructions;

namespace VirtualMachineCodegen;

public class InstructionsBuilder
{
    private readonly List<BasicBlock> _basicBlocks;
    private BasicBlock _insertPoint;

    public InstructionsBuilder()
    {
        _basicBlocks = [];
        _insertPoint = CreateBasicBlock();
    }

    /// <summary>
    /// Базовый блок, в который выполняется вставка инструкций
    /// </summary>
    public BasicBlock InsertPoint
    {
        get => _insertPoint;

        set
        {
            if (!ReferenceEquals(_basicBlocks[value.Id], value))
            {
                // Такого не должно быть по логике кодогенерации.
                throw new InvalidOperationException("Basic block does not belong to the current instructions builder");
            }

            _insertPoint = value;
        }
    }

    /// <summary>
    /// Собирает финальный список инструкций из базовых блоков, заменяя адреса во всех инструкциях перехода
    ///  на окончательные адреса инструкций.
    /// </summary>
    public List<Instruction> Finish()
    {
        int entryPointIndex = _basicBlocks.FindIndex(block => block.IsEntryPoint);
        BasicBlock entryPointBlock = _basicBlocks[entryPointIndex];

        _basicBlocks.RemoveAt(entryPointIndex);
        _basicBlocks.Insert(0, entryPointBlock);

        Dictionary<int, int> blockAddress = CalculateBasicBlockAddresses();
        List<Instruction> instructions = new List<Instruction>();

        foreach (BasicBlock block in _basicBlocks)
        {
            foreach (Instruction instruction in block.Instructions)
            {
                if (IsJump(instruction.Code))
                {
                    int targetBlockId = (int)instruction.Operand.AsLong();
                    int newAddress = blockAddress[targetBlockId];
                    instructions.Add(new Instruction(instruction.Code, newAddress));
                }
                else
                {
                    instructions.Add(instruction);
                }
            }
        }

        return instructions;
    }

    /// <summary>
    /// Добавляет инструкцию в текущий базовый блок.
    /// Инструкции перехода добавляются другим методом.
    /// </summary>
    public void Append(Instruction instruction)
    {
        if (IsJump(instruction.Code))
        {
            throw new InvalidOperationException($"Cannot append {instruction.Code} using this method");
        }

        _insertPoint.Append(instruction);
    }

    /// <summary>
    /// Добавляет инструкцию перехода на указанный базовый блок.
    /// </summary>
    public void AppendJump(InstructionCode code, BasicBlock target)
    {
        if (!IsJump(code))
        {
            throw new InvalidOperationException($"Instruction {code} is not a jump instruction");
        }

        _insertPoint.Append(new Instruction(code, target.Id));
    }

    /// <summary>
    /// Создаёт базовый блок инструкций и возвращает ссылку на него.
    /// </summary>
    public BasicBlock CreateBasicBlock()
    {
        BasicBlock bb = new(_basicBlocks.Count);
        _basicBlocks.Add(bb);

        return bb;
    }

    /// <summary>
    /// Проверяет, является ли указанная инструкция переходом.
    /// </summary>
    private bool IsJump(InstructionCode code)
    {
        return code switch
        {
            InstructionCode.Call => true,
            InstructionCode.Jump => true,
            InstructionCode.JumpIfFalse => true,
            InstructionCode.JumpIfTrue => true,

            _ => false,
        };
    }

    /// <summary>
    /// Вычисляет адреса последовательно расположенных базовых блоков инструкций.
    /// </summary>
    private Dictionary<int, int> CalculateBasicBlockAddresses()
    {
        Dictionary<int, int> blockAddress = new Dictionary<int, int>();
        int currentAddress = 0;

        foreach (BasicBlock block in _basicBlocks)
        {
            blockAddress[block.Id] = currentAddress;
            currentAddress += block.Instructions.Count;
        }

        return blockAddress;
    }
}