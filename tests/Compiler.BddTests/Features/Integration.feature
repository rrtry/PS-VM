Feature: Integration - End-to-End Compilation
  As a compiler developer
  I want to test the complete compilation pipeline
  So that programs execute correctly from source to output

  @integration @positive @arithmetic
  Scenario: Evaluate simple integer addition
    Given the source code "fn main(): int { printi(1 + 2); return 0; }"
    When the interpreter executes the program
    Then the output should be "3"

  @integration @positive @arithmetic
  Scenario: Evaluate integer subtraction
    Given the source code "fn main(): int { printi(10 - 3); return 0; }"
    When the interpreter executes the program
    Then the output should be "7"

  @integration @positive @arithmetic
  Scenario: Evaluate integer multiplication
    Given the source code "fn main(): int { printi(4 * 5); return 0; }"
    When the interpreter executes the program
    Then the output should be "20"

  @integration @positive @arithmetic
  Scenario: Evaluate integer division
    Given the source code "fn main(): int { printi(10 / 2); return 0; }"
    When the interpreter executes the program
    Then the output should be "5"

  @integration @positive @arithmetic
  Scenario: Evaluate modulo operation
    Given the source code "fn main(): int { printi(10 % 3); return 0; }"
    When the interpreter executes the program
    Then the output should be "1"

  @integration @positive @arithmetic
  Scenario: Evaluate power operation
    Given the source code "fn main(): int { printi(2 ** 10); return 0; }"
    When the interpreter executes the program
    Then the output should be "1024"

  @integration @positive @arithmetic
  Scenario: Evaluate unary minus
    Given the source code "fn main(): int { printi(-5); return 0; }"
    When the interpreter executes the program
    Then the output should be "-5"

  @integration @positive @arithmetic
  Scenario: Evaluate unary minus on expression
    Given the source code "fn main(): int { printi(-(5 + 3)); return 0; }"
    When the interpreter executes the program
    Then the output should be "-8"

  @integration @positive @arithmetic
  Scenario: Evaluate power with right associativity
    Given the source code "fn main(): int { printi(2 ** 3 ** 2); return 0; }"
    When the interpreter executes the program
    Then the output should be "512"

  @integration @positive @arithmetic
  Scenario: Evaluate subtraction with left associativity
    Given the source code "fn main(): int { printi(10 - 3 - 2); return 0; }"
    When the interpreter executes the program
    Then the output should be "5"

  @integration @positive @arithmetic
  Scenario: Evaluate expression with parentheses
    Given the source code "fn main(): int { printi((1 + 2) * (8 / (3 - 1))); return 0; }"
    When the interpreter executes the program
    Then the output should be "12"

  @integration @positive @arithmetic
  Scenario: Evaluate complex expression with precedence
    Given the source code "fn main(): int { printi(1 + 2 * 8 / 3 - 1); return 0; }"
    When the interpreter executes the program
    Then the output should be "5"

  @integration @positive @unary
  Scenario: Evaluate unary plus
    Given the source code "fn main(): int { printi(+1); return 0; }"
    When the interpreter executes the program
    Then the output should be "1"

  @integration @positive @unary
  Scenario: Evaluate double negation
    Given the source code "fn main(): int { printi(-(-5)); return 0; }"
    When the interpreter executes the program
    Then the output should be "5"

  @integration @positive @float
  Scenario: Evaluate float arithmetic
    Given the source code "fn main(): int { printf(3.14 * 2, 2); return 0; }"
    When the interpreter executes the program
    Then the output should be "6.28"

  @integration @positive @variable
  Scenario: Declare and use integer variable
    Given the source code "fn main(): int { let x = 42; printi(x); return 0; }"
    When the interpreter executes the program
    Then the output should be "42"

  @integration @positive @variable
  Scenario: Declare variable with explicit type
    Given the source code "fn main(): int { let x: int = 10; printi(x); return 0; }"
    When the interpreter executes the program
    Then the output should be "10"

  @integration @positive @variable
  Scenario: Assign new value to variable
    Given the source code "fn main(): int { let x = 0; x = 42; printi(x); return 0; }"
    When the interpreter executes the program
    Then the output should be "42"

  @integration @positive @variable
  Scenario: Use variable in expression
    Given the source code "fn main(): int { let x = 5; printi(x * 2); return 0; }"
    When the interpreter executes the program
    Then the output should be "10"

  @integration @positive @variable
  Scenario: Declare multiple variables
    Given the source code "fn main(): int { let a = 1; let b = 2; printi(a + b); return 0; }"
    When the interpreter executes the program
    Then the output should be "3"

  @integration @positive @variable
  Scenario: Declare float variable
    Given the source code "fn main(): int { let pi = 3.14; printf(pi, 2); return 0; }"
    When the interpreter executes the program
    Then the output should be "3.14"

  @integration @positive @variable
  Scenario: Declare string variable
    Given the source code 'fn main(): int { let s = "Hello"; print(s); return 0; }'
    When the interpreter executes the program
    Then the output should be "Hello"

  @integration @positive @string
  Scenario: Print string literal
    Given the source code 'fn main(): int { print("Hello, World!"); return 0; }'
    When the interpreter executes the program
    Then the output should be "Hello, World!"

  @integration @positive @string
  Scenario: String with escape sequences
    Given the source code 'fn main(): int { print("\\n\\t\\"\\\\"); return 0; }'
    When the interpreter executes the program
    Then the output should contain newline and tab

  @integration @positive @builtin
  Scenario: Use printi builtin
    Given the source code "fn main(): int { printi(42); return 0; }"
    When the interpreter executes the program
    Then the output should be "42"

  @integration @positive @builtin
  Scenario: Use printf builtin
    Given the source code "fn main(): int { printf(3.14159, 2); return 0; }"
    When the interpreter executes the program
    Then the output should be "3.14"

  @integration @positive @builtin
  Scenario: Use itos builtin
    Given the source code "fn main(): int { print(itos(42)); return 0; }"
    When the interpreter executes the program
    Then the output should be "42"

  @integration @positive @builtin
  Scenario: Use stoi builtin
    Given the source code 'fn main(): int { printi(stoi("123")); return 0; }'
    When the interpreter executes the program
    Then the output should be "123"

  @integration @positive @builtin
  Scenario: Use itof builtin
    Given the source code "fn main(): int { printf(itof(49), 1); return 0; }"
    When the interpreter executes the program
    Then the output should be "49.0"

  @integration @positive @builtin
  Scenario: Use ftoi builtin
    Given the source code "fn main(): int { printi(ftoi(49.0)); return 0; }"
    When the interpreter executes the program
    Then the output should be "49"

  @integration @positive @builtin
  Scenario: Use strlen builtin
    Given the source code 'fn main(): int { printi(strlen("Hello!")); return 0; }'
    When the interpreter executes the program
    Then the output should be "6"

  @integration @positive @builtin
  Scenario: Use substr builtin
    Given the source code 'fn main(): int { print(substr("Hello!", 2, 2)); return 0; }'
    When the interpreter executes the program
    Then the output should be "ll"

  @integration @positive @builtin
  Scenario: Use sconcat builtin
    Given the source code 'fn main(): int { print(sconcat("Hello", " World")); return 0; }'
    When the interpreter executes the program
    Then the output should be "Hello World"

  @integration @positive @builtin
  Scenario: Use input builtin
    Given the source code "fn main(): int { print(input()); return 0; }"
    And the input buffer contains "test input"
    When the interpreter executes the program
    Then the output should be "test input"

  @integration @positive @type_conversion
  Scenario: Chain type conversions
    Given the source code 'fn main(): int { let s = itos(42); let n = stoi(s); printi(n); return 0; }'
    When the interpreter executes the program
    Then the output should be "42"

  @integration @positive @complex
  Scenario: Multiple outputs
    Given the source code 'fn main(): int { printi(1); print("\\n"); printi(2); print("\\n"); return 0; }'
    When the interpreter executes the program
    Then the output should be "1\n2\n"

  @integration @positive @complex
  Scenario: Complex arithmetic with variables
    Given the source code "fn main(): int { let x = 5; let y = 10; printi(x * y + 3); return 0; }"
    When the interpreter executes the program
    Then the output should be "53"

  @integration @positive @edge
  Scenario: Zero exponent
    Given the source code "fn main(): int { printi(5 ** 0); return 0; }"
    When the interpreter executes the program
    Then the output should be "1"

  @integration @positive @edge
  Scenario: Unary minus with power
    Given the source code "fn main(): int { printi(-5 ** 2); return 0; }"
    When the interpreter executes the program
    Then the output should be "-25"

  @integration @positive @edge
  Scenario: Parenthesized negative in power
    Given the source code "fn main(): int { printi((-5) ** 2); return 0; }"
    When the interpreter executes the program
    Then the output should be "25"

  @integration @negative
  Scenario: Reject undeclared variable
    Given the source code "fn main(): int { printi(undeclared); return 0; }"
    When the interpreter executes the program
    Then execution should fail with UnknownSymbolException

  @integration @negative
  Scenario: Reject duplicate variable declaration
    Given the source code "fn main(): int { let x = 1; let x = 2; return 0; }"
    When the interpreter executes the program
    Then execution should fail with DuplicateSymbolException

  @integration @negative
  Scenario: Reject type mismatch
    Given the source code 'fn main(): int { let x: int = "string"; return 0; }'
    When the interpreter executes the program
    Then execution should fail with TypeErrorException

  @integration @negative
  Scenario: Reject wrong argument type
    Given the source code "fn main(): int { strlen(42); return 0; }"
    When the interpreter executes the program
    Then execution should fail with TypeErrorException

  @integration @negative
  Scenario: Reject wrong argument count
    Given the source code "fn main(): int { printi(); return 0; }"
    When the interpreter executes the program
    Then execution should fail with InvalidFunctionCallException