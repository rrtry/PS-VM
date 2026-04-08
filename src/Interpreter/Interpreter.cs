namespace Interpreter;

using Ast;
using Parser;
using Semantics;

using VirtualMachine;
using VirtualMachine.Instructions;
using VirtualMachineCodegen;

public class Interpreter
{
    private readonly IEnvironment environment;

    public Interpreter(IEnvironment env)
    {
        environment = env;
    }

    public int ExitCode { get; set; }

    /// <summary>
    /// Выполнение программы.
    /// </summary>
    /// <param name="sourceCode">Исходный код программы.</param>
    public int Execute(string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            throw new ArgumentException("Source code cannot be null or empty", nameof(sourceCode));
        }

        Parser parser = new(sourceCode);
        EntryPointNode program = parser.ParseProgram();

        SemanticsChecker semanticsChecker = new(Builtins.Functions, Builtins.Types);
        semanticsChecker.Check(program);

        PsVmCodegen codegen = new();
        List<Instruction> instructions = codegen.GenerateCode(program);

        PsVm vm = new(environment, instructions);
        ExitCode = vm.RunProgram();

        return ExitCode;
    }
}