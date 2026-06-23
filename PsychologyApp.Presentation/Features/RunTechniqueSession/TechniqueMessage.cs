using PsychologyApp.Application.Models;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public enum TechniqueMessageType
{
    Add,
    Remove,
    Change
}

public class TechniqueMessage
{
    public TechniqueMessageType MessageType { get; set; }
    public TechniqueDTO Technique { get; set; } = default;
}
