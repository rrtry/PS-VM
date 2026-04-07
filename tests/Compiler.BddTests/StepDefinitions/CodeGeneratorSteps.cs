using Compiler.BddTests.Support;
using FluentAssertions;
using Runtime;
using TechTalk.SpecFlow;
using VirtualMachine.Instructions;
using VirtualMachineCodegen;

namespace Compiler.BddTests.StepDefinitions;

[Binding]
public class CodeGeneratorSteps
{
    private readonly CompilerTestContext _context;

    public CodeGeneratorSteps(CompilerTestContext context)
    {
        _context = context;
    }

    [When(@"code generation runs")]
    public void WhenCodeGenerationRuns()
    {
        if (_context.ParserError != null || _context.SemanticError != null || _context.AstRoot == null)
        {
            return;
        }

        var codegen = new PsVmCodegen();
        _context.Instructions = codegen.GenerateCode(_context.AstRoot);
    }

    [Then(@"the instruction list should contain Push with value (.*)")]
    public void ThenTheInstructionListShouldContainPushWithValue(object value)
    {
        _context.Instructions.Should().Contain(i =>
            i.Code == InstructionCode.Push &&
            ValuesEqual(i.Operand, value),
            $"should find Push instruction with value {value}");
    }

    [Then(@"the instruction list should contain Push with value (.*)")]
    public void ThenTheInstructionListShouldContainPushWithValue(double value)
    {
        _context.Instructions.Should().Contain(i =>
            i.Code == InstructionCode.Push &&
            i.Operand is Value v &&
            v.Type == ValueType.Float &&
            Math.Abs(v.AsFloat - value) < 0.0001,
            $"should find Push instruction with float value {value}");
    }

    [Then(@"the instruction list should contain Push with value (.*)")]
    public void ThenTheInstructionListShouldContainPushWithStringValue(string value)
    {
        _context.Instructions.Should().Contain(i =>
            i.Code == InstructionCode.Push &&
            i.Operand?.ToString() == value,
            $"should find Push instruction with string value '{value}'");
    }

    [Then(@"the instruction sequence should be \[(.*)\]")]
    public void ThenTheInstructionSequenceShouldBe(string instructionList)
    {
        var expectedInstructions = ParseInstructionSequence(instructionList);
        _context.Instructions.Should().HaveCount(expectedInstructions.Count);

        for (int i = 0; i < expectedInstructions.Count; i++)
        {
            var (code, operand) = expectedInstructions[i];
            _context.Instructions[i].Code.Should().Be(code);

            if (operand != null)
            {
                ValuesEqual(_context.Instructions[i].Operand, operand).Should().BeTrue(
                    $"instruction {i} operand should match");
            }
        }
    }

    [Then(@"the instruction list should contain LoadLocal with operand (.*)")]
    public void ThenTheInstructionListShouldContainLoadLocalWithOperand(string variableName)
    {
        _context.Instructions.Should().Contain(i =>
            i.Code == InstructionCode.LoadLocal &&
            i.Operand?.ToString() == variableName,
            $"should find LoadLocal instruction for variable '{variableName}'");
    }

    [Then(@"the instruction list should contain StoreLocal with operand (.*)")]
    public void ThenTheInstructionListShouldContainStoreLocalWithOperand(string variableName)
    {
        _context.Instructions.Should().Contain(i =>
            i.Code == InstructionCode.StoreLocal &&
            i.Operand?.ToString() == variableName,
            $"should find StoreLocal instruction for variable '{variableName}'");
    }

    [Then(@"the instruction list should contain exactly (.*) StoreLocal instructions")]
    public void ThenTheInstructionListShouldContainExactlyStoreLocalInstructions(int count)
    {
        _context.Instructions.Count(i => i.Code == InstructionCode.StoreLocal).Should().Be(count);
    }

    [Then(@"the instruction list should contain CallBuiltin for (.*)")]
    public void ThenTheInstructionListShouldContainCallBuiltinFor(string functionName)
    {
        var builtinCode = GetBuiltinFunctionCode(functionName);
        _context.Instructions.Should().Contain(i =>
            i.Code == InstructionCode.CallBuiltin &&
            i.Operand is int code &&
            code == (int)builtinCode,
            $"should find CallBuiltin instruction for {functionName}");
    }

    [Then(@"before CallBuiltin there should be Push\((.*)\) and Push\((.*)\) in order")]
    public void ThenBeforeCallBuiltinThereShouldBePushAndPushInOrder(double firstValue, int secondValue)
    {
        var callBuiltinIndex = _context.Instructions.FindIndex(i => i.Code == InstructionCode.CallBuiltin);
        callBuiltinIndex.Should().BeGreaterOrEqualTo(2);

        var secondPush = _context.Instructions[callBuiltinIndex - 1];
        secondPush.Code.Should().Be(InstructionCode.Push);
        ValuesEqual(secondPush.Operand, secondValue).Should().BeTrue();

        var firstPush = _context.Instructions[callBuiltinIndex - 2];
        firstPush.Code.Should().Be(InstructionCode.Push);
        ValuesEqual(firstPush.Operand, firstValue).Should().BeTrue();
    }

    [Then(@"the last instruction should be Halt")]
    public void ThenTheLastInstructionShouldBeHalt()
    {
        _context.Instructions.Should().NotBeEmpty();
        _context.Instructions[^1].Code.Should().Be(InstructionCode.Halt);
    }

    private static bool ValuesEqual(object? operand, object? expected)
    {
        if (operand == null && expected == null) return true;
        if (operand == null || expected == null) return false;

        if (operand is Value v)
        {
            return expected switch
            {
                int i => v.Type == ValueType.Int && v.AsInt == i,
                long l => v.Type == ValueType.Int && v.AsInt == l,
                double d => v.Type == ValueType.Float && Math.Abs(v.AsFloat - d) < 0.0001,
                string s => v.Type == ValueType.Str && v.AsString == s,
                _ => false
            };
        }

        return operand.Equals(expected) || operand.ToString() == expected.ToString();
    }

    private static List<(InstructionCode Code, object? Operand)> ParseInstructionSequence(string instructionList)
    {
        var result = new List<(InstructionCode, object?)>();
        var parts = instructionList.Split(", ", StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            var trimmed = part.Trim();

            if (trimmed.StartsWith("Push(") && trimmed.EndsWith(")"))
            {
                var valueStr = trimmed[5..^1];
                object? value = valueStr switch
                {
                    _ when int.TryParse(valueStr, out int i) => new Value(i),
                    _ when double.TryParse(valueStr, out double d) => new Value(d),
                    _ => new Value(valueStr.Trim('"'))
                };
                result.Add((InstructionCode.Push, value));
            }
            else if (trimmed.StartsWith("LoadLocal(") && trimmed.EndsWith(")"))
            {
                var name = trimmed[10..^1].Trim('"');
                result.Add((InstructionCode.LoadLocal, name));
            }
            else if (trimmed.StartsWith("StoreLocal(") && trimmed.EndsWith(")"))
            {
                var name = trimmed[11..^1].Trim('"');
                result.Add((InstructionCode.StoreLocal, name));
            }
            else if (trimmed.StartsWith("CallBuiltin(") && trimmed.EndsWith(")"))
            {
                var funcName = trimmed[12..^1];
                result.Add((InstructionCode.CallBuiltin, (int)GetBuiltinFunctionCode(funcName)));
            }
            else
            {
                var code = Enum.Parse<InstructionCode>(trimmed);
                result.Add((code, null));
            }
        }

        return result;
    }

    private static BuiltinFunctionCode GetBuiltinFunctionCode(string functionName)
    {
        return functionName switch
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
            _ => throw new ArgumentException($"Unknown builtin function: {functionName}")
        };
    }
}

public enum BuiltinFunctionCode
{
    Print = 0,
    PrintI = 1,
    PrintF = 2,
    ItoS = 3,
    FtoS = 4,
    ItoF = 5,
    FtoI = 6,
    StoI = 7,
    StoF = 8,
    SConcat = 9,
    SubStr = 10,
    StrLen = 11,
    Input = 12
}