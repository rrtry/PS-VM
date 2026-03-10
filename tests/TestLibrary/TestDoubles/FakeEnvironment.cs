using System.Globalization;
using System.Text;

using VirtualMachine;

namespace Tests.TestLibrary.TestDoubles;

/// <summary>
/// Имитирует средства ввода-вывода для тестов.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly Queue<string> _input = new();
    private readonly StringBuilder _outputBuffer = new();
    private readonly StringBuilder _flushedOutput = new();

    public List<string> Evaluated { get; } = new();

    public string BufferedOutput => _outputBuffer.ToString();

    public string FlushedOutput => _flushedOutput.ToString();

    public void AddInput(string text)
    {
        _input.Enqueue(text);
    }

    public string Input()
    {
        string? text = null;
        _input.TryDequeue(out text);

        if (text == null)
        {
            throw new EndOfStreamException("stdin is empty");
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
    }

    public void PrintFloat(double value, int precision)
    {
        _outputBuffer.Append(string.Format("{0:F2}", value));
    }

    public void Flush()
    {
        _flushedOutput.Append(_outputBuffer.ToString());
        _outputBuffer.Clear();
    }
}