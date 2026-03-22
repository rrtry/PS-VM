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
    /// Добавляет инструкцию в текущий базовый блок.
    /// Инструкции перехода добавляются другим методом.
    /// </summary>
    public void Append(Instruction instruction)
    {
        _insertPoint.Append(instruction);
    }

    /// <summary>
    /// Собирает финальный список инструкций из базовых блоков, заменяя адреса во всех инструкциях перехода
    ///  на окончательные адреса инструкций.
    /// </summary>
    public List<Instruction> Finish()
    {
        List<Instruction> instructions = [];
        foreach (BasicBlock block in _basicBlocks)
        {
            foreach (Instruction instruction in block.Instructions)
            {
                instructions.Add(instruction);
            }
        }

        return instructions;
    }

    /// <summary>
    /// Создаёт базовый блок инструкций и возвращает ссылку на него.
    /// </summary>
    private BasicBlock CreateBasicBlock()
    {
        BasicBlock bb = new(_basicBlocks.Count);
        _basicBlocks.Add(bb);
        return bb;
    }
}