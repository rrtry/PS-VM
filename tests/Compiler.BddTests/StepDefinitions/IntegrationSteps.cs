using Compiler.BddTests.Support;
using FluentAssertions;
using Interpreter;
using Semantics.Exceptions;
using TechTalk.SpecFlow;

namespace Compiler.BddTests.StepDefinitions;

[Binding]
public class IntegrationSteps
{
    private readonly CompilerTestContext _context;
    private Interpreter.Interpreter? _interpreter;

    public IntegrationSteps(CompilerTestContext context)
    {
        _context = context;
    }

    [Given(@"the input buffer contains (.*)")]
    public void GivenTheInputBufferContains(string input)
    {
        _context.Environment.AddInput(input);
    }

    [When(@"the interpreter executes the program")]
    public void WhenTheInterpreterExecutesTheProgram()
    {
        try
        {
            _interpreter = new Interpreter.Interpreter(_context.Environment);
            _context.ExitCode = _interpreter.Execute(_context.SourceCode);
            _context.OutputBuffer = _context.Environment.OutputBuffer;
        }
        catch (Exception ex)
        {
            _context.RuntimeError = ex;
        }
    }

    [Then(@"the output should be (.*)")]
    public void ThenTheOutputShouldBe(string expectedOutput)
    {
        _context.RuntimeError.Should().BeNull("execution should not fail");
        _context.OutputBuffer.Should().Be(expectedOutput);
    }

    [Then(@"the output should contain newline and tab")]
    public void ThenTheOutputShouldContainNewlineAndTab()
    {
        _context.RuntimeError.Should().BeNull("execution should not fail");
        _context.OutputBuffer.Should().Contain("\n");
        _context.OutputBuffer.Should().Contain("\t");
    }

    [Then(@"execution should fail with (.*)")]
    public void ThenExecutionShouldFailWith(string exceptionTypeName)
    {
        _context.RuntimeError.Should().NotBeNull("execution should fail");

        var expectedType = exceptionTypeName switch
        {
            "UnknownSymbolException" => typeof(UnknownSymbolException),
            "DuplicateSymbolException" => typeof(DuplicateSymbolException),
            "TypeErrorException" => typeof(TypeErrorException),
            "InvalidFunctionCallException" => typeof(InvalidFunctionCallException),
            "InvalidAssignmentException" => typeof(InvalidAssignmentException),
            "UnexpectedLexemeException" => typeof(Parser.UnexpectedLexemeException),
            _ => throw new ArgumentException($"Unknown exception type: {exceptionTypeName}")
        };

        _context.RuntimeError.Should().BeOfType(expectedType,
            $"error should be of type {exceptionTypeName}");
    }
}