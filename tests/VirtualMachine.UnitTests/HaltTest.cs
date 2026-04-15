using Tests.TestLibrary;
using VirtualMachine.Instructions;

namespace VirtualMachine.UnitTests;

public class HaltTest
{
    [Theory]
    [MemberData(nameof(GetHaltVmData))]
    public void Can_halt_VM(int exitCode)
    {
        FakeEnvironment environment = new();
        PsVm vm = new(environment, [
            new Instruction(InstructionCode.Push, exitCode),
            new Instruction(InstructionCode.Halt),
        ]);

        vm.RunProgram();
        Assert.Equal(exitCode, vm.ExitCode);
        Assert.Empty(environment.OutputBuffer);
    }

    public static TheoryData<int> GetHaltVmData()
    {
        return
        [
            0, // Остановка виртуальной машины с нулевым кодом
            1, // Остановка виртуальной машины с ненулевым кодом
        ];
    }
}