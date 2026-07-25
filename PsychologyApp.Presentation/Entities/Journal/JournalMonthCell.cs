namespace PsychologyApp.Presentation.Entities.Journal;

public sealed class JournalMonthCell
{
    public DateOnly? Date { get; init; }
    public string DayNumber { get; init; } = string.Empty;
    public string MoodGlyph { get; init; } = string.Empty;
    public bool HasEntry { get; init; }
    public bool IsEnabled { get; init; }
    public bool IsSelected { get; init; }
    public bool IsPlaceholder => Date is null;
}
