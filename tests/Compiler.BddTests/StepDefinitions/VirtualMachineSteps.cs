using Compiler.BddTests.Support;
using FluentAssertions;
using Runtime;
using TechTalk.SpecFlow;
using VirtualMachine;
using VirtualMachine.Instructions;

namespace Compiler.BddTests.StepDefinitions;

[Binding]
public class VirtualMachineSteps
{
    private readonly CompilerTestContext _context;

    public VirtualMachineSteps(CompilerTestContext context)
    {
        _context = context;
    }

    [Given(@"VM instructions \[(.*)\]")]
    public void GivenVMInstructions(string instructionList)
    {
        _context.Instructions = ParseInstructions(instructionList);
    }

    [When(@"the VM executes")]
    public void WhenTheVmExecutes()
    {
        try
        {
            var vm = new PsVm(_context.Environment, _context.Instructions);
            _context.ExitCode = vm.RunProgram();
        }
        catch (Exception ex)
        {
            _context.RuntimeError = ex;
        }
    }

    [Then(@"the stack should contain (.*)")]
    public void ThenTheStackShouldContain(long expectedValue)
    {
        _context.RuntimeError.Should().BeNull("VM should not throw an error");
        _context.Environment.Evaluated.Should().Contain(expectedValue.ToString());
    }

    [Then(@"the stack should contain (.*)")]
    public void ThenTheStackShouldContainValue(int expectedValue)
    {
        ThenTheStackShouldContain((long)expectedValue);
    }

    [Then(@"the stack should be empty")]
    public void ThenTheStackShouldBeEmpty()
    {
        _context.RuntimeError.Should().BeNull("VM should not throw an error");
    }

    [Then(@"the exit code should be (.*)")]
    public void ThenTheExitCodeShouldBe(int expectedExitCode)
    {
        _context.ExitCode.Should().Be(expectedExitCode);
    }

    private List<Instruction> ParseInstructions(string instructionList)
    {
        var result = new List<Instruction>();
        var parts = instructionList.Split(", ", StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            var trimmed = part.Trim();

            if (trimmed.StartsWith("Push(") && trimmed.EndsWith(")"))
            {
                var valueStr = trimmed[5..^1];
                var value = ParseValue(valueStr);
                result.Add(new Instruction(InstructionCode.Push, value));
            }
            else if (trimmed.StartsWith("StoreLocal(") && trimmed.EndsWith(")"))
            {
                var name = trimmed[11..^1].Trim('"');
                result.Add(new Instruction(InstructionCode.StoreLocal, name));
            }
            else if (trimmed.StartsWith("LoadLocal(") && trimmed.EndsWith(")"))
            {
                var name = trimmed[10..^1].Trim('"');
                result.Add(new Instruction(InstructionCode.LoadLocal, name));
            }
            else if (trimmed.StartsWith("CallBuiltin(") && trimmed.EndsWith(")"))
            {
                var funcName = trimmed[12..^1];
                result.Add(new Instruction(InstructionCode.CallBuiltin, (int)GetBuiltinCode(funcName)));
            }
            else
            {
                var code = Enum.Parse<InstructionCode>(trimmed);
                result.Add(new Instruction(code));
            }
        }

        return result;
    }

    private static object ParseValue(string valueStr)
    {
        if (int.TryParse(valueStr, out int intValue))
            return new Value(intValue);
        if (long.TryParse(valueStr, out long longValue))
            return new Value(longValue);
        if (double.TryParse(valueStr, out double doubleValue))
            return new Value(doubleValue);
        if (valueStr.StartsWith('"') && valueStr.EndsWith('"'))
            return new Value(valueStr[1..^1]);
        return new Value(valueStr);
    }

    private static BuiltinFunctionCode GetBuiltinCode(string funcName)
    {
        return funcName switch
        {
            "Print" => BuiltinFunctionCode.Print,
            "PrintI" => BuiltinFunctionCode.PrintI,
            "PrintF" => BuiltinFunctionCode.PrintF,
            "ItoS" => BuiltinFunctionCode.ItoS,
            "FtoS" => BuiltinFunctionCode.FtoS,
            "ItoF" => BuiltinFunctionCode.ItoF,
            "FtoI" => BuiltinFunctionCode.FtoI,
            "StoI" => BuiltinFunctionCode.StoI,
            "StoF" => BuiltinFunctionCode.StoF,
            "SConcat" => BuiltinFunctionCode.SConcat,
            "SubStr" => BuiltinFunctionCode.SubStr,
            "StrLen" => BuiltinFunctionCode.StrLen,
            "Input" => BuiltinFunctionCode.Input,
            _ => throw new ArgumentException($"Unknown builtin function: {funcName}")
        };
    }
}