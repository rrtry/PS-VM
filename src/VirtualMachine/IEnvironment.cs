namespace VirtualMachine;

public interface IEnvironment
{
    /// <summary>
    /// Читает строку из потока ввода либо кидает исключение, если достигнут конец файла.
    /// </summary>
    public string Input();

    /// <summary>
    /// Печатает текст в поток вывода.
    /// </summary>
    public void Print(string text);

    /// <summary>
    /// Печатает число в поток вывода.
    /// </summary>
    public void PrintInt(int value);

    /// <summary>
    /// Печатает вещественное число в поток вывода.
    /// </summary>
    public void PrintFloat(double value, int precision);

    /// <summary>
    /// Сбрасывает накопленный буфер вывода в поток вывода.
    /// </summary>
    public void Flush();
}