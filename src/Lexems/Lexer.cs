namespace Lexems;

public class Lexer
{
    public static readonly List<TokenType> Operators = new List<TokenType>
    {
        TokenType.Assign,        // =
        TokenType.Plus,          // +
        TokenType.Minus,         // -
        TokenType.Star,          // *
        TokenType.Slash,         // /
        TokenType.StarStar,      // **
        TokenType.Percent,       // %
        TokenType.PlusPlus,      // ++
        TokenType.MinusMinus,    // --
        TokenType.EqualEqual,    // ==
        TokenType.NotEqual,      // !=
        TokenType.Less,          // <
        TokenType.LessEqual,     // <=
        TokenType.Greater,       // >
        TokenType.GreaterEqual,  // >=
        TokenType.AndAnd,        // &&
        TokenType.OrOr,          // ||
        TokenType.Not,           // !
    };

    public static readonly List<TokenType> OtherLexems = new()
    {
        TokenType.LeftBrace,     // {
        TokenType.RightBrace,    // }
        TokenType.LeftParen,     // (
        TokenType.RightParen,    // )
        TokenType.LeftBracket,   // [
        TokenType.RightBracket,  // ]
        TokenType.Comma,         // ,
        TokenType.Semicolon,     // ;
        TokenType.Eof,           // End of file
        TokenType.Unknown,        // Unknown or invalid token
    };

    public static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
    {
        { "fn",       TokenType.Fn },
        { "let",      TokenType.Let },
        { "return",   TokenType.Return },
        { "if",       TokenType.If },
        { "else",     TokenType.Else },
        { "while",    TokenType.While },
        { "for",      TokenType.For },
        { "break",    TokenType.Break },
        { "continue", TokenType.Continue },
        { "true",     TokenType.True },
        { "false",    TokenType.False },
        { "str",      TokenType.Str },
        { "bool",     TokenType.Bool },
        { "int",      TokenType.Int },
        { "float",    TokenType.Float },
        { "unit",     TokenType.Unit },
    };

    private readonly TextScanner scanner;

    public Lexer(string source)
    {
        scanner = new TextScanner(source);
    }

    public Token Peek()
    {
        int pos = scanner.GetPosition();
        Token token = ParseToken();
        scanner.SetPosition(pos);
        return token;
    }

    public Token ParseToken()
    {
        SkipWhiteSpacesAndComments();

        if (scanner.IsEnd())
        {
            return new Token(TokenType.Eof);
        }

        // Разбор числовых литералов
        char c = scanner.Peek();
        int octal = 10;

        if (char.IsAsciiDigit(c))
        {
            if ((c - '0') == 0)
            {
                if (scanner.Peek(1) == 'b' || scanner.Peek(1) == 'B')
                {
                    octal = 2;
                }
                else if (scanner.Peek(1) == 'x' || scanner.Peek(1) == 'X')
                {
                    octal = 16;
                }

                if (octal != 10)
                {
                    scanner.Advance();
                    scanner.Advance();
                }
            }

            return ParseNumericLiteral(octal);
        }
        else if (c == '.')
        {
            return ParseNumericLiteral(octal);
        }

        if (c == '"')
        {
            return ParseStringLiteral();
        }

        switch (c)
        {
            case ':':
                scanner.Advance();
                return new Token(TokenType.Colon);
            case '{':
                scanner.Advance();
                return new Token(TokenType.LeftBrace);
            case '}':
                scanner.Advance();
                return new Token(TokenType.RightBrace);
            case '[':
                scanner.Advance();
                return new Token(TokenType.LeftBracket);
            case ']':
                scanner.Advance();
                return new Token(TokenType.RightBracket);
            case '|':
                scanner.Advance();
                if (scanner.Peek() == '|')
                {
                    scanner.Advance();
                    return new Token(TokenType.OrOr);
                }

                return new Token(TokenType.Or);
            case '&':
                scanner.Advance();
                if (scanner.Peek() == '&')
                {
                    scanner.Advance();
                    return new Token(TokenType.AndAnd);
                }

                return new Token(TokenType.And);
            case '!':
                scanner.Advance();
                if (scanner.Peek() == '=')
                {
                    scanner.Advance();
                    return new Token(TokenType.NotEqual);
                }

                return new Token(TokenType.Not);
            case '=':
                scanner.Advance();
                if (scanner.Peek() == '=')
                {
                    scanner.Advance();
                    return new Token(TokenType.EqualEqual);
                }

                return new Token(TokenType.Assign);
            case ';':
                scanner.Advance();
                return new Token(TokenType.Semicolon);
            case ',':
                scanner.Advance();
                return new Token(TokenType.Comma);
            case '+':
                scanner.Advance();
                if (scanner.Peek() == '+')
                {
                    scanner.Advance();
                    return new Token(TokenType.PlusPlus);
                }

                return new Token(TokenType.Plus);
            case '-':
                scanner.Advance();
                if (scanner.Peek() == '-')
                {
                    scanner.Advance();
                    return new Token(TokenType.MinusMinus);
                }

                return new Token(TokenType.Minus);
            case '*':
                scanner.Advance();
                if (scanner.Peek() == '*')
                {
                    scanner.Advance();
                    return new Token(TokenType.StarStar);
                }

                return new Token(TokenType.Star);
            case '/':
                scanner.Advance();
                return new Token(TokenType.Slash);
            case '%':
                scanner.Advance();
                return new Token(TokenType.Percent);
            case '<':
                scanner.Advance();
                if (scanner.Peek() == '=')
                {
                    scanner.Advance();
                    return new Token(TokenType.LessEqual);
                }

                return new Token(TokenType.Less);
            case '>':
                scanner.Advance();
                if (scanner.Peek() == '=')
                {
                    scanner.Advance();
                    return new Token(TokenType.GreaterEqual);
                }

                return new Token(TokenType.Greater);
            case '(':
                scanner.Advance();
                return new Token(TokenType.LeftParen);
            case ')':
                scanner.Advance();
                return new Token(TokenType.RightParen);
        }

        return ParseIdentifierOrKeyword();
    }

    /// <summary>
    ///  Распознаёт идентификаторы и ключевые слова.
    ///  Идентификаторы обрабатываются по правилам:
    /// identifier = (letter | "_"), { letter | digit | "_" } ;
    /// keyword    = "fn" | "let" | "return" | "if" | "else" | "while" | "for"
    /// | "break" | "continue" | "int" | "float"
    /// | "str" | "unit" | "bool";
    /// </summary>
    private Token ParseIdentifierOrKeyword()
    {
        string value = scanner.Peek().ToString();
        scanner.Advance();

        for (char c = scanner.Peek(); char.IsLetter(c) || c == '_' || char.IsAsciiDigit(c); c = scanner.Peek())
        {
            value += c;
            scanner.Advance();
        }

        // Проверяем на совпадение с ключевым словом.
        if (Keywords.TryGetValue(value, out TokenType type))
        {
            return new Token(type);
        }

        // Возвращаем токен идентификатора.
        return new Token(TokenType.Identifier, new TokenValue(value));
    }

    /// <summary>
    ///  Распознаёт литерал числа по правилам:
    /// integer_literal     = decimal_literal | hexadecimal_literal | binary_literal ;
    /// decimal_literal     = digit , { digit } ;
    /// hexadecimal_literal = ("0x" | "0X") , hex_digit , { hex_digit } ;
    /// binary_literal      = ("0b" | "0B") , binary_digit , { binary_digit } ;
    /// float_literal       = digit , { digit } , "." , digit , { digit } ;
    /// </summary>
    private Token ParseNumericLiteral(int octal)
    {
        char lastChar = scanner.Peek();
        decimal value = 0m;

        if (IsDigitValue(lastChar, octal))
        {
            value = GetDigitValue(scanner.Peek(), octal);
            scanner.Advance();

            for (char c = scanner.Peek(); IsDigitValue(c, octal); c = scanner.Peek())
            {
                value = value * octal + GetDigitValue(c, octal);
                scanner.Advance();
            }
        }

        // Читаем дробную часть числа.
        if (scanner.Peek() == '.')
        {
            if (octal != 10)
            {
                return new Token(TokenType.Unknown, new TokenValue(value));
            }

            scanner.Advance();
            decimal factor = 0.1m;

            for (char c = scanner.Peek(); char.IsAsciiDigit(c); c = scanner.Peek())
            {
                scanner.Advance();
                value += factor * GetDigitValue(c, 10);
                factor *= 0.1m;
            }
        }
        else
        {
            return new Token(TokenType.IntegerLiteral, decimal.ToInt64(value));
        }

        return new Token(TokenType.FloatLiteral, value);

        // Локальная функция, проверяющая является ли символ числом
        static bool IsDigitValue(char ch, int octal)
        {
            switch (octal)
            {
                case 2:
                    return ch == '1' || ch == '0';
                case 10:
                    return ch >= '0' && ch <= '9';
                case 16:
                    return (ch >= '0' && ch <= '9') || (ch >= 'A' && ch <= 'F') || (ch >= 'a' && ch <= 'f');
                default:
                    throw new ArgumentException($"Invalid base: {octal}");
            }
        }

        // Локальная функция для получения числа из символа цифры.
        static int GetDigitValue(char ch, int octal)
        {
            if (octal != 2 && octal != 10 && octal != 16)
            {
                throw new ArgumentException($"Invalid base: {octal}");
            }

            if ((ch == '1' || ch == '0') && octal == 2)
            {
                return ch - '0';
            }

            if (ch >= '0' && ch <= '9' && (octal == 10 || octal == 16))
            {
                return ch - '0';
            }
            else if (ch >= 'A' && ch <= 'F' && octal == 16)
            {
                return ch - 'A' + 10;
            }
            else if (ch >= 'a' && ch <= 'f' && octal == 16)
            {
                return ch - 'a' + 10;
            }
            else
            {
                throw new ArgumentException($"Invalid digit {ch} for base: {octal}");
            }
        }
    }

    /// <summary>
    ///  Распознаёт литерал строки по правилам:
    /// string_literal  = '"' , { character | escape_sequence } , '"' ;
    /// character       = ? любой Unicode-символ, кроме " и \ ? ;
    /// escape_sequence = "\\" , ( "n" | "t" | "\\" | "\"" ) ;
    /// </summary>
    private Token ParseStringLiteral()
    {
        scanner.Advance();

        string contents = "";
        while (scanner.Peek() != '\"')
        {
            if (scanner.IsEnd())
            {
                // Ошибка: строка, не закрытая кавычкой.
                return new Token(TokenType.Unknown, new TokenValue(contents));
            }

            // Проверяем наличие escape-последовательности.
            if (TryParseStringLiteralEscapeSequence(out char unescaped))
            {
                contents += unescaped;
            }
            else
            {
                contents += scanner.Peek();
                scanner.Advance();
            }
        }

        scanner.Advance();

        return new Token(TokenType.StringLiteral, new TokenValue(contents));
    }

    /// <summary>
    ///  Распознаёт escape-последовательности по правилам:
    ///  escape_sequence = "\\" , ( "n" | "t" | "\\" | "\"" ) ;
    ///  Возвращает null при появлении неизвестных escape-последовательностей.
    /// </summary>
    private bool TryParseStringLiteralEscapeSequence(out char unescaped)
    {
        if (scanner.Peek() == '\\')
        {
            scanner.Advance();
            char next = scanner.Peek();

            switch (next)
            {
                case '\"':
                    scanner.Advance();
                    unescaped = '\"';
                    return true;

                case '\\':
                    scanner.Advance();
                    unescaped = '\\';
                    return true;

                case 'n':
                    scanner.Advance();
                    unescaped = '\n';
                    return true;

                case 't':
                    scanner.Advance();
                    unescaped = '\t';
                    return true;

                case 'r':
                    scanner.Advance();
                    unescaped = '\r';
                    return true;

                default:
                    unescaped = '\0';
                    return false;
            }
        }

        unescaped = '\0';
        return false;
    }

    /// <summary>
    ///  Пропускает пробельные символы и комментарии, пока не встретит что-либо иное.
    /// </summary>
    private void SkipWhiteSpacesAndComments()
    {
        do
        {
            SkipWhiteSpaces();
        }
        while (TryParseMultilineComment() || TryParseSingleLineComment());
    }

    /// <summary>
    ///  Пропускает пробельные символы, пока не встретит иной символ.
    /// </summary>
    private void SkipWhiteSpaces()
    {
        while (char.IsWhiteSpace(scanner.Peek()))
        {
            scanner.Advance();
        }
    }

    /// <summary>
    ///  Пропускает многострочный комментарий в виде `/* ...текст */`,
    ///  пока не встретит `*/`.
    /// </summary>
    private bool TryParseMultilineComment()
    {
        if (scanner.Peek() == '/' && scanner.Peek(1) == '*')
        {
            do
            {
                scanner.Advance();
            }
            while (!(scanner.Peek() == '*' && scanner.Peek(1) == '/'));

            scanner.Advance();
            scanner.Advance();
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Пропускает однострочный комментарий в виде `-- ...текст`,
    ///  пока не встретит конец строки (его оставляет).
    /// </summary>
    private bool TryParseSingleLineComment()
    {
        if (scanner.Peek() == '/' && scanner.Peek(1) == '/')
        {
            do
            {
                scanner.Advance();
            }
            while (scanner.Peek() != '\n' && scanner.Peek() != '\r' && !scanner.IsEnd());

            return true;
        }

        return false;
    }
}