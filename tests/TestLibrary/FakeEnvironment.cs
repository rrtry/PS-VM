using System.Globalization;
using System.Text;

using VirtualMachine;

namespace Tests.TestLibrary;

/// <summary>
/// Имитирует средства ввода-вывода для тестов.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly Queue<string> _input = new();
    private readonly StringBuilder _outputBuffer = new();

    public List<string> Evaluated { get; } = new();

    public string OutputBuffer => _outputBuffer.ToString();

    public void AddInput(string text)
    {
        _input.Enqueue(text);
    }

    public string Input()
    {
        string? text;
        _input.TryDequeue(out text);

        if (text == null)
        {
            throw new EndOfStreamException("EOF reached");
        }

        return text;
    }

    public void Print(string text)
    {
        _outputBuffer.Append(text);
        Evaluated.Add(text);
    }

    public void PrintInt(int value)
    {
        _outputBuffer.Append(value.ToString(CultureInfo.InvariantCulture));
        Evaluated.Add(value.ToString(CultureInfo.InvariantCulture));
    }

    public void PrintFloat(double value, int precision)
    {
        string output = value.ToString($"F{precision}", CultureInfo.InvariantCulture);
        _outputBuffer.Append(output);
        Evaluated.Add(output);
    }
}