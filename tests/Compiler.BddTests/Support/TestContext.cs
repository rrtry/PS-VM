using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;
using Lexems;
using Parser;
using Runtime;
using Semantics;
using Tests.TestLibrary.TestDoubles;
using VirtualMachine;
using VirtualMachine.Instructions;
using VirtualMachineCodegen;

namespace Compiler.BddTests.Support;

/// <summary>
/// Shared context for BDD tests, holding state across steps.
/// </summary>
public class CompilerTestContext
{
    public string SourceCode { get; set; } = string.Empty;
    public string InputBuffer { get; set; } = string.Empty;

    public List<Token> Tokens { get; set; } = [];
    public Exception? LexerError { get; set; }

    public EntryPointNode? AstRoot { get; set; }
    public Exception? ParserError { get; set; }

    public Exception? SemanticError { get; set; }

    public List<Instruction> Instructions { get; set; } = [];

    public string OutputBuffer { get; set; } = string.Empty;
    public int ExitCode { get; set; }
    public Exception? RuntimeError { get; set; }

    public FakeEnvironment Environment { get; } = new();

    public void Reset()
    {
        SourceCode = string.Empty;
        InputBuffer = string.Empty;
        Tokens.Clear();
        LexerError = null;
        AstRoot = null;
        ParserError = null;
        SemanticError = null;
        Instructions.Clear();
        OutputBuffer = string.Empty;
        ExitCode = 0;
        RuntimeError = null;
    }
}