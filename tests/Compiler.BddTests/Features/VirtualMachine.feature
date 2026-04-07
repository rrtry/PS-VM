Feature: Virtual Machine - Execution
  As a compiler developer
  I want the virtual machine to correctly execute bytecode
  So that programs produce the expected output

  @vm @positive @stack
  Scenario: Push value onto stack
    Given VM instructions [Push(42)]
    When the VM executes
    Then the stack should contain 42

  @vm @positive @stack
  Scenario: Pop value from stack
    Given VM instructions [Push(42), Pop]
    When the VM executes
    Then the stack should be empty

  @vm @positive @arithmetic
  Scenario: Add two integers
    Given VM instructions [Push(3), Push(7), Add]
    When the VM executes
    Then the stack should contain 10

  @vm @positive @arithmetic
  Scenario: Subtract two integers
    Given VM instructions [Push(10), Push(3), Subtract]
    When the VM executes
    Then the stack should contain 7

  @vm @positive @arithmetic
  Scenario: Multiply two integers
    Given VM instructions [Push(4), Push(5), Multiply]
    When the VM executes
    Then the stack should contain 20

  @vm @positive @arithmetic
  Scenario: Divide two integers
    Given VM instructions [Push(10), Push(2), Divide]
    When the VM executes
    Then the stack should contain 5

  @vm @positive @arithmetic
  Scenario: Modulo operation
    Given VM instructions [Push(10), Push(3), Modulo]
    When the VM executes
    Then the stack should contain 1

  @vm @positive @arithmetic
  Scenario: Power operation
    Given VM instructions [Push(2), Push(10), Power]
    When the VM executes
    Then the stack should contain 1024

  @vm @positive @unary
  Scenario: Negate value
    Given VM instructions [Push(5), Negate]
    When the VM executes
    Then the stack should contain -5

  @vm @positive @unary
  Scenario: Double negate
    Given VM instructions [Push(-5), Negate]
    When the VM executes
    Then the stack should contain 5

  @vm @positive @unary
  Scenario: Negate negative value
    Given VM instructions [Push(-42), Negate]
    When the VM executes
    Then the stack should contain 42

  @vm @positive @variable
  Scenario: Store and load variable
    Given VM instructions [Push(10), StoreLocal(x), LoadLocal(x)]
    When the VM executes
    Then the stack should contain 10

  @vm @positive @variable
  Scenario: Overwrite variable
    Given VM instructions [Push(5), StoreLocal(x), Push(10), StoreLocal(x), LoadLocal(x)]
    When the VM executes
    Then the stack should contain 10

  @vm @positive @halt
  Scenario: Halt with return code
    Given VM instructions [Push(42), Halt]
    When the VM executes
    Then the exit code should be 42

  @vm @positive @complex
  Scenario: Complex arithmetic expression
    Given VM instructions [Push(1), Push(2), Add, Push(3), Multiply]
    When the VM executes
    Then the stack should contain 9

  @vm @positive @complex
  Scenario: Multiple operations
    Given VM instructions [Push(10), Push(3), Push(2), Multiply, Add]
    When the VM executes
    Then the stack should contain 16