Feature: Semantic Analysis - Type and Symbol Validation
  As a compiler developer
  I want the semantic analyzer to validate types and symbols
  So that invalid programs are rejected before code generation

  @semantic @positive @symbol
  Scenario: Resolve declared variable
    Given the source code "fn main(): int { let x = 5; printi(x); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    Then no semantic errors should occur

  @semantic @positive @symbol
  Scenario: Resolve builtin function
    Given the source code "fn main(): int { printi(42); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    Then no semantic errors should occur

  @semantic @positive @type
  Scenario: Check integer addition type
    Given the source code "fn main(): int { return 1 + 2; }"
    When the parser parses the program
    And semantic analysis runs
    Then no semantic errors should occur

  @semantic @positive @type
  Scenario: Check float addition type
    Given the source code "fn main(): int { return 1.5 + 2.5; }"
    When the parser parses the program
    And semantic analysis runs
    Then no semantic errors should occur

  @semantic @positive @type
  Scenario: Infer variable type from initializer
    Given the source code "fn main(): int { let x = 5; return x; }"
    When the parser parses the program
    And semantic analysis runs
    Then no semantic errors should occur
    And variable "x" should have inferred type Int

  @semantic @positive @type
  Scenario: Accept integer argument to float parameter
    Given the source code "fn main(): int { printf(42, 1); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    Then no semantic errors should occur

  @semantic @positive @function
  Scenario: Validate correct argument count for printi
    Given the source code "fn main(): int { printi(42); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    Then no semantic errors should occur

  @semantic @positive @function
  Scenario: Validate correct argument count for printf
    Given the source code "fn main(): int { printf(3.14, 2); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    Then no semantic errors should occur

  @semantic @positive @function
  Scenario: Validate string concatenation
    Given the source code 'fn main(): int { print(sconcat("a", "b")); return 0; }'
    When the parser parses the program
    And semantic analysis runs
    Then no semantic errors should occur

  @semantic @positive @function
  Scenario: Validate type conversion functions
    Given the source code 'fn main(): int { let s = itos(42); let n = stoi(s); printi(n); return 0; }'
    When the parser parses the program
    And semantic analysis runs
    Then no semantic errors should occur

  @semantic @negative @symbol
  Scenario: Reject undeclared variable
    Given the source code "fn main(): int { printi(x); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    Then a semantic error of type UnknownSymbolException should occur

  @semantic @negative @symbol
  Scenario: Reject duplicate variable declaration
    Given the source code "fn main(): int { let x = 1; let x = 2; return 0; }"
    When the parser parses the program
    And semantic analysis runs
    Then a semantic error of type DuplicateSymbolException should occur

  @semantic @negative @symbol
  Scenario: Reject unknown function call
    Given the source code "fn main(): int { unknownFunc(); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    Then a semantic error of type UnknownSymbolException should occur

  @semantic @negative @type
  Scenario: Reject type mismatch in variable declaration
    Given the source code 'fn main(): int { let x: int = "string"; return 0; }'
    When the parser parses the program
    And semantic analysis runs
    Then a semantic error of type TypeErrorException should occur

  @semantic @negative @type
  Scenario: Reject string argument to integer parameter
    Given the source code 'fn main(): int { printi("hello"); return 0; }'
    When the parser parses the program
    And semantic analysis runs
    Then a semantic error of type TypeErrorException should occur

  @semantic @negative @type
  Scenario: Reject integer argument to string parameter
    Given the source code "fn main(): int { strlen(42); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    Then a semantic error of type TypeErrorException should occur

  @semantic @negative @function
  Scenario: Reject too few arguments for printi
    Given the source code "fn main(): int { printi(); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    Then a semantic error of type InvalidFunctionCallException should occur

  @semantic @negative @function
  Scenario: Reject too many arguments for printi
    Given the source code "fn main(): int { printi(1, 2); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    Then a semantic error of type InvalidFunctionCallException should occur

  @semantic @negative @function
  Scenario: Reject too few arguments for strlen
    Given the source code "fn main(): int { strlen(); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    Then a semantic error of type InvalidFunctionCallException should occur

  @semantic @negative @function
  Scenario: Reject too many arguments for strlen
    Given the source code 'fn main(): int { strlen("a", "b"); return 0; }'
    When the parser parses the program
    And semantic analysis runs
    Then a semantic error of type InvalidFunctionCallException should occur

  @semantic @negative @function
  Scenario: Reject too few arguments for sconcat
    Given the source code "fn main(): int { sconcat(); return 0; }"
    When the parser parses the program
    And semantic analysis runs
    Then a semantic error of type InvalidFunctionCallException should occur

  @semantic @negative @function
  Scenario: Reject wrong argument count for sconcat
    Given the source code 'fn main(): int { sconcat("a", "b", "c"); return 0; }'
    When the parser parses the program
    And semantic analysis runs
    Then a semantic error of type InvalidFunctionCallException should occur