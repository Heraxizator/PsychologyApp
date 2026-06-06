namespace PsychologyApp.Application.Exceptions;

public sealed class TechniqueNotFoundException : NotFoundException
{
    public TechniqueNotFoundException(string message) : base(message)
    {
    }
}
