namespace Lexems.UnitTests;

using System.Collections.Generic;

using Lexems;
using Xunit;

public class LexerTests
{
    [Theory]
    [MemberData(nameof(GetTokenizeIdentifiersAndKeywordsData))]
    [MemberData(nameof(GetTokenizeLiterals))]
    [MemberData(nameof(GetSkipWhitespacesAndCommentsData))]
    [MemberData(nameof(GetTokenizeExpressions))]
    public void Can_tokenize_lexemes(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, List<Token>> GetTokenizeConstructions()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "fn add(a: int, b: int): int { return a + b; }",
                [
                    new(TokenType.Fn),
                    new(TokenType.Identifier, "add"),
                    new(TokenType.LeftParen),
                    new(TokenType.Identifier, "a"),
                    new(TokenType.Colon),
                    new(TokenType.Identifier),
                    new(TokenType.Comma),
                    new(TokenType.Identifier, "b"),
                    new(TokenType.Colon),
                    new(TokenType.Identifier, "int"),
                    new(TokenType.RightParen),
                    new(TokenType.Colon),
                    new(TokenType.Identifier, "int"),
                    new(TokenType.LeftBrace),
                    new(TokenType.Return),
                    new(TokenType.Identifier, "a"),
                    new(TokenType.Plus),
                    new(TokenType.Identifier, "b"),
                    new(TokenType.Semicolon),
                    new(TokenType.RightBrace)
                ]
            },
            {
                "if (x > 100) { return 100; } else { return x; }",
                [
                    new(TokenType.If),
                    new(TokenType.LeftParen),
                    new(TokenType.Identifier, "x"),
                    new(TokenType.Greater),
                    new(TokenType.Identifier, 100),
                    new(TokenType.RightParen),
                    new(TokenType.LeftBrace),
                    new(TokenType.Return),
                    new(TokenType.Identifier, 100),
                    new(TokenType.Semicolon),
                    new(TokenType.RightBrace),
                    new(TokenType.Else),
                    new(TokenType.LeftBrace),
                    new(TokenType.Return),
                    new(TokenType.Identifier, "x"),
                    new(TokenType.Semicolon),
                    new(TokenType.RightBrace)
                ]
            },
            {
                "for (let i = 1; i <= 5; i = i + 1) { printi(i); }",
                new List<Token>
                {
                    new(TokenType.For),
                    new(TokenType.LeftParen),
                    new(TokenType.Let),
                    new(TokenType.Identifier, "i"),
                    new(TokenType.Assign),
                    new(TokenType.IntegerLiteral, "1"),
                    new(TokenType.Semicolon),
                    new(TokenType.Identifier, "i"),
                    new(TokenType.LessEqual),
                    new(TokenType.IntegerLiteral, "5"),
                    new(TokenType.Semicolon),
                    new(TokenType.Identifier, "i"),
                    new(TokenType.Assign),
                    new(TokenType.Identifier, "i"),
                    new(TokenType.Plus),
                    new(TokenType.IntegerLiteral, "1"),
                    new(TokenType.RightParen),
                    new(TokenType.LeftBrace),
                    new(TokenType.Identifier, "printi"),
                    new(TokenType.LeftParen),
                    new(TokenType.Identifier, "i"),
                    new(TokenType.RightParen),
                    new(TokenType.Semicolon),
                    new(TokenType.RightBrace),
                }
            },
            {
                "while (n < 100) { if (n > 10) { break; } n = n * 2; }",
                new List<Token>
                {
                    new(TokenType.While),
                    new(TokenType.LeftParen),
                    new(TokenType.Identifier, "n"),
                    new(TokenType.Less),
                    new(TokenType.IntegerLiteral, "100"),
                    new(TokenType.RightParen),
                    new(TokenType.LeftBrace),
                    new(TokenType.If),
                    new(TokenType.LeftParen),
                    new(TokenType.Identifier, "n"),
                    new(TokenType.Greater),
                    new(TokenType.IntegerLiteral, "10"),
                    new(TokenType.RightParen),
                    new(TokenType.LeftBrace),
                    new(TokenType.Break),
                    new(TokenType.Semicolon),
                    new(TokenType.RightBrace),
                    new(TokenType.Identifier, "n"),
                    new(TokenType.Assign),
                    new(TokenType.Identifier, "n"),
                    new(TokenType.Star),
                    new(TokenType.IntegerLiteral, "2"),
                    new(TokenType.Semicolon),
                    new(TokenType.RightBrace),
                }
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeExpressions()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "x % y",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Percent),
                    new Token(TokenType.Identifier, "y")
                ]
            },
            {
                "x ** y",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.StarStar),
                    new Token(TokenType.Identifier, "y")
                ]
            },
            {
                "x++",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.PlusPlus),
                ]
            },
            {
                "x--",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.MinusMinus),
                ]
            },
            {
                "x + y / (10 - z * 2)",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Plus),
                    new Token(TokenType.Identifier, "y"),
                    new Token(TokenType.Slash),
                    new Token(TokenType.LeftParen),
                    new Token(TokenType.IntegerLiteral, 10),
                    new Token(TokenType.Minus),
                    new Token(TokenType.Identifier, "z"),
                    new Token(TokenType.Star),
                    new Token(TokenType.IntegerLiteral, 2),
                    new Token(TokenType.RightParen),
                ]
            },
            {
                "a < b || a > c || b == c",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Less),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.OrOr),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Greater),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.OrOr),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.EqualEqual),
                    new Token(TokenType.Identifier, "c"),
                ]
            },
            {
                "a <= b && b >= c && a != c",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.LessEqual),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.AndAnd),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.GreaterEqual),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.AndAnd),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.NotEqual),
                    new Token(TokenType.Identifier, "c"),
                ]
            },
            {
                "a < b | a > c | b == c",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Less),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Or),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Greater),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.Or),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.EqualEqual),
                    new Token(TokenType.Identifier, "c"),
                ]
            },
            {
                "a <= b & b >= c & a != c",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.LessEqual),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.And),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.GreaterEqual),
                    new Token(TokenType.Identifier, "c"),
                    new Token(TokenType.And),
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.NotEqual),
                    new Token(TokenType.Identifier, "c"),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetSkipWhitespacesAndCommentsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "x \t\r\ny",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Identifier, "y"),
                ]
            },
            {
                "let x = 0; // This is a comment",
                [
                    new Token(TokenType.Let),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntegerLiteral, 0),
                    new Token(TokenType.Semicolon)
                ]
            },
            {
                "/* comments */ a / /* should be */ b * c /* ignored */",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Slash),
                    new Token(TokenType.Identifier, "b"),
                    new Token(TokenType.Star),
                    new Token(TokenType.Identifier, "c"),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeLiterals()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "255 0xFF 0b11111111 3.14 0.5 .5",
                [
                    new(TokenType.IntegerLiteral, 255),
                    new(TokenType.IntegerLiteral, 255),
                    new(TokenType.IntegerLiteral, 255),
                    new(TokenType.FloatLiteral,   3.14m),
                    new(TokenType.FloatLiteral,   0.5m),
                    new(TokenType.FloatLiteral,   0.5m),
                ]
            },
            {
                "let x = 3.14; let s = \"text\";",
                [
                    new(TokenType.Let),
                    new(TokenType.Identifier, "x"),
                    new(TokenType.Assign),
                    new(TokenType.FloatLiteral, 3.14m),
                    new(TokenType.Semicolon),
                    new(TokenType.Let),
                    new(TokenType.Identifier, "s"),
                    new(TokenType.Assign),
                    new(TokenType.StringLiteral, "text"),
                    new(TokenType.Semicolon),
                ]
            },
            {
                """
                "" "0" "Hello, world!"
                """,
                [
                    new Token(TokenType.StringLiteral, ""),
                    new Token(TokenType.StringLiteral, "0"),
                    new Token(TokenType.StringLiteral, "Hello, world!"),
                ]
            },
            {
                "let hello = \"Hello world\"; ",
                [
                    new(TokenType.Let),
                    new(TokenType.Identifier, "hello"),
                    new(TokenType.Assign),
                    new(TokenType.StringLiteral, "Hello world"),
                    new(TokenType.Semicolon),
                ]
            },
            {
                """
                "\n\t\"\\"
                """,
                [
                    new Token(TokenType.StringLiteral, "\n\t\"\\"),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeIdentifiersAndKeywordsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "x y z123 _underscore",
                new List<Token>
                {
                    new(TokenType.Identifier, "x"),
                    new(TokenType.Identifier, "y"),
                    new(TokenType.Identifier, "z123"),
                    new(TokenType.Identifier, "_underscore"),
                }
            },
            {
                "fn let return if else while for break continue true false str bool int float unit",
                new List<Token>
                {
                    new(TokenType.Fn),
                    new(TokenType.Let),
                    new(TokenType.Return),
                    new(TokenType.If),
                    new(TokenType.Else),
                    new(TokenType.While),
                    new(TokenType.For),
                    new(TokenType.Break),
                    new(TokenType.Continue),
                    new(TokenType.True),
                    new(TokenType.False),
                    new(TokenType.Str),
                    new(TokenType.Bool),
                    new(TokenType.Int),
                    new(TokenType.Float),
                    new(TokenType.Unit),
                }
            },
        };
    }

    private static List<Token> Tokenize(string code)
    {
        List<Token> results = [];
        Lexer lexer = new(code);

        for (Token t = lexer.ParseToken(); t.Type != TokenType.Eof; t = lexer.ParseToken())
        {
            results.Add(t);
        }

        return results;
    }
}