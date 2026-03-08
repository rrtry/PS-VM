namespace Execution;

public class ConsoleEnvironment : IEnvironment
{
    private readonly List<string> evaluated = new List<string>();

    public List<string> GetEvaluated()
    {
        return evaluated;
    }

    public string Input()
    {
        return Console.ReadLine() ?? throw new EndOfStreamException();
    }

    public void Print(string result)
    {
        evaluated.Add(result);
        Console.WriteLine(result);
    }
}