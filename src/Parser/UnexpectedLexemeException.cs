namespace Parser;

using Lexems;

#pragma warning disable RCS1194
public class UnexpectedLexemeException : Exception
{
    public UnexpectedLexemeException(TokenType expected, Token actual)
        : base($"Unexpected lexeme {actual} where expected {expected}")
    {
    }

    public UnexpectedLexemeException(List<TokenType> expected, Token actual)
        : base($"Unexpected lexeme {actual} where expected {expected}")
    {
    }
}
#pragma warning restore RCS1194