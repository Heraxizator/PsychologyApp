namespace PsychologyApp.Presentation.Features.ManageJournal;

/// <summary>
/// The question of the day above the journal note: one of a fixed set, chosen by the date, so it stays the same all day and changes
/// the next. Questions are numbers 1..<see cref="Count"/>; their texts live in the resources.
/// </summary>
public static class JournalPrompts
{
    public const int Count = 14;

    public static int For(DateOnly day) => day.DayNumber % Count + 1;
}
