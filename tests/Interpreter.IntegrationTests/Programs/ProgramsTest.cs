using Tests.TestLibrary;

namespace Interpreter.IntegrationTests;

public class ProgramsTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetPrograms))]
    public void Can_exec_program(string prg, string expected, int exitCode)
    {
        FakeEnvironment environment = new();
        Interpreter interpreter = new(environment);

        interpreter.Execute(Samples.GetSampleProgram(prg));
        Assert.Equal(expected, environment.OutputBuffer);
        Assert.Equal(exitCode, interpreter.ExitCode);
    }

    // Из examples.md
    public static TheoryData<string, string, int> GetPrograms()
    {
        return new TheoryData<string, string, int>
        {
            {
                "check_data_types.psvm",
                "45.14\n123\n3.14\n3\n456\n",
                0
            },
        };
    }
}