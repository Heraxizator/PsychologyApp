using System.Windows.Input;
using PsychologyApp.Application.Models;

namespace PsychologyApp.Presentation.Modules.Physics;

public sealed class PhysicsReasonItem
{
    public long ReasonId { get; init; }
    public string? Title { get; init; }
    public string? Subtitle { get; init; }
    public string? Solution { get; init; }
    public bool IsExpanded { get; set; }
    public ICommand? ToggleExpandCommand { get; set; }
    public IReadOnlyList<PhysicsTechniqueSuggestion> SuggestedTechniques { get; init; } = [];

    public static PhysicsReasonItem FromDto(ReasonDTO dto, IReadOnlyList<PhysicsTechniqueSuggestion> suggestions) =>
        new()
        {
            ReasonId = dto.ReasonId,
            Title = dto.Title,
            Subtitle = dto.Subtitle,
            Solution = dto.Solution,
            SuggestedTechniques = suggestions
        };
}

public sealed class PhysicsTechniqueSuggestion
{
    public string Title { get; init; } = string.Empty;
    public ICommand? OpenCommand { get; init; }
}
