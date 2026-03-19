namespace Parser;

using Ast;
using Ast.Declarations;
using Ast.Expressions;

using Lexems;
using Runtime;

/// <summary>
/// Грамматика описана в файле `docs/specification/expressions-grammar.md`.
/// </summary>
public class Parser
{
    private readonly TokenStream tokens;

    public Parser(string source)
    {
        tokens = new TokenStream(source);
    }

    public BlockStatement Parse()
    {
        List<AstNode> nodes = new();
        while (tokens.Peek().Type != TokenType.Eof)
        {
            nodes.Add(ParseStatement());
        }

        return new BlockStatement(nodes);
    }

    /// <summary>
    /// statement =
    /// | variable_declaration , ";"
    /// | function_declaration, ";"
    /// | function_call , ";"
    /// | return_statement , ";"
    /// | if_statement
    /// | while_statement
    /// | for_statement
    /// | "break" , ";"
    /// | "continue" , ";".
    /// </summary>
    private AstNode ParseStatement()
    {
        Token token = tokens.Peek();
        AstNode evaluated;

        switch (token.Type)
        {
            case TokenType.Let:
                evaluated = ParseVariableDeclaration();
                Match(TokenType.Semicolon);
                break;

            default:
                evaluated = ParseExpression();
                Match(TokenType.Semicolon);
                break;
        }

        return evaluated;
    }

    /// <summary>
    /// block = "{" , { statement } , "}" ;.
    /// </summary>
    private BlockStatement ParseBlockStatement()
    {
        Match(TokenType.LeftBrace);
        List<AstNode> statements = [];

        while (tokens.Peek().Type != TokenType.RightBrace &&
               tokens.Peek().Type != TokenType.Eof)
        {
            AstNode stmt = ParseStatement();
            statements.Add(stmt);
        }

        Match(TokenType.RightBrace);
        return new BlockStatement(statements);
    }

    /// <summary>
    /// expression = assignment;.
    /// </summary>
    private Expression ParseExpression() => ParseAssignment();

    private Expression ParseAssignment()
    {
        Expression expr = ParseLogicalOr();
        while (tokens.Peek().Type == TokenType.Assign)
        {
            tokens.Advance();
            expr = new AssignmentExpression(expr, ParseLogicalOr());
        }

        return expr;
    }

    /// <summary>
    /// logical_or = logical_and , { "||" , logical_and } ;.
    /// </summary>
    private Expression ParseLogicalOr()
    {
        Expression left = ParseLogicalAnd();
        while (tokens.Peek().Type == TokenType.OrOr)
        {
            tokens.Advance();
            Expression right = ParseLogicalAnd();
            left = new BinaryOperationExpression(left, BinaryOperation.Or, right);
        }

        return left;
    }

    /// <summary>
    /// logical_and = equality, { "&&", equality } ;.
    /// </summary>
    private Expression ParseLogicalAnd()
    {
        Expression left = ParseEquality();
        while (tokens.Peek().Type == TokenType.AndAnd)
        {
            tokens.Advance();
            Expression right = ParseEquality();
            left = new BinaryOperationExpression(left, BinaryOperation.And, right);
        }

        return left;
    }

    /// <summary>
    /// equality = relational, { ("==" | "!="), relational } ;.
    /// </summary>
    private Expression ParseEquality()
    {
        Expression left = ParseRelational();
        if (tokens.Peek().Type == TokenType.EqualEqual ||
            tokens.Peek().Type == TokenType.NotEqual)
        {
            Token op = tokens.Advance();
            Expression right = ParseRelational();
            left = new BinaryOperationExpression(
                left,
                op.Type == TokenType.EqualEqual ? BinaryOperation.Equal : BinaryOperation.NotEqual,
                right
            );
        }

        return left;
    }

    /// <summary>
    /// (* Сравнение *)
    /// additive, { ("<" | ">" | "<=" | ">="), additive }.
    /// </summary>
    private Expression ParseRelational()
    {
        Expression left = ParseAdditive();
        if (tokens.Peek().Type == TokenType.Less ||
            tokens.Peek().Type == TokenType.Greater ||
            tokens.Peek().Type == TokenType.LessEqual ||
            tokens.Peek().Type == TokenType.GreaterEqual)
        {
            Token op = tokens.Advance();
            Expression right = ParseAdditive();
            switch (op.Type)
            {
                case TokenType.Less:
                    left = new BinaryOperationExpression(left, BinaryOperation.LessThan, right);
                    break;
                case TokenType.Greater:
                    left = new BinaryOperationExpression(left, BinaryOperation.GreaterThan, right);
                    break;
                case TokenType.LessEqual:
                    left = new BinaryOperationExpression(left, BinaryOperation.LessThanOrEqual, right);
                    break;
                case TokenType.GreaterEqual:
                    left = new BinaryOperationExpression(left, BinaryOperation.GreaterThanOrEqual, right);
                    break;
            }
        }

        return left;
    }

    /// <summary>
    /// variable_declaration =
    /// "let", identifier,[ ":", type ], "=", expression, ";".
    /// </summary>
    private VariableDeclaration ParseVariableDeclaration()
    {
        tokens.Advance();

        Token identifier = Match(TokenType.Identifier);
        string name = identifier.Value!.ToString();

        string? declaredType = null;
        if (tokens.Peek().Type == TokenType.Colon)
        {
            tokens.Advance();
            declaredType = tokens.Peek().Value?.ToString();
            tokens.Advance();
        }

        Match(TokenType.Assign);
        Expression value = ParseExpression();
        return new VariableDeclaration(name, declaredType, value);
    }

    /// <summary>
    /// additive = multiplicative, { ("+" | "-"), multiplicative } ;.
    /// </summary>
    private Expression ParseAdditive()
    {
        Expression left = ParseMultiplicative();

        while (tokens.Peek().Type == TokenType.Plus ||
               tokens.Peek().Type == TokenType.Minus)
        {
            Token op = tokens.Advance();
            Expression right = ParseMultiplicative();
            left = new BinaryOperationExpression(
                left,
                op.Type == TokenType.Plus ? BinaryOperation.Add : BinaryOperation.Subtract,
                right
            );
        }

        return left;
    }

    /// <summary>
    /// multiplicative  = power, { ("*" | "/" | "%"), power } ;.
    /// </summary>
    private Expression ParseMultiplicative()
    {
        Expression left = ParsePower();

        while (tokens.Peek().Type == TokenType.Star ||
               tokens.Peek().Type == TokenType.Slash ||
               tokens.Peek().Type == TokenType.Percent)
        {
            Token op = tokens.Advance();
            Expression right = ParsePower();

            left = op.Type switch
            {
                TokenType.Star => new BinaryOperationExpression(left, BinaryOperation.Multiply, right),
                TokenType.Slash => new BinaryOperationExpression(left, BinaryOperation.Divide, right),
                TokenType.Percent => new BinaryOperationExpression(left, BinaryOperation.Modulo, right),
                _ => throw new Exception("Invalid operator"),
            };
        }

        return left;
    }

    /// <summary>
    /// power = unary, [ ("^", "**"), power ] ;.
    /// </summary>
    private Expression ParsePower()
    {
        Expression left = ParseUnary();

        if (tokens.Peek().Type == TokenType.StarStar)
        {
            tokens.Advance();
            Expression right = ParsePower(); // правоассоциативно
            left = new BinaryOperationExpression(left, BinaryOperation.Power, right);
        }

        return left;
    }

    /// <summary>
    /// unary = [ ("+" | "-" | "!") ], primary ;.
    /// </summary>
    private Expression ParseUnary()
    {
        if (tokens.Peek().Type == TokenType.Plus)
        {
            tokens.Advance();
            return new UnaryOperationExpression(UnaryOperation.Plus, ParseUnary());
        }
        else if (tokens.Peek().Type == TokenType.Minus)
        {
            tokens.Advance();
            return new UnaryOperationExpression(UnaryOperation.Minus, ParseUnary());
        }
        else if (tokens.Peek().Type == TokenType.Not)
        {
            tokens.Advance();
            return new UnaryOperationExpression(UnaryOperation.Not, ParseUnary());
        }
        else
        {
            return ParsePrimary();
        }
    }

    /// <summary>
    /// primary = number_literal
    ///       | string_literal
    ///       | identifier
    ///       | function_call
    ///       | "(" , expression , ")" ;.
    /// </summary>
    private Expression ParsePrimary()
    {
        Token token = tokens.Peek();
        switch (token.Type)
        {
            case TokenType.IntegerLiteral:
                tokens.Advance();
                return new LiteralExpression(ValueType.Int, new Value((long)token.Value!.ToDecimal()));

            case TokenType.FloatLiteral:
                tokens.Advance();
                return new LiteralExpression(ValueType.Float, new Value((double)token.Value!.ToDecimal()));

            case TokenType.StringLiteral:
                tokens.Advance();
                return new LiteralExpression(ValueType.String, new Value(token.Value!.ToString()));

            case TokenType.Identifier:

                string name = tokens.Advance().Value!.ToString();
                if (tokens.Peek().Type == TokenType.LeftParen)
                {
                    return ParseFunctionCall(name);
                }

                return new VariableExpression(name);

            case TokenType.LeftParen:
            case TokenType.AndAnd:
            case TokenType.OrOr:

                tokens.Advance();
                Expression expr = ParseExpression();

                if (token.Type == TokenType.LeftParen)
                {
                    Match(TokenType.RightParen);
                }

                return expr;

            default:
                throw new Exception($"Unexpected token {token.Type}");
        }
    }

    /// <summary>
    /// function_call = identifier , "(" , [ argument_list ] , ")" ;
    /// argument_list = expression , { "," , expression } ;.
    /// </summary>
    private FunctionCallExpression ParseFunctionCall(string name)
    {
        Match(TokenType.LeftParen);
        List<Expression> args = new();

        if (tokens.Peek().Type != TokenType.RightParen)
        {
            do
            {
                args.Add(ParseExpression());
            }
            while (MatchOptional(TokenType.Comma));
        }

        Match(TokenType.RightParen);
        return new FunctionCallExpression(name, args);
    }

    private bool MatchOptional(TokenType type)
    {
        if (tokens.Peek().Type == type)
        {
            tokens.Advance();
            return true;
        }

        return false;
    }

    private Token Match(TokenType expected)
    {
        Token t = tokens.Peek();
        if (t.Type != expected)
        {
            throw new UnexpectedLexemeException(expected, t);
        }

        return tokens.Advance();
    }
}