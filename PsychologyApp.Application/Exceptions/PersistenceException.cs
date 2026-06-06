namespace PsychologyApp.Application.Exceptions;

public sealed class PersistenceException : AppException
{
    public PersistenceException(string message) : base(message)
    {
    }
}
