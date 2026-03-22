using Runtime;
using Tests.TestLibrary.TestDoubles;
using VirtualMachine.Builtins;
using VirtualMachine.Instructions;

namespace VirtualMachine.UnitTests;

public class EvaluationTest
{
    [Theory]
    [MemberData(nameof(GetEvaluateExpressionData))]
    public void Can_evaluate_expression(List<Instruction> instructions, string expected)
    {
        FakeEnvironment environment = new();
        PsVm vm = new(environment, instructions);

        vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(expected, environment.OutputBuffer);
    }

    public static TheoryData<List<Instruction>, string> GetEvaluateExpressionData()
    {
        return new TheoryData<List<Instruction>, string>
        {
            // Push + Print
            {
                [
                    new Instruction(InstructionCode.Push, 42),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintI)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "42"
            },

            // +
            {
                [
                    new Instruction(InstructionCode.Push, 10),
                    new Instruction(InstructionCode.Push, 20),
                    new Instruction(InstructionCode.Add),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintI)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "30"
            },

            // -
            {
                [
                    new Instruction(InstructionCode.Push, 50),
                    new Instruction(InstructionCode.Push, 30),
                    new Instruction(InstructionCode.Subtract),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintI)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "20"
            },

            // *
            {
                [
                    new Instruction(InstructionCode.Push, 7),
                    new Instruction(InstructionCode.Push, 6),
                    new Instruction(InstructionCode.Multiply),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintI)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "42"
            },

            // '/'
            {
                [
                    new Instruction(InstructionCode.Push, 100),
                    new Instruction(InstructionCode.Push, 4),
                    new Instruction(InstructionCode.Divide),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintI)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "25"
            },

            // **
            {
                [
                    new Instruction(InstructionCode.Push, 2),
                    new Instruction(InstructionCode.Push, 10),
                    new Instruction(InstructionCode.Power),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintI)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "1024"
            },

            // %
            {
                [
                    new Instruction(InstructionCode.Push, 17),
                    new Instruction(InstructionCode.Push, 5),
                    new Instruction(InstructionCode.Modulo),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintI)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "2"
            },

            // -
            {
                [
                    new Instruction(InstructionCode.Push, 42),
                    new Instruction(InstructionCode.Negate),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintI)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "-42"
            },

            // Pop
            {
                [
                    new Instruction(InstructionCode.Push, 10),
                    new Instruction(InstructionCode.Push, 20),
                    new Instruction(InstructionCode.Pop),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintI)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "10"
            },

            // (10 + 5) * 3
            {
                [
                    new Instruction(InstructionCode.Push, 10),
                    new Instruction(InstructionCode.Push, 5),
                    new Instruction(InstructionCode.Add),
                    new Instruction(InstructionCode.Push, 3),
                    new Instruction(InstructionCode.Multiply),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintI)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "45"
            },

            // (100 / 4) + (2 ** 3) * 2
            {
                [
                    new Instruction(InstructionCode.Push, 100),
                    new Instruction(InstructionCode.Push, 4),
                    new Instruction(InstructionCode.Divide),
                    new Instruction(InstructionCode.Push, 2),
                    new Instruction(InstructionCode.Push, 3),
                    new Instruction(InstructionCode.Power),
                    new Instruction(InstructionCode.Push, 2),
                    new Instruction(InstructionCode.Multiply),
                    new Instruction(InstructionCode.Add),
                    new Instruction(InstructionCode.CallBuiltin, new Value((int)BuiltinFunctionCode.PrintI)),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "41"
            },
        };
    }
}