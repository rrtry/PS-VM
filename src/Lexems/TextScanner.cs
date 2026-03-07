namespace Lexems;

/// <summary>
///  Сканирует текст выражения, предоставляя три операции: Peek(N), Advance() и IsEnd().
/// </summary>
public class TextScanner(string expr)
{
    private readonly string source = expr;
    private int pos;

    public int GetPosition()
    {
        return pos;
    }

    public void SetPosition(int newPos)
    {
        pos = newPos;
    }

    /// <summary>
    ///  Читает на N символов вперёд текущей позиции (по умолчанию N=0).
    /// </summary>
    public char Peek(int n = 0)
    {
        int position = pos + n;
        return position >= source.Length ? '\0' : source[position];
    }

    /// <summary>
    ///  Сдвигает текущую позицию на один символ.
    /// </summary>
    public void Advance()
    {
        pos++;
    }

    public bool IsEnd()
    {
        return pos >= source.Length;
    }
}