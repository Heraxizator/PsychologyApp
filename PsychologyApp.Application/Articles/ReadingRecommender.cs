using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Articles;

public enum ReadingPickReason
{
    ContinueCourse,
    ContinueReading,

    /// <summary>A recent questionnaire result (<see cref="ReadingPick.TestId"/>).</summary>
    Test,

    /// <summary>Several hard days in the mood journal.</summary>
    Mood
}

public sealed record ReadingPick(string Id, bool IsCourse, ReadingPickReason Reason, string? TestId = null);

/// <summary>
/// "For you" on the Articles tab, from what the app already knows on the device: questionnaire results, the mood journal, what is
/// being read. A closed table of published cut-offs, not a guess; nothing is sent anywhere.
/// </summary>
public static class ReadingRecommender
{
    public const int MaxPicks = 3;

    /// <summary>A result older than this no longer says how the person is now.</summary>
    public static readonly TimeSpan ResultFreshFor = TimeSpan.FromDays(90);

    public static readonly IReadOnlyList<string> TestIds = ["pcl5", "pcptsd5", "phq9", "isi", "gad7"];

    private const int LowMoodLevel = 2;
    private const int LowMoodDays = 3;
    private static readonly TimeSpan MoodWindow = TimeSpan.FromDays(14);
    private static readonly TimeSpan ContinueReadingFor = TimeSpan.FromDays(14);

    public static IReadOnlyList<ReadingPick> Pick(
        IReadOnlyList<TestResultDTO> latestResults,
        IReadOnlyList<MoodEntryDTO> moods,
        ReadingState state,
        DateTime nowUtc)
    {
        List<ReadingPick> picks = [];

        void Add(ReadingPick pick)
        {
            bool finished = pick.IsCourse && state.Course(pick.Id)?.FinishedUtc is not null;
            bool read = !pick.IsCourse && pick.Reason != ReadingPickReason.ContinueReading && state.Read.Contains(pick.Id);
            if (!finished && !read && picks.All(existing => existing.Id != pick.Id || existing.IsCourse != pick.IsCourse))
            {
                picks.Add(pick);
            }
        }

        foreach (CourseProgress course in state.Courses.Where(course => course.FinishedUtc is null).OrderByDescending(Latest))
        {
            Add(new ReadingPick(course.CourseId, true, ReadingPickReason.ContinueCourse));
        }

        if (state.LastArticleId is { } last && !state.Read.Contains(last) && state.LastOpenedUtc is { } opened && nowUtc - opened <= ContinueReadingFor)
        {
            Add(new ReadingPick(last, false, ReadingPickReason.ContinueReading));
        }

        Dictionary<string, int> scores = latestResults
            .Where(result => result.Score is not null && nowUtc - result.CompletedAt <= ResultFreshFor)
            .GroupBy(result => result.TestId)
            .ToDictionary(group => group.Key, group => group.OrderByDescending(result => result.CompletedAt).First().Score!.Value);

        // Trauma first: its symptoms (sleep, mood, anxiety) show up on the other scales too, and the other courses would miss the cause.
        if (scores.TryGetValue("pcl5", out int pcl5) && pcl5 >= 33)
        {
            Add(new ReadingPick(CourseIds.Trauma, true, ReadingPickReason.Test, "pcl5"));
        }
        else if (scores.TryGetValue("pcptsd5", out int screen) && screen >= 3)
        {
            Add(new ReadingPick(CourseIds.Trauma, true, ReadingPickReason.Test, "pcptsd5"));
        }

        if (scores.TryGetValue("phq9", out int phq9))
        {
            if (phq9 >= 10)
            {
                Add(new ReadingPick(CourseIds.Mood, true, ReadingPickReason.Test, "phq9"));
            }
            else if (phq9 >= 5)
            {
                Add(new ReadingPick(ArticleIds.LowMood, false, ReadingPickReason.Test, "phq9"));
            }
        }

        if (scores.TryGetValue("isi", out int isi))
        {
            if (isi >= 15)
            {
                Add(new ReadingPick(CourseIds.Sleep, true, ReadingPickReason.Test, "isi"));
            }
            else if (isi >= 8)
            {
                Add(new ReadingPick(ArticleIds.Sleep, false, ReadingPickReason.Test, "isi"));
            }
        }

        if (scores.TryGetValue("gad7", out int gad7))
        {
            if (gad7 >= 10)
            {
                Add(new ReadingPick(CourseIds.Anxiety, true, ReadingPickReason.Test, "gad7"));
            }
            else if (gad7 >= 5)
            {
                Add(new ReadingPick(ArticleIds.Anxiety, false, ReadingPickReason.Test, "gad7"));
            }
        }

        int lowDays = moods
            .Where(mood => mood.MoodLevel <= LowMoodLevel && nowUtc - mood.RecordedAt <= MoodWindow)
            .Select(mood => mood.RecordedAt.Date)
            .Distinct()
            .Count();
        if (lowDays >= LowMoodDays)
        {
            Add(new ReadingPick(CourseIds.Mood, true, ReadingPickReason.Mood));
        }

        return picks.Take(MaxPicks).ToList();
    }

    private static DateTime Latest(CourseProgress course) => course.Days.Count > 0 ? course.Days.Max(day => day.CompletedUtc) : course.StartedUtc;
}
