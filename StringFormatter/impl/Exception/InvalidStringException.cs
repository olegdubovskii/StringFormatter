namespace StringFormatter.impl.Exception;

public class InvalidStringException : System.Exception
{
    public InvalidStringException()
    {
    }

    public InvalidStringException(string message)
        : base(message)
    {
    }

    public InvalidStringException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}