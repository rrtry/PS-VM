using Ast.Declarations;
using VirtualMachine.Instructions;

namespace VirtualMachineCodegen;

public class FunctionInfo
{
    public FunctionInfo(string name, IReadOnlyList<AbstractParameterDeclaration> parameters)
    {
        Name = name;
        Parameters = parameters;
    }

    public string Name { get; }

    public IReadOnlyList<AbstractParameterDeclaration> Parameters { get; }

    public List<Instruction> Instructions { get; set; }

    public int StartAddress { get; set; } = -1;
}