using Compiler.BddTests.Support;
using FluentAssertions;
using Lexems;
using TechTalk.SpecFlow;
using Xunit;

namespace Compiler.BddTests.StepDefinitions;

[Binding]
public class LexerSteps
{
    private readonly CompilerTestContext _context;

    public LexerSteps(CompilerTestContext context)
    {
        _context = context;
    }

    [Given(@"the source code (.*)")]
    public void GivenTheSourceCode(string sourceCode)
    {
        if (sourceCode.StartsWith('"') && sourceCode.EndsWith('"') && sourceCode.Length >= 2)
        {
            sourceCode = sourceCode[1..^1];
        }

        else if (sourceCode.StartsWith('\'') && sourceCode.EndsWith('\'') && sourceCode.Length >= 2)
        {
            sourceCode = sourceCode[1..^1];
        }

        _context.SourceCode = sourceCode;
    }

    [When(@"the lexer tokenizes the input")]
    public void WhenTheLexerTokenizesTheInput()
    {
        try
        {
            var lexer = new Lexer(_context.SourceCode);
            _context.Tokens.Clear();

            Token token;
            do
            {
                token = lexer.ParseToken();
                _context.Tokens.Add(token);
            } while (token.Type != TokenType.Eof);
        }
        catch (Exception ex)
        {
            _context.LexerError = ex;
        }
    }

    [Then(@"the token list should contain IntegerLiteral with value (.*)")]
    public void ThenTheTokenListShouldContainIntegerLiteralWithValue(int value)
    {
        _context.LexerError.Should().BeNull("lexer should not produce errors");
        _context.Tokens.Should().Contain(t => t.Type == TokenType.IntegerLiteral &&
                                             t.Value != null &&
                                             Convert.ToInt32(t.Value) == value);
    }

    [Then(@"the token list should contain FloatLiteral with value (.*)")]
    public void ThenTheTokenListShouldContainFloatLiteralWithValue(decimal value)
    {
        _context.LexerError.Should().BeNull("lexer should not produce errors");
        _context.Tokens.Should().Contain(t => t.Type == TokenType.FloatLiteral &&
                                             t.Value != null &&
                                             Convert.ToDecimal(t.Value) == value);
    }

    [Then(@"the token list should contain StringLiteral with value (.*)")]
    public void ThenTheTokenListShouldContainStringLiteralWithValue(string value)
    {
        _context.LexerError.Should().BeNull("lexer should not produce errors");
        _context.Tokens.Should().Contain(t => t.Type == TokenType.StringLiteral &&
                                             t.Value?.ToString() == value);
    }

    [Then(@"the token list should contain (.*) tokens")]
    public void ThenTheTokenListShouldContainTokens(int count)
    {
        _context.LexerError.Should().BeNull("lexer should not produce errors");
        _context.Tokens.Count(t => t.Type != TokenType.Eof).Should().Be(count);
    }

    [Then(@"the tokens should be \[(.*)\]")]
    public void ThenTheTokensShouldBe(string tokenTypes)
    {
        var expectedTypes = tokenTypes.Split(", ", StringSplitOptions.RemoveEmptyEntries)
            .Select(t => Enum.Parse<TokenType>(t.Trim()))
            .ToList();

        var actualTypes = _context.Tokens.Where(t => t.Type != TokenType.Eof).Select(t => t.Type).ToList();
        actualTypes.Should().Equal(expectedTypes);
    }

    [Then(@"token (.*) should be (.*)")]
    public void ThenTokenShouldBe(int index, TokenType tokenType)
    {
        _context.Tokens[index].Type.Should().Be(tokenType);
    }

    [Then(@"token (.*) should be Identifier with value (.*)")]
    public void ThenTokenShouldBeIdentifierWithValue(int index, string value)
    {
        _context.Tokens[index].Type.Should().Be(TokenType.Identifier);
        _context.Tokens[index].Value?.ToString().Should().Be(value);
    }

    [Then(@"the token list should contain (.*) Identifier tokens")]
    public void ThenTheTokenListShouldContainIdentifierTokens(int count)
    {
        _context.Tokens.Count(t => t.Type == TokenType.Identifier).Should().Be(count);
    }

    [Then(@"the token list should not contain any comment tokens")]
    public void ThenTheTokenListShouldNotContainAnyCommentTokens()
    {
        _context.Tokens.Should().NotContain(t => t.Type == TokenType.Comment);
    }

    [Then(@"the last token should be Semicolon")]
    public void ThenTheLastTokenShouldBeSemicolon()
    {
        var nonEofTokens = _context.Tokens.Where(t => t.Type != TokenType.Eof).ToList();
        nonEofTokens.Should().NotBeEmpty();
        nonEofTokens.Last().Type.Should().Be(TokenType.Semicolon);
    }

    [Then(@"the first identifier token should have value (.*)")]
    public void ThenTheFirstIdentifierTokenShouldHaveValue(string value)
    {
        var firstIdentifier = _context.Tokens.FirstOrDefault(t => t.Type == TokenType.Identifier);
        firstIdentifier.Should().NotBeNull();
        firstIdentifier!.Value?.ToString().Should().Be(value);
    }

    [Then(@"the token list should contain exactly (.*) tokens")]
    public void ThenTheTokenListShouldContainExactlyTokens(int count)
    {
        _context.Tokens.Count(t => t.Type != TokenType.Eof).Should().Be(count);
    }

    [Then(@"the token list should contain only EndOfFile")]
    public void ThenTheTokenListShouldContainOnlyEndOfFile()
    {
        _context.Tokens.Should().HaveCount(1);
        _context.Tokens[0].Type.Should().Be(TokenType.Eof);
    }

    [Then(@"the token list should contain \[(.*)\]")]
    public void ThenTheTokenListShouldContain(string tokenTypes)
    {
        var expectedTypes = tokenTypes.Split(", ", StringSplitOptions.RemoveEmptyEntries)
            .Select(t => Enum.Parse<TokenType>(t.Trim()))
            .ToList();

        var actualTypes = _context.Tokens.Where(t => t.Type != TokenType.Eof).Select(t => t.Type).ToList();
        actualTypes.Should().Equal(expectedTypes);
    }

    [Then(@"a lexer error should occur")]
    public void ThenALexerErrorShouldOccur()
    {
        _context.LexerError.Should().NotBeNull("lexer should produce an error for invalid input");
    }
}