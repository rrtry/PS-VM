using System.Text;

using Runtime;

namespace VirtualMachine.Instructions;

public class Instruction
{
    public Instruction(InstructionCode code)
    {
        Code = code;
        Operand = Value.Unit;
    }

    public Instruction(InstructionCode code, int value)
    {
        Code = code;
        Operand = new Value(value);
    }

    public Instruction(InstructionCode code, string value)
    {
        Code = code;
        Operand = new Value(value);
    }

    public Instruction(InstructionCode code, Value value)
    {
        Code = code;
        Operand = value;
    }

    public InstructionCode Code { get; }

    public Value Operand { get; }

    /// <summary>
    /// Печатает инструкцию в формате "Code Operand" либо просто "Code".
    /// </summary>
    public override string ToString()
    {
        return "";
        /*
        StringBuilder sb = new();
        sb.Append(Code);
        if (!Operand.IsVoid())
        {
            sb.Append(' ');
            sb.Append(Operand);
        }

        return sb.ToString(); */
    }
}