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

    public EntryPointNode ParseProgram()
    {
        return ParseEntryPoint();
    }

    /// <summary>
    /// program = { function_declaration } ;
    /// </summary>
    private EntryPointNode ParseEntryPoint()
    {
        return new EntryPointNode(ParseFunctionDeclaration());
    }

    private void MatchBuiltinType(out string typeName, out ValueType typeValue)
    {
        switch (_tokens.Peek().Type)
        {
            case TokenType.Int:
                typeName = "int";
                typeValue = ValueType.Int;
                break;

            case TokenType.Float:
                typeName = "float";
                typeValue = ValueType.Float;
                break;

            case TokenType.Str:
                typeName = "str";
                typeValue = ValueType.Str;
                break;

            case TokenType.Unit:
                typeName = "unit";
                typeValue = ValueType.Unit;
                break;

            case TokenType.Bool:
                throw new NotImplementedException("'bool' type is not yet implemeneted");

            default:
                throw new UnexpectedLexemeException([TokenType.Int, TokenType.Float, TokenType.Str, TokenType.Unit], _tokens.Peek());
        }
    }

    /// <summary>
    /// function_declaration = "fn" , identifier , "(" , [ parameter_list ] , ")", ":" , type , block ;
    /// </summary>
    private FunctionDeclaration ParseFunctionDeclaration()
    {
        // Парсим как обычную функцию, проверку на main делегируем SemanticsChecker
        Match(TokenType.Fn);
        Token fnNameToken = Match(TokenType.Identifier);
        string fnName = fnNameToken.Value!.ToString();

        // Без параметров
        Match(TokenType.LeftParen);
        Match(TokenType.RightParen);

        string typeName = "unit";
        ValueType typeValue = ValueType.Unit;
        BlockStatement body;

        if (_tokens.Peek().Type == TokenType.LeftBrace)
        {
            body = ParseBlockStatement();
            return new FunctionDeclaration(fnName, typeName, body);
        }

        Match(TokenType.Colon);
        MatchBuiltinType(out typeName, out typeValue);

        _tokens.Advance();
        body = ParseBlockStatement();
        return new FunctionDeclaration(fnName, typeName, body);
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

    private VariableDeclaration ParseVariableDeclaration()
    {
        Match(TokenType.Let);
        Token identifier = Match(TokenType.Identifier);

        if (_tokens.Peek().Type == TokenType.Colon)
        {
            _tokens.Advance();

            string typeName;
            ValueType typeValue;

            MatchBuiltinType(out typeName, out typeValue);

            _tokens.Advance();
            Match(TokenType.Assign);

            return new VariableDeclaration(
                identifier.Value!.ToString(),
                new BuiltinType(typeName, typeValue),
                ParseExpression()
            );
        }

        Match(TokenType.Assign);
        return new VariableDeclaration(
            identifier.Value!.ToString(),
            null,
            ParseExpression()
        );
    }

    private AssignmentStatement ParseAssignmentStatement()
    {
        IdentifierExpression left = (IdentifierExpression)ParseExpression();
        Match(TokenType.Assign);
        Expression right = ParseExpression();

        return new AssignmentStatement(left, right);
    }

    /// <summary>
    /// statement = variable_declaration , ";"
    ///       | assign_statement , ";"
    ///       | function_call , ";"
    ///       | return_statement , ";"
    ///       | if_statement
    ///       | while_statement
    ///       | for_statement
    ///       | "break" , ";"
    ///       | "continue" , ";" ;
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

            case TokenType.Let:
                evaluated = ParseVariableDeclaration();
                Match(TokenType.Semicolon);
                break;

            default:
                evaluated = _tokens.Peek(1).Type == TokenType.Assign ? ParseAssignmentStatement() : ParseExpression();
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
    /// expression = logical_or; // начиная с 4-ей итерации
    /// expression = additive; // 2-ая итерация
    /// </summary>
    private Expression ParseExpression() => ParseAdditive();

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
        Expression left = ParseUnary();

        while (_tokens.Peek().Type == TokenType.Star ||
               _tokens.Peek().Type == TokenType.Slash ||
               _tokens.Peek().Type == TokenType.Percent)
        {
            Token op = _tokens.Advance();
            Expression right = ParseUnary();

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
    /// power = primary , [ "**" , power ] ;
    /// </summary>
    private Expression ParsePower()
    {
        Expression left = ParsePrimary();

        if (_tokens.Peek().Type == TokenType.StarStar)
        {
            _tokens.Advance();
            Expression right = ParsePower();
            left = new BinaryOperationExpression(left, BinaryOperation.Power, right);
        }

        return left;
    }

    /// <summary>
    /// unary = [ "+" | "-" | "!" ] , power ;.
    /// </summary>
    private Expression ParseUnary()
    {
        if (_tokens.Peek().Type == TokenType.Plus)
        {
            _tokens.Advance();
            return new UnaryOperationExpression(UnaryOperation.Plus, ParsePower());
        }
        else if (_tokens.Peek().Type == TokenType.Minus)
        {
            _tokens.Advance();
            return new UnaryOperationExpression(UnaryOperation.Minus, ParsePower());
        }
        else
        {
            return ParsePower();
        }
    }

    /// <summary>
    /// primary = integer_literal
    ///     | float_literal
    ///     | string_literal
    ///     | bool_literal
    ///     | identifier
    ///     | function_call
    ///     | "(" , expression , ")" ;
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
                return new LiteralExpression(ValueType.Str, new Value(token.Value!.ToString()));

            case TokenType.True:
            case TokenType.False:
                throw new NotImplementedException("'bool' type is not yet implemented");

            case TokenType.Identifier:

                string name = _tokens.Advance().Value!.ToString();
                if (_tokens.Peek().Type == TokenType.LeftParen)
                {
                    return ParseFunctionCall(name);
                }

                return new IdentifierExpression(name);

            case TokenType.LeftParen:

                _tokens.Advance();
                Expression expr = ParseExpression();
                Match(TokenType.RightParen);

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