using Runtime;
using Tests.TestLibrary;
using VirtualMachine.Builtins;
using VirtualMachine.Instructions;

namespace VirtualMachine.UnitTests;

public class VariablesTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetVariableAssignmentData))]
    public void Can_assign_and_load_variables_of_different_types(List<Instruction> instructions, string expected)
    {
        FakeEnvironment environment = new();
        PsVm vm = new(environment, instructions);

        vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    public static TheoryData<List<Instruction>, string> GetVariableAssignmentData()
    {
        return new TheoryData<List<Instruction>, string>
        {
            {
                [
                    new Instruction(InstructionCode.Push, 42),
                    new Instruction(InstructionCode.StoreLocal, new Value("x")),
                    new Instruction(InstructionCode.LoadLocal, new Value("x")),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintI)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "42"
            },
            {
                [
                    new Instruction(InstructionCode.Push, new Value(3.14)),
                    new Instruction(InstructionCode.StoreLocal, new Value("pi")),
                    new Instruction(InstructionCode.LoadLocal, new Value("pi")),
                    new Instruction(InstructionCode.Push, new Value(2)),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintF)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "3.14"
            },
            {
                [
                    new Instruction(InstructionCode.Push, new Value("Hello, World!")),
                    new Instruction(InstructionCode.StoreLocal, new Value("msg")),
                    new Instruction(InstructionCode.LoadLocal, new Value("msg")),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.Print)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "Hello, World!"
            },
            {
                [
                    new Instruction(InstructionCode.Push, 100),
                    new Instruction(InstructionCode.StoreLocal, new Value("var")),
                    new Instruction(InstructionCode.Push, new Value("now a string")),
                    new Instruction(InstructionCode.StoreLocal, new Value("var")),
                    new Instruction(InstructionCode.LoadLocal, new Value("var")),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.Print)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "now a string"
            },
        };
    }
}