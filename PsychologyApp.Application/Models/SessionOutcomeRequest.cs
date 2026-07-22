namespace PsychologyApp.Application.Models;

public sealed class SessionOutcomeRequest
{
    public required string ItemKey { get; init; }

    public required string ModuleName { get; init; }

    public required string PageName { get; init; }

    public int DurationSeconds { get; init; }

    public string? PayloadJson { get; init; }

    public int? PreIntensity { get; init; }

    public string? ProgramType { get; init; }

    public int? ProgramWeek { get; init; }

    public bool DeleteDraft { get; init; } = true;
}
