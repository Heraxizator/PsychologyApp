namespace PsychologyApp.Presentation.Entities.Profile;

public sealed class PracticeHistoryItem
{
    public string DateText { get; init; } = string.Empty;

    public string TechniqueName { get; init; } = string.Empty;

    public string IconName { get; init; } = string.Empty;

    public string DurationText { get; init; } = string.Empty;

    public bool HasDuration { get; init; }

    public string DisplayText { get; init; } = string.Empty;
}
