using PsychologyApp.Presentation.Entities.Profile;

namespace PsychologyApp.Presentation.Entities.Journal;

public sealed record JournalTimelineDayGroup(
    DateOnly Day,
    string DateLabel,
    IReadOnlyList<MoodNoteItem> Entries);
