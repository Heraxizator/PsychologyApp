namespace PsychologyApp.Presentation.Features.ManageJournal;

public enum JournalMoodTrend
{
    None,
    Flat,
    Up,
    Down
}

public static class JournalMoodTrendResolver
{
    public static JournalMoodTrend Resolve(IReadOnlyList<int> moodLevelsOldestFirst)
    {
        if (moodLevelsOldestFirst.Count == 0)
        {
            return JournalMoodTrend.None;
        }

        if (moodLevelsOldestFirst.Count == 1)
        {
            return JournalMoodTrend.Flat;
        }

        int first = moodLevelsOldestFirst[0];
        int last = moodLevelsOldestFirst[^1];
        if (last > first)
        {
            return JournalMoodTrend.Up;
        }

        if (last < first)
        {
            return JournalMoodTrend.Down;
        }

        return JournalMoodTrend.Flat;
    }
}
