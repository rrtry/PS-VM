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

    public List<Instruction> Finish()
    {
        int haltBlockIndex = _basicBlocks.FindIndex(b => b.IsHaltBlock);
        BasicBlock haltBlock = _basicBlocks[haltBlockIndex];

        _basicBlocks.RemoveAt(haltBlockIndex);
        _basicBlocks.Add(haltBlock);

        Dictionary<int, int> blockAddress = CalculateBasicBlockAddresses();
        List<Instruction> instructions = new List<Instruction>();

        foreach (BasicBlock block in _basicBlocks)
        {
            foreach (Instruction instr in block.Instructions)
            {
                if (IsJump(instr.Code))
                {
                    int targetBlockId = (int)instr.Operand.AsLong();
                    int targetAddress = blockAddress[targetBlockId];
                    instructions.Add(new Instruction(instr.Code, targetAddress));
                }
                else
                {
                    instructions.Add(instr);
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
            // TODO: Call в 5-ой итерации
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