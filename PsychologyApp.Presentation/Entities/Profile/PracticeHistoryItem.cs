namespace PsychologyApp.Presentation.Entities.Profile;

using System.Windows.Input;

public sealed class PracticeHistoryItem
{
    public string DateText { get; init; } = string.Empty;

    public string TechniqueName { get; init; } = string.Empty;

    public string IconName { get; init; } = string.Empty;

    public string DurationText { get; init; } = string.Empty;

    public bool HasDuration { get; init; }

    public string ItemKey { get; init; } = string.Empty;

    public bool CanOpen { get; init; }

    public ICommand? TapCommand { get; init; }

    public string DisplayText { get; init; } = string.Empty;

    public string SudsDeltaText { get; init; } = string.Empty;

    public bool HasSudsDelta { get; init; }
}
