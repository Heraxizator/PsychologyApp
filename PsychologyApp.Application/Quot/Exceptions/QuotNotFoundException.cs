namespace PsychologyApp.Application.Exceptions;

public sealed class QuotNotFoundException : NotFoundException
{
    public QuotNotFoundException(string message) : base(message)
    {
    }
}
