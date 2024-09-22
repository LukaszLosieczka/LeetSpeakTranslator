namespace MyWebApplication.Exceptions;

public class TranslationException : Exception
{
    public TranslationException(string message)
        : base(message)
    {   
    }
}