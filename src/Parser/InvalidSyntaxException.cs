namespace Parser;

#pragma warning disable RCS1194
public class InvalidSyntaxException : Exception
{
    public InvalidSyntaxException(string message)
        : base(message)
    {
    }
}
#pragma warning restore RCS1194