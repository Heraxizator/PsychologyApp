namespace PsychologyApp.Presentation.Entities.Profile;

public sealed record MoodNoteItem(
    long MoodEntryId,
    DateOnly Day,
    string DateText,
    string TimeText,
    string NoteText,
    bool HasNote,
    int MoodLevel,
    string MoodDisplay,
    bool IsToday);
