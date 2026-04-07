using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;
using Compiler.BddTests.Support;
using FluentAssertions;
using Parser;
using TechTalk.SpecFlow;

namespace Compiler.BddTests.StepDefinitions;

[Binding]
public class ParserSteps
{
    private readonly CompilerTestContext _context;

    public ParserSteps(CompilerTestContext context)
    {
        _context = context;
    }

    [When(@"the parser parses the program")]
    public void WhenTheParserParsesTheProgram()
    {
        try
        {
            var parser = new Parser(_context.SourceCode);
            _context.AstRoot = parser.ParseProgram();
        }
        catch (Exception ex)
        {
            _context.ParserError = ex;
        }
    }

    [Then(@"the AST should be valid")]
    public void ThenTheAstShouldBeValid()
    {
        _context.ParserError.Should().BeNull("parser should not produce errors");
        _context.AstRoot.Should().NotBeNull();
    }

    [Then(@"the AST should contain a VariableDeclaration for (.*)")]
    public void ThenTheAstShouldContainAVariableDeclarationFor(string variableName)
    {
        var varDecl = FindNode<VariableDeclaration>(_context.AstRoot!,
            v => v.Name == variableName);
        varDecl.Should().NotBeNull($"variable '{variableName}' should be declared");
    }

    [Then(@"the variable (.*) should have type (.*)")]
    public void ThenTheVariableShouldHaveType(string variableName, string typeName)
    {
        var varDecl = FindNode<VariableDeclaration>(_context.AstRoot!,
            v => v.Name == variableName);
        varDecl.Should().NotBeNull();
        varDecl!.DeclaredType.Should().NotBeNull();
        varDecl.DeclaredType!.Name.Should().Be(typeName);
    }

    [Then(@"the variable (.*) should have no explicit type")]
    public void ThenTheVariableShouldHaveNoExplicitType(string variableName)
    {
        var varDecl = FindNode<VariableDeclaration>(_context.AstRoot!,
            v => v.Name == variableName);
        varDecl.Should().NotBeNull();
        varDecl!.DeclaredType.Should().BeNull();
    }

    [Then(@"the variable (.*) should have initializer value (.*)")]
    public void ThenTheVariableShouldHaveInitializerValue(string variableName, int value)
    {
        var varDecl = FindNode<VariableDeclaration>(_context.AstRoot!,
            v => v.Name == variableName);
        varDecl.Should().NotBeNull();
        varDecl!.Initializer.Should().BeOfType<LiteralExpression>();
        var literal = (LiteralExpression)varDecl.Initializer;
        Convert.ToInt64(literal.Value).Should().Be(value);
    }

    [Then(@"the AST should contain a BinaryOperationExpression with operation (.*)")]
    public void ThenTheAstShouldContainABinaryOperationExpressionWithOperation(string operation)
    {
        var op = Enum.Parse<BinaryOperation>(operation);
        var binaryExpr = FindNode<BinaryOperationExpression>(_context.AstRoot!,
            e => e.Operation == op);
        binaryExpr.Should().NotBeNull($"should find binary expression with operation {operation}");
    }

    [Then(@"the multiplication should be evaluated before addition")]
    public void ThenTheMultiplicationShouldBeEvaluatedBeforeAddition()
    {
        var addExpr = FindNode<BinaryOperationExpression>(_context.AstRoot!,
            e => e.Operation == BinaryOperation.Add);
        addExpr.Should().NotBeNull();
        addExpr!.Right.Should().BeOfType<BinaryOperationExpression>();
        var rightExpr = (BinaryOperationExpression)addExpr.Right;
        rightExpr.Operation.Should().Be(BinaryOperation.Multiply);
    }

    [Then(@"the addition should be evaluated before multiplication")]
    public void ThenTheAdditionShouldBeEvaluatedBeforeMultiplication()
    {
        var mulExpr = FindNode<BinaryOperationExpression>(_context.AstRoot!,
            e => e.Operation == BinaryOperation.Multiply);
        mulExpr.Should().NotBeNull();
        mulExpr!.Left.Should().BeOfType<BinaryOperationExpression>();
        var leftExpr = (BinaryOperationExpression)mulExpr.Left;
        leftExpr.Operation.Should().Be(BinaryOperation.Add);
    }

    [Then(@"the power operation should be right associative")]
    public void ThenThePowerOperationShouldBeRightAssociative()
    {
        var outerPower = FindNode<BinaryOperationExpression>(_context.AstRoot!,
            e => e.Operation == BinaryOperation.Power && e.Right is BinaryOperationExpression);
        outerPower.Should().NotBeNull();
        outerPower!.Right.Should().BeOfType<BinaryOperationExpression>();
        var innerPower = (BinaryOperationExpression)outerPower.Right;
        innerPower.Operation.Should().Be(BinaryOperation.Power);
    }

    [Then(@"the subtraction should be left associative")]
    public void ThenTheSubtractionShouldBeLeftAssociative()
    {
        var outerSub = FindNode<BinaryOperationExpression>(_context.AstRoot!,
            e => e.Operation == BinaryOperation.Subtract && e.Left is BinaryOperationExpression);
        outerSub.Should().NotBeNull();
        outerSub!.Left.Should().BeOfType<BinaryOperationExpression>();
        var innerSub = (BinaryOperationExpression)outerSub.Left;
        innerSub.Operation.Should().Be(BinaryOperation.Subtract);
    }

    [Then(@"the AST should contain a UnaryOperationExpression with operation (.*)")]
    public void ThenTheAstShouldContainAUnaryOperationExpressionWithOperation(string operation)
    {
        var op = Enum.Parse<UnaryOperation>(operation);
        var unaryExpr = FindNode<UnaryOperationExpression>(_context.AstRoot!,
            e => e.Operation == op);
        unaryExpr.Should().NotBeNull($"should find unary expression with operation {operation}");
    }

    [Then(@"the AST should contain nested UnaryOperationExpressions")]
    public void ThenTheAstShouldContainNestedUnaryOperationExpressions()
    {
        var outerUnary = FindNode<UnaryOperationExpression>(_context.AstRoot!,
            e => e.Operand is UnaryOperationExpression);
        outerUnary.Should().NotBeNull();
    }

    [Then(@"the AST should contain a FunctionCallExpression for (.*)")]
    public void ThenTheAstShouldContainAFunctionCallExpressionFor(string functionName)
    {
        var callExpr = FindNode<FunctionCallExpression>(_context.AstRoot!,
            e => e.Function is NativeFunction nf && nf.Name == functionName);
        callExpr.Should().NotBeNull($"should find function call for '{functionName}'");
    }

    [Then(@"the function call should have (.*) argument[s]?")]
    public void ThenTheFunctionCallShouldHaveArguments(int count)
    {
        var callExpr = FindFirstNode<FunctionCallExpression>(_context.AstRoot!);
        callExpr.Should().NotBeNull();
        callExpr!.Arguments.Should().HaveCount(count);
    }

    [Then(@"the AST should contain a ReturnStatement with value")]
    public void ThenTheAstShouldContainAReturnStatementWithValue()
    {
        var returnStmt = FindFirstNode<ReturnStatement>(_context.AstRoot!);
        returnStmt.Should().NotBeNull();
        returnStmt!.ReturnValue.Should().NotBeNull();
    }

    [Then(@"the AST should contain a ReturnStatement without value")]
    public void ThenTheAstShouldContainAReturnStatementWithoutValue()
    {
        var returnStmt = FindFirstNode<ReturnStatement>(_context.AstRoot!);
        returnStmt.Should().NotBeNull();
        returnStmt!.ReturnValue.Should().BeNull();
    }

    [Then(@"the AST should contain an AssignmentStatement for (.*)")]
    public void ThenTheAstShouldContainAnAssignmentStatementFor(string variableName)
    {
        var assignment = FindNode<AssignmentStatement>(_context.AstRoot!,
            a => a.Left is IdentifierExpression id && id.Name == variableName);
        assignment.Should().NotBeNull($"should find assignment for '{variableName}'");
    }

    [Then(@"the block should contain (.*) statements")]
    public void ThenTheBlockShouldContainStatements(int count)
    {
        var block = FindFirstNode<BlockStatement>(_context.AstRoot!);
        block.Should().NotBeNull();
        block!.Statements.Should().HaveCount(count);
    }

    [Then(@"the function name should be (.*)")]
    public void ThenTheFunctionNameShouldBe(string functionName)
    {
        _context.AstRoot!.Main.Name.Should().Be(functionName);
    }

    [Then(@"the function return type should be (.*)")]
    public void ThenTheFunctionReturnTypeShouldBe(string typeName)
    {
        _context.AstRoot!.Main.ReturnType.Should().Be(typeName);
    }

    [Then(@"a parser error should occur")]
    public void ThenAParserErrorShouldOccur()
    {
        _context.ParserError.Should().NotBeNull("parser should produce an error for invalid input");
    }

    private static T? FindNode<T>(AstNode root, Func<T, bool> predicate) where T : AstNode
    {
        if (root is T t && predicate(t))
            return t;

        foreach (var child in GetChildren(root))
        {
            var result = FindNode(child, predicate);
            if (result != null)
                return result;
        }

        return null;
    }

    private static T? FindFirstNode<T>(AstNode root) where T : AstNode
    {
        if (root is T t)
            return t;

        foreach (var child in GetChildren(root))
        {
            var result = FindFirstNode<T>(child);
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
            FunctionDeclaration fd => [fd.Body],
            BlockStatement bs => bs.Statements,
            VariableDeclaration vd => [vd.Initializer],
            AssignmentStatement a => [a.Left, a.Right],
            ReturnStatement r when r.ReturnValue != null => [r.ReturnValue],
            BinaryOperationExpression b => [b.Left, b.Right],
            UnaryOperationExpression u => [u.Operand],
            FunctionCallExpression f => f.Arguments.Cast<AstNode>(),
            _ => []
        };
    }
}