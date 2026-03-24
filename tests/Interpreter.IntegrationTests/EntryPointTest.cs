using Parser;
using Semantics.Exceptions;
using Tests.TestLibrary.TestDoubles;

namespace Interpreter.IntegrationTests;

public class EntryPointTest
{
    [Theory]
    [MemberData(nameof(GetEntryPointData))]
    public void Can_exec_main(string code, string expected, int exitCode)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);

        interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
        Assert.Equal(exitCode, interpreter.ExitCode);
    }

    [Theory]
    [MemberData(nameof(GetInvalidEntryPointData))]
    public void Throws_on_invalid_entry_point_declaration(string code, Type expectedExceptionType)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);
        Assert.Throws(expectedExceptionType, () => interpreter.Execute(code));
    }

    public static TheoryData<string, string, int> GetEntryPointData()
    {
        return new TheoryData<string, string, int>
        {
            {
                """fn main(): int { print("Program exited with code 0"); return 0; }""", "Program exited with code 0", 0
            },
            {
                """fn main(): int { print("Program exited with code 1"); return 1; }""", "Program exited with code 1", 1
            },
        };
    }

    public static TheoryData<string, Type> GetInvalidEntryPointData()
    {
        return new TheoryData<string, Type>
        {
            // Без точки входа
            { "printi(1);", typeof(UnexpectedLexemeException) },

            // неверное определение main
            { "fn func(): unit { printi(0); }", typeof(InvalidDeclarationException) },

            // main не имеет return с int в конце
            { "fn main(): int { printi(0); }", typeof(TypeErrorException) },

            // main содержит return неверного типа
            { """fn main(): int { printi(0); return ""; }""", typeof(TypeErrorException) },

            // main содержит return неверного типа
            { "fn main(): int { printi(0); return 0.0; }", typeof(TypeErrorException) },

            // main содержит return неверного типа
            { "fn main(): int { printi(0); return; }", typeof(TypeErrorException) },

            // недостижимый код
            { "fn main(): int { return 1; printi(0); }", typeof(InvalidOperationException) },
        };
    }
}