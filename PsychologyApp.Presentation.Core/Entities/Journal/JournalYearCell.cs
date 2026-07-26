namespace PsychologyApp.Presentation.Entities.Journal;

public sealed class JournalYearCell
{
    public DateOnly? Date { get; init; }
    public string MoodGlyph { get; init; } = string.Empty;
    public int? MoodLevel { get; init; }
    public bool HasEntry { get; init; }
    public bool IsEnabled { get; init; }
    public bool IsSelected { get; init; }
    public bool IsPlaceholder => Date is null;
}
