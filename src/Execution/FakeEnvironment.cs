namespace Execution;

public class FakeEnvironment : IEnvironment
{
    private readonly List<string> evaluated = new List<string>();

    private int inputIndex = 0;

    private List<string> programInput = new List<string>();

    public string Input()
    {
        if (inputIndex >= programInput.Count)
        {
            throw new EndOfStreamException();
        }

        return programInput[inputIndex++];
    }

    public void Print(string result)
    {
        evaluated.Add(result);
    }

    public List<string> GetEvaluated()
    {
        return evaluated;
    }

    public void SetProgramInput(List<string> input)
    {
        programInput = input;
    }
}