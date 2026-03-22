namespace Parser;

using Ast;
using Ast.Declarations;
using Ast.Expressions;
using Ast.Statements;

using Lexems;
using Runtime;

public class Parser
{
    private readonly TokenStream _tokens;

    public Parser(string source)
    {
        _tokens = new TokenStream(source);
    }

    public EntryPointNode Parse()
    {
        return ParseEntryPoint();
    }

    /// <summary>
    /// return_statement = "return" , [ expression ] ; (* expression обязателен, если тип функции != unit *)
    /// </summary>
    private ReturnStatement ParseReturnStatement()
    {
        _tokens.Advance();
        if (_tokens.Peek().Type == TokenType.Semicolon)
        {
            return new ReturnStatement();
        }

        Expression returnExpression = ParseExpression();
        return new ReturnStatement(returnExpression);
    }

    /// <summary>
    /// main_function = "fn" , "main" , "(" , "): int" , block ;.
    /// </summary>
    private EntryPointNode ParseEntryPoint()
    {
        Match(TokenType.Fn);
        Token fnNameToken = Match(TokenType.Identifier);
        string fnName = fnNameToken.Value!.ToString();

        if (fnName != "main")
        {
            throw new UnexpectedLexemeException("main", fnNameToken);
        }

        Match(TokenType.LeftParen);
        Match(TokenType.RightParen);
        Match(TokenType.Colon);
        Match(TokenType.Int);

        BlockStatement body = ParseBlockStatement();
        FunctionDeclaration main = new FunctionDeclaration(fnName, [], "int", body);
        return new EntryPointNode(main);
    }

    /// <summary>
    /// statement = variable_declaration , ";"
    /// | assignment , ";"
    /// | function_call , ";"
    /// | return_statement , ";"
    /// | if_statement
    /// | while_statement
    /// | for_statement
    /// | "break" , ";"
    /// | "continue" , ";" ;
    /// </summary>
    private AstNode ParseStatement()
    {
        Token token = _tokens.Peek();
        AstNode evaluated;

        switch (token.Type)
        {
            case TokenType.Return:
                evaluated = ParseReturnStatement();
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

        while (_tokens.Peek().Type != TokenType.RightBrace &&
               _tokens.Peek().Type != TokenType.Eof)
        {
            AstNode stmt = ParseStatement();
            statements.Add(stmt);
        }

        Match(TokenType.RightBrace);
        return new BlockStatement(statements);
    }

    /// <summary>
    /// expression = assignment; // 3-ая итерация
    /// expression = logical_or
    /// </summary>
    private Expression ParseExpression() => ParseLogicalOr();

    /// <summary>
    /// logical_or = logical_and , { "||" , logical_and } ;.
    /// </summary>
    private Expression ParseLogicalOr()
    {
        Expression left = ParseLogicalAnd();
        while (_tokens.Peek().Type == TokenType.Or)
        {
            _tokens.Advance();
            Expression right = ParseLogicalAnd();
            left = new BinaryOperationExpression(left, BinaryOperation.Or, right);
        }

        return left;
    }

    /// <summary>
    /// logical_and = equality , { "&&" , equality } ;
    /// </summary>
    private Expression ParseLogicalAnd()
    {
        Expression left = ParseEquality();
        while (_tokens.Peek().Type == TokenType.And)
        {
            _tokens.Advance();
            Expression right = ParseEquality();
            left = new BinaryOperationExpression(left, BinaryOperation.And, right);
        }

        return left;
    }

    /// <summary>
    /// equality = relational , { ( "==" | "!=" ) , relational } ;
    /// </summary>
    private Expression ParseEquality()
    {
        Expression left = ParseRelational();
        if (_tokens.Peek().Type == TokenType.EqualEqual ||
            _tokens.Peek().Type == TokenType.NotEqual)
        {
            Token op = _tokens.Advance();
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
    /// relational = additive , { ( "<" | ">" | "<=" | ">=" ) , additive } ;
    /// </summary>
    private Expression ParseRelational()
    {
        Expression left = ParseAdditive();
        if (_tokens.Peek().Type == TokenType.Less ||
            _tokens.Peek().Type == TokenType.Greater ||
            _tokens.Peek().Type == TokenType.LessEqual ||
            _tokens.Peek().Type == TokenType.GreaterEqual)
        {
            Token op = _tokens.Advance();
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
    /// additive = multiplicative , { ( "+" | "-" ) , multiplicative } ;
    /// </summary>
    private Expression ParseAdditive()
    {
        Expression left = ParseMultiplicative();

        while (_tokens.Peek().Type == TokenType.Plus ||
               _tokens.Peek().Type == TokenType.Minus)
        {
            Token op = _tokens.Advance();
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
    /// multiplicative = unary , { ( "*" | "/" | "%" ) , unary } ;
    /// </summary>
    private Expression ParseMultiplicative()
    {
        Expression left = ParsePower();

        while (_tokens.Peek().Type == TokenType.Star ||
               _tokens.Peek().Type == TokenType.Slash ||
               _tokens.Peek().Type == TokenType.Percent)
        {
            Token op = _tokens.Advance();
            Expression right = ParsePower();

            left = op.Type switch
            {
                TokenType.Star => new BinaryOperationExpression(left, BinaryOperation.Multiply, right),
                TokenType.Slash => new BinaryOperationExpression(left, BinaryOperation.Divide, right),
                TokenType.Percent => new BinaryOperationExpression(left, BinaryOperation.Modulo, right),
                _ => throw new UnexpectedLexemeException(
                    [TokenType.Star, TokenType.Slash, TokenType.Percent],
                    op
                ),
            };
        }

        return left;
    }

    /// <summary>
    /// power = primary , [ "**" , power ] ;.
    /// </summary>
    private Expression ParsePower()
    {
        Expression left = ParseUnary();

        if (_tokens.Peek().Type == TokenType.StarStar)
        {
            _tokens.Advance();
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
        if (_tokens.Peek().Type == TokenType.Plus)
        {
            _tokens.Advance();
            return new UnaryOperationExpression(UnaryOperation.Plus, ParseUnary());
        }
        else if (_tokens.Peek().Type == TokenType.Minus)
        {
            _tokens.Advance();
            return new UnaryOperationExpression(UnaryOperation.Minus, ParseUnary());
        }
        else if (_tokens.Peek().Type == TokenType.Not)
        {
            _tokens.Advance();
            return new UnaryOperationExpression(UnaryOperation.Not, ParseUnary());
        }
        else
        {
            return ParsePrimary();
        }
    }

    /// <summary>
    /// primary = integer_literal
    /// | float_literal
    /// | string_literal
    /// | bool_literal
    /// | identifier
    /// | function_call
    /// | "(" , expression , ")" ;
    /// </summary>
    private Expression ParsePrimary()
    {
        Token token = _tokens.Peek();
        switch (token.Type)
        {
            case TokenType.IntegerLiteral:
                _tokens.Advance();
                return new LiteralExpression(ValueType.Int, new Value((long)token.Value!.ToDecimal()));

            case TokenType.FloatLiteral:
                _tokens.Advance();
                return new LiteralExpression(ValueType.Float, new Value((double)token.Value!.ToDecimal()));

            case TokenType.StringLiteral:
                _tokens.Advance();
                return new LiteralExpression(ValueType.String, new Value(token.Value!.ToString()));

            case TokenType.Identifier:

                string name = _tokens.Advance().Value!.ToString();
                if (_tokens.Peek().Type == TokenType.LeftParen)
                {
                    return ParseFunctionCall(name);
                }

                throw new NotImplementedException("Variable expression are not yet implemented");

            case TokenType.LeftParen:
            case TokenType.And:
            case TokenType.Or:

                _tokens.Advance();
                Expression expr = ParseExpression();

                if (token.Type == TokenType.LeftParen)
                {
                    Match(TokenType.RightParen);
                }

                return expr;

            default:
                throw new UnexpectedLexemeException(token);
        }
    }

    /// <summary>
    /// function_call = identifier , "(" , [ argument_list ] , ")" ;
    /// argument_list = expression , { "," , expression } ;
    /// </summary>
    private FunctionCallExpression ParseFunctionCall(string name)
    {
        Match(TokenType.LeftParen);
        List<Expression> args = new();

        if (_tokens.Peek().Type != TokenType.RightParen)
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
        if (_tokens.Peek().Type == type)
        {
            _tokens.Advance();
            return true;
        }

        return false;
    }

    private Token Match(TokenType expected)
    {
        Token t = _tokens.Peek();
        if (t.Type != expected)
        {
            throw new UnexpectedLexemeException(expected, t);
        }

        return _tokens.Advance();
    }
}