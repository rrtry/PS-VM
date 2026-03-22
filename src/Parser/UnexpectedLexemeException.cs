namespace Parser;

using Lexems;

#pragma warning disable RCS1194
public class UnexpectedLexemeException : Exception
{
    public UnexpectedLexemeException(Token actual)
        : base($"Unexpected lexeme {actual}")
    {
    }

    public UnexpectedLexemeException(TokenType expected, Token actual)
        : base($"Unexpected lexeme {actual} where expected {expected}")
    {
    }

    public UnexpectedLexemeException(string expected, Token actual)
        : base($"Unexpected identifier {actual} where expected {expected}")
    {
    }

    public UnexpectedLexemeException(List<TokenType> expected, Token actual)
        : base($"Unexpected lexeme {actual} where expected {expected}")
    {
    }
}
#pragma warning restore RCS1194