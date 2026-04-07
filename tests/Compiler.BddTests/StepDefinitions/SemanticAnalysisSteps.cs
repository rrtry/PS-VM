using Ast;
using Compiler.BddTests.Support;
using FluentAssertions;
using Semantics;
using Semantics.Exceptions;
using TechTalk.SpecFlow;

namespace Compiler.BddTests.StepDefinitions;

[Binding]
public class SemanticAnalysisSteps
{
    private readonly CompilerTestContext _context;

    public SemanticAnalysisSteps(CompilerTestContext context)
    {
        _context = context;
    }

    [When(@"semantic analysis runs")]
    public void WhenSemanticAnalysisRuns()
    {
        if (_context.ParserError != null || _context.AstRoot == null)
        {
            return;
        }

        try
        {
            var checker = new SemanticsChecker(Ast.Builtins.Functions, Ast.Builtins.Types);
            checker.Check(_context.AstRoot);
        }
        catch (Exception ex)
        {
            _context.SemanticError = ex;
        }
    }

    [Then(@"no semantic errors should occur")]
    public void ThenNoSemanticErrorsShouldOccur()
    {
        _context.SemanticError.Should().BeNull("semantic analysis should not produce errors");
    }

    [Then(@"variable (.*) should have inferred type (.*)")]
    public void ThenVariableShouldHaveInferredType(string variableName, string typeName)
    {
        _context.SemanticError.Should().BeNull();

        var varDecl = FindVariableDeclaration(_context.AstRoot!, variableName);
        varDecl.Should().NotBeNull($"variable '{variableName}' should exist");

        varDecl!.TryGetAttribute<ResultTypeAttribute>(out var resultType).Should().BeTrue();
        resultType.Should().NotBeNull();
        resultType!.Type.Should().Be(Enum.Parse<Runtime.ValueType>(typeName));
    }

    [Then(@"a semantic error of type (.*) should occur")]
    public void ThenASemanticErrorOfTypeShouldOccur(string exceptionTypeName)
    {
        _context.SemanticError.Should().NotBeNull("semantic analysis should produce an error");

        var expectedType = exceptionTypeName switch
        {
            "UnknownSymbolException" => typeof(UnknownSymbolException),
            "DuplicateSymbolException" => typeof(DuplicateSymbolException),
            "TypeErrorException" => typeof(TypeErrorException),
            "InvalidFunctionCallException" => typeof(InvalidFunctionCallException),
            "InvalidAssignmentException" => typeof(InvalidAssignmentException),
            "InvalidDeclarationException" => typeof(InvalidDeclarationException),
            _ => throw new ArgumentException($"Unknown exception type: {exceptionTypeName}")
        };

        _context.SemanticError.Should().BeOfType(expectedType,
            $"error should be of type {exceptionTypeName}");
    }

    private static Ast.Declarations.VariableDeclaration? FindVariableDeclaration(AstNode root, string name)
    {
        if (root is Ast.Declarations.VariableDeclaration vd && vd.Name == name)
            return vd;

        foreach (var child in GetChildren(root))
        {
            var result = FindVariableDeclaration(child, name);
            if (result != null)
                return result;
        }

        return null;
    }

    private static IEnumerable<AstNode> GetChildren(AstNode node)
    {
        return node switch
        {
            EntryPointNode ep => [ep.Main],
            Ast.Declarations.FunctionDeclaration fd => [fd.Body],
            Ast.Statements.BlockStatement bs => bs.Statements,
            Ast.Declarations.VariableDeclaration vd => [vd.Initializer],
            Ast.Statements.AssignmentStatement a => [a.Left, a.Right],
            Ast.Statements.ReturnStatement r when r.ReturnValue != null => [r.ReturnValue],
            Ast.Expressions.BinaryOperationExpression b => [b.Left, b.Right],
            Ast.Expressions.UnaryOperationExpression u => [u.Operand],
            Ast.Expressions.FunctionCallExpression f => f.Arguments.Cast<AstNode>(),
            _ => []
        };
    }
}