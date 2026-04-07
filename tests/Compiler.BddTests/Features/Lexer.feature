Feature: Lexer - Tokenization
  As a compiler developer
  I want the lexer to correctly tokenize source code
  So that the parser receives valid tokens

  @lexer @positive
  Scenario: Recognize decimal integer literal
    Given the source code "42"
    When the lexer tokenizes the input
    Then the token list should contain IntegerLiteral with value 42

  @lexer @positive
  Scenario: Recognize hexadecimal integer literal
    Given the source code "0xFF"
    When the lexer tokenizes the input
    Then the token list should contain IntegerLiteral with value 255

  @lexer @positive
  Scenario: Recognize binary integer literal
    Given the source code "0b1010"
    When the lexer tokenizes the input
    Then the token list should contain IntegerLiteral with value 10

  @lexer @positive
  Scenario: Recognize float literal
    Given the source code "3.14"
    When the lexer tokenizes the input
    Then the token list should contain FloatLiteral with value 3.14

  @lexer @positive
  Scenario: Recognize empty string literal
    Given the source code '""'
    When the lexer tokenizes the input
    Then the token list should contain StringLiteral with value ""

  @lexer @positive
  Scenario: Recognize string with content
    Given the source code '"Hello, World!"'
    When the lexer tokenizes the input
    Then the token list should contain StringLiteral with value "Hello, World!"

  @lexer @positive
  Scenario: Recognize string with escape sequences
    Given the source code '"\\n\\t\\"\\\\"'
    When the lexer tokenizes the input
    Then the token list should contain StringLiteral with value "\n\t\"\\"

  @lexer @positive
  Scenario: Recognize all keywords
    Given the source code "fn let return if else while for break continue true false str bool int float unit"
    When the lexer tokenizes the input
    Then the token list should contain 15 tokens
    And the tokens should be [Fn, Let, Return, If, Else, While, For, Break, Continue, True, False, Str, Bool, Int, Float, Unit]

  @lexer @positive
  Scenario: Distinguish identifier from keyword
    Given the source code "fn fnMain"
    When the lexer tokenizes the input
    Then the token list should contain 2 tokens
    And token 0 should be Fn
    And token 1 should be Identifier with value "fnMain"

  @lexer @positive
  Scenario: Recognize identifiers
    Given the source code "x y z123 _underscore"
    When the lexer tokenizes the input
    Then the token list should contain 4 Identifier tokens

  @lexer @positive
  Scenario: Skip single-line comment
    Given the source code "let x = 5; // This is ignored"
    When the lexer tokenizes the input
    Then the token list should not contain any comment tokens
    And the last token should be Semicolon

  @lexer @positive
  Scenario: Skip multi-line comment
    Given the source code "/* comment */ a + b"
    When the lexer tokenizes the input
    Then the token list should not contain any comment tokens
    And the first identifier token should have value "a"

  @lexer @positive
  Scenario: Skip whitespace between tokens
    Given the source code "x \t\ny"
    When the lexer tokenizes the input
    Then the token list should contain exactly 2 tokens

  @lexer @positive
  Scenario: Recognize multi-character operators
    Given the source code "** == != <= >= && ||"
    When the lexer tokenizes the input
    Then the token list should contain [StarStar, EqualEqual, NotEqual, LessEqual, GreaterEqual, And, Or]

  @lexer @positive
  Scenario: Recognize single-character operators
    Given the source code "+ - * / % = < > !"
    When the lexer tokenizes the input
    Then the token list should contain [Plus, Minus, Star, Slash, Percent, Assign, Less, Greater, Not]

  @lexer @positive
  Scenario: Recognize punctuation
    Given the source code "( ) { } ; : ,"
    When the lexer tokenizes the input
    Then the token list should contain [LeftParen, RightParen, LeftBrace, RightBrace, Semicolon, Colon, Comma]

  @lexer @edge
  Scenario: Handle empty input
    Given the source code ""
    When the lexer tokenizes the input
    Then the token list should contain only EndOfFile

  @lexer @edge
  Scenario: Handle input with only whitespace
    Given the source code "   \t\n  "
    When the lexer tokenizes the input
    Then the token list should contain only EndOfFile

  @lexer @edge
  Scenario: Handle input with only comments
    Given the source code "// only comment\n/* multi-line */"
    When the lexer tokenizes the input
    Then the token list should contain only EndOfFile

  @lexer @negative
  Scenario: Reject invalid character
    Given the source code "let x = @;"
    When the lexer tokenizes the input
    Then a lexer error should occur

  @lexer @negative
  Scenario: Reject unclosed string
    Given the source code '"unclosed'
    When the lexer tokenizes the input
    Then a lexer error should occur