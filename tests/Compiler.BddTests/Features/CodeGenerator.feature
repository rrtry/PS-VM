Feature: Code Generator - Instruction Generation
  As a compiler developer
  I want the code generator to produce correct VM instructions
  So that the virtual machine can execute programs correctly

  @codegen @positive @literal
  Scenario: Generate Push instruction for integer literal
    Given the source code "fn main(): int { return 42; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction list should contain Push with value 42

  @codegen @positive @literal
  Scenario: Generate Push instruction for float literal
    Given the source code "fn main(): int { return 3.14; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction list should contain Push with value 3.14

  @codegen @positive @literal
  Scenario: Generate Push instruction for string literal
    Given the source code 'fn main(): int { return "hello"; }'
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction list should contain Push with value "hello"

  @codegen @positive @arithmetic
  Scenario: Generate Add instruction for addition
    Given the source code "fn main(): int { return 1 + 2; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction sequence should be [Push(1), Push(2), Add, Halt]

  @codegen @positive @arithmetic
  Scenario: Generate Subtract instruction for subtraction
    Given the source code "fn main(): int { return 10 - 3; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction sequence should be [Push(10), Push(3), Subtract, Halt]

  @codegen @positive @arithmetic
  Scenario: Generate Multiply instruction for multiplication
    Given the source code "fn main(): int { return 4 * 5; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction sequence should be [Push(4), Push(5), Multiply, Halt]

  @codegen @positive @arithmetic
  Scenario: Generate Divide instruction for division
    Given the source code "fn main(): int { return 10 / 2; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction sequence should be [Push(10), Push(2), Divide, Halt]

  @codegen @positive @arithmetic
  Scenario: Generate Modulo instruction for modulo
    Given the source code "fn main(): int { return 10 % 3; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction sequence should be [Push(10), Push(3), Modulo, Halt]

  @codegen @positive @arithmetic
  Scenario: Generate Power instruction for exponentiation
    Given the source code "fn main(): int { return 2 ** 8; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction sequence should be [Push(2), Push(8), Power, Halt]

  @codegen @positive @unary
  Scenario: Generate Negate instruction for unary minus
    Given the source code "fn main(): int { return -5; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction sequence should be [Push(5), Negate, Halt]

  @codegen @positive @unary
  Scenario: Generate no instruction for unary plus
    Given the source code "fn main(): int { return +5; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction sequence should be [Push(5), Halt]

  @codegen @positive @variable
  Scenario: Generate LoadLocal for variable reference
    Given the source code "fn main(): int { let x = 5; return x; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction list should contain LoadLocal with operand "x"

  @codegen @positive @variable
  Scenario: Generate StoreLocal for variable declaration
    Given the source code "fn main(): int { let x = 42; return 0; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction list should contain StoreLocal with operand "x"

  @codegen @positive @variable
  Scenario: Generate StoreLocal for assignment
    Given the source code "fn main(): int { let x = 0; x = 42; return 0; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction list should contain exactly 2 StoreLocal instructions

  @codegen @positive @function
  Scenario: Generate CallBuiltin for printi
    Given the source code "fn main(): int { printi(42); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction list should contain CallBuiltin for PrintI

  @codegen @positive @function
  Scenario: Generate CallBuiltin for print
    Given the source code 'fn main(): int { print("hello"); return 0; }'
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction list should contain CallBuiltin for Print

  @codegen @positive @function
  Scenario: Generate CallBuiltin for printf
    Given the source code "fn main(): int { printf(3.14, 2); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction list should contain CallBuiltin for PrintF

  @codegen @positive @function
  Scenario: Generate arguments in correct order for function call
    Given the source code "fn main(): int { printf(3.14, 2); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then before CallBuiltin there should be Push(3.14) and Push(2) in order

  @codegen @positive @halt
  Scenario: Generate Halt for return statement
    Given the source code "fn main(): int { return 42; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the last instruction should be Halt

  @codegen @positive @complex
  Scenario: Generate instructions for complex expression
    Given the source code "fn main(): int { return (1 + 2) * 3; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction sequence should be [Push(1), Push(2), Add, Push(3), Multiply, Halt]

  @codegen @positive @complex
  Scenario: Generate instructions for nested expression
    Given the source code "fn main(): int { return 1 + 2 * 3; }"
    When the parser parses the program
    And semantic analysis runs
    And code generation runs
    Then the instruction sequence should be [Push(1), Push(2), Push(3), Multiply, Add, Halt]