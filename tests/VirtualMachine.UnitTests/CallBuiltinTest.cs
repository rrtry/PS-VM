using Runtime;
using Tests.TestLibrary.TestDoubles;
using VirtualMachine.Builtins;
using VirtualMachine.Instructions;

namespace VirtualMachine.UnitTests;

public class CallBuiltinTest
{
    [Theory]
    [MemberData(nameof(GetUseInputAndOutputData))]
    public void Can_use_input_and_output(
        List<Instruction> program,
        string input,
        string expectedBufferedOutput
    )
    {
        FakeEnvironment environment = new();
        environment.AddInput(input);

        PsVm vm = new(environment, program);
        Value result = vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(Value.Unit, result);
        Assert.Equal(expectedBufferedOutput, environment.OutputBuffer);
    }

    [Theory]
    [MemberData(nameof(GetCallBuiltinFunctionsData))]
    public void Can_call_builtin_functions(
        List<Instruction> program,
        string expectedBufferedOutput
    )
    {
        FakeEnvironment environment = new();
        PsVm vm = new(environment, program);
        Value result = vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(Value.Unit, result);
        Assert.Equal(expectedBufferedOutput, environment.OutputBuffer);
    }

    public static TheoryData<List<Instruction>, string> GetCallBuiltinFunctionsData()
    {
        return new TheoryData<List<Instruction>, string>()
        {
            // Функция strlen: strlen("Hello, world!") = 13
            {
                [
                    new Instruction(InstructionCode.Push, "Hello, world!"),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.StrLen),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.PrintI),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "13"
            },

            // Функция substr: substr("Cogito, ergo sum", 0, 6) = "Cogito"
            {
                [
                    new Instruction(InstructionCode.Push, "Cogito, ergo sum"),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Push, 6),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.SubStr),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "Cogito"
            },

            // Функция concat: sconcat("Cogito", " ergo sum") = "Cogito ergo sum"
            {
                [
                    new Instruction(InstructionCode.Push, "Cogito"),
                    new Instruction(InstructionCode.Push, " ergo sum"),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.SConcat),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "Cogito ergo sum"
            },
        };
    }

    public static TheoryData<List<Instruction>, string, string> GetUseInputAndOutputData()
    {
        return new TheoryData<List<Instruction>, string, string>()
        {
            // Функция printf
            {
                [
                    new Instruction(InstructionCode.Push, new Value(3.14159265359)),
                    new Instruction(InstructionCode.Push, new Value(2)),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.PrintF),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                string.Empty, "3.14"
            },

            // Функция printi
            {
                [
                    new Instruction(InstructionCode.Push, new Value(2147483647)),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.PrintI),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                string.Empty, "2147483647"
            },

            // Функция print
            {
                [
                    new Instruction(InstructionCode.Push, "Hello, world!"),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                string.Empty, "Hello, world!"
            },
        };
    }
}