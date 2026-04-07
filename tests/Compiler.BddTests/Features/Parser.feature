Feature: Parser - AST Generation
  As a compiler developer
  I want the parser to correctly build the AST from tokens
  So that semantic analysis receives a valid AST

  @parser @positive @variable
  Scenario: Parse variable declaration with explicit type
    Given the source code "fn main(): int { let x: int = 42; return 0; }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain a VariableDeclaration for "x"
    And the variable "x" should have type "int"
    And the variable "x" should have initializer value 42

  @parser @positive @variable
  Scenario: Parse variable declaration with type inference
    Given the source code "fn main(): int { let x = 42; return 0; }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain a VariableDeclaration for "x"
    And the variable "x" should have no explicit type

  @parser @positive @variable
  Scenario: Parse variable declaration with float type
    Given the source code "fn main(): int { let pi: float = 3.14; return 0; }"
    When the parser parses the program
    Then the AST should be valid
    And the variable "pi" should have type "float"

  @parser @positive @variable
  Scenario: Parse variable declaration with string type
    Given the source code 'fn main(): int { let s: str = "hello"; return 0; }'
    When the parser parses the program
    Then the AST should be valid
    And the variable "s" should have type "str"

  @parser @positive @expression
  Scenario: Parse binary addition expression
    Given the source code "fn main(): int { return 1 + 2; }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain a BinaryOperationExpression with operation Add

  @parser @positive @expression
  Scenario: Parse binary subtraction expression
    Given the source code "fn main(): int { return 10 - 3; }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain a BinaryOperationExpression with operation Subtract

  @parser @positive @expression
  Scenario: Parse binary multiplication expression
    Given the source code "fn main(): int { return 5 * 4; }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain a BinaryOperationExpression with operation Multiply

  @parser @positive @expression
  Scenario: Parse binary division expression
    Given the source code "fn main(): int { return 20 / 4; }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain a BinaryOperationExpression with operation Divide

  @parser @positive @expression
  Scenario: Parse modulo expression
    Given the source code "fn main(): int { return 10 % 3; }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain a BinaryOperationExpression with operation Modulo

  @parser @positive @expression
  Scenario: Parse power expression
    Given the source code "fn main(): int { return 2 ** 10; }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain a BinaryOperationExpression with operation Power

  @parser @positive @expression
  Scenario: Parse expression with multiplication precedence
    Given the source code "fn main(): int { return 1 + 2 * 3; }"
    When the parser parses the program
    Then the AST should be valid
    And the multiplication should be evaluated before addition

  @parser @positive @expression
  Scenario: Parse expression with parentheses
    Given the source code "fn main(): int { return (1 + 2) * 3; }"
    When the parser parses the program
    Then the AST should be valid
    And the addition should be evaluated before multiplication

  @parser @positive @expression
  Scenario: Parse power expression with right associativity
    Given the source code "fn main(): int { return 2 ** 3 ** 2; }"
    When the parser parses the program
    Then the AST should be valid
    And the power operation should be right associative

  @parser @positive @expression
  Scenario: Parse subtraction with left associativity
    Given the source code "fn main(): int { return 10 - 3 - 2; }"
    When the parser parses the program
    Then the AST should be valid
    And the subtraction should be left associative

  @parser @positive @expression
  Scenario: Parse unary minus expression
    Given the source code "fn main(): int { return -5; }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain a UnaryOperationExpression with operation Minus

  @parser @positive @expression
  Scenario: Parse unary plus expression
    Given the source code "fn main(): int { return +5; }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain a UnaryOperationExpression with operation Plus

  @parser @positive @expression
  Scenario: Parse double negation
    Given the source code "fn main(): int { return -(-5); }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain nested UnaryOperationExpressions

  @parser @positive @function
  Scenario: Parse function call with single argument
    Given the source code "fn main(): int { return printi(42); }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain a FunctionCallExpression for "printi"
    And the function call should have 1 argument

  @parser @positive @function
  Scenario: Parse function call with multiple arguments
    Given the source code "fn main(): int { return printf(3.14, 2); }"
    When the parser parses the program
    Then the AST should be valid
    And the function call should have 2 arguments

  @parser @positive @statement
  Scenario: Parse return statement with value
    Given the source code "fn main(): int { return 42; }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain a ReturnStatement with value

  @parser @positive @statement
  Scenario: Parse return statement without value
    Given the source code "fn main(): unit { return; }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain a ReturnStatement without value

  @parser @positive @statement
  Scenario: Parse assignment statement
    Given the source code "fn main(): int { let x = 0; x = 42; return 0; }"
    When the parser parses the program
    Then the AST should be valid
    And the AST should contain an AssignmentStatement for "x"

  @parser @positive @statement
  Scenario: Parse block with multiple statements
    Given the source code "fn main(): int { let a = 1; let b = 2; return a; }"
    When the parser parses the program
    Then the AST should be valid
    And the block should contain 3 statements

  @parser @positive @function
  Scenario: Parse main function with int return type
    Given the source code "fn main(): int { return 0; }"
    When the parser parses the program
    Then the AST should be valid
    And the function name should be "main"
    And the function return type should be "int"

  @parser @positive @function
  Scenario: Parse function with unit return type
    Given the source code 'fn main() { print("hello"); }'
    When the parser parses the program
    Then the AST should be valid
    And the function return type should be "unit"

  @parser @negative
  Scenario: Reject variable without initializer
    Given the source code "fn main(): int { let x: int; return 0; }"
    When the parser parses the program
    Then a parser error should occur

  @parser @negative
  Scenario: Reject double plus operator
    Given the source code "fn main(): int { return 1++; }"
    When the parser parses the program
    Then a parser error should occur

  @parser @negative
  Scenario: Reject double minus operator
    Given the source code "fn main(): int { return 1--; }"
    When the parser parses the program
    Then a parser error should occur

  @parser @negative
  Scenario: Reject chained unary operators
    Given the source code "fn main(): int { return -+1; }"
    When the parser parses the program
    Then a parser error should occur

  @parser @negative
  Scenario: Reject unmatched parenthesis
    Given the source code "fn main(): int { return ((5 + 5) * 2; }"
    When the parser parses the program
    Then a parser error should occur

  @parser @negative
  Scenario: Reject missing semicolon
    Given the source code "fn main(): int { let x = 5 return 0; }"
    When the parser parses the program
    Then a parser error should occur