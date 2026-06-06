namespace PsychologyApp.Application.Exceptions;

public sealed class QuotApiLoadException : AppException
{
    public QuotApiLoadException(string message) : base(message)
    {
    }

    public QuotApiLoadException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
