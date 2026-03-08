namespace Parser;

using Lexems;

/// <summary>
/// Представляет поток токенов с двумя операциями:
///  - Peek() возвращает текущий токен
///  - Advance() переходит к следующему токену.
/// </summary>
public class TokenStream
{
    private readonly Lexer lexer;
    private Token nextToken;

    public TokenStream(string source)
    {
        lexer = new Lexer(source);
        nextToken = lexer.ParseToken();
    }

    public Token Peek(int c)
    {
        return lexer.Peek(c);
    }

    public Token Peek()
    {
        return nextToken;
    }

    public Token Advance()
    {
        Token previousToken = Peek();
        nextToken = lexer.ParseToken();
        return previousToken;
    }
}