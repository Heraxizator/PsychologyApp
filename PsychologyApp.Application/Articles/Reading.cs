namespace PsychologyApp.Application.Articles;

/// <param name="Day">1-based.</param>
/// <param name="Reflection">The person's own words: kept only in the encrypted database and the backup.</param>
public sealed record CourseDayRecord(int Day, DateTime CompletedUtc, string? Reflection);

public sealed record CourseProgress(string CourseId, DateTime StartedUtc, IReadOnlyList<CourseDayRecord> Days, DateTime? FinishedUtc = null)
{
    public bool IsDone(int day) => Days.Any(record => record.Day == day);
}

/// <summary>What the person has read, kept and is going through. One small document, read and written whole.</summary>
public sealed record ReadingState
{
    public IReadOnlyList<string> Read { get; init; } = [];
    public IReadOnlyList<string> Saved { get; init; } = [];

    /// <summary>The article opened last, for "continue reading" until it is read to the end.</summary>
    public string? LastArticleId { get; init; }
    public DateTime? LastOpenedUtc { get; init; }

    public IReadOnlyList<CourseProgress> Courses { get; init; } = [];

    public bool IsEmpty => Read.Count == 0 && Saved.Count == 0 && LastArticleId is null && Courses.Count == 0;

    public CourseProgress? Course(string courseId) => Courses.FirstOrDefault(course => course.CourseId == courseId);
}

public enum CourseDayState
{
    Done,
    Open,

    /// <summary>The previous day was closed today: the next one opens tomorrow, so there is time to try the task.</summary>
    Tomorrow,

    /// <summary>An earlier day is not done yet.</summary>
    Locked
}

/// <summary>The pace of a course: one day at a time, the next on a later calendar day, because the tasks need time to be tried.</summary>
public static class CourseRules
{
    public static CourseDayState StateOf(int day, CourseProgress? progress, DateOnly today, TimeZoneInfo zone)
    {
        if (progress?.IsDone(day) == true)
        {
            return CourseDayState.Done;
        }

        if (day == 1)
        {
            return CourseDayState.Open;
        }

        CourseDayRecord? previous = progress?.Days.FirstOrDefault(record => record.Day == day - 1);
        if (previous is null)
        {
            return CourseDayState.Locked;
        }

        DateOnly closedOn = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(previous.CompletedUtc, DateTimeKind.Utc), zone));
        return closedOn < today ? CourseDayState.Open : CourseDayState.Tomorrow;
    }

    /// <summary>The first day not done yet, or null when the course is finished.</summary>
    public static int? NextDay(Course course, CourseProgress? progress) =>
        Enumerable.Range(1, course.Days.Count).Cast<int?>().FirstOrDefault(day => progress?.IsDone(day!.Value) != true);
}

/// <param name="Before">The score the course started from, or null when there is only one result.</param>
/// <param name="Now">The latest score, or null when the test was never taken.</param>
public sealed record CourseMeasurement(int? Before, int? Now);

/// <summary>Whether the course helps, from the course's questionnaire: the result it started from against the latest one.</summary>
public static class CourseMeasure
{
    /// <summary>A result older than this before the start does not say where the course started from.</summary>
    public static readonly TimeSpan BaselineFreshFor = TimeSpan.FromDays(30);

    public static CourseMeasurement Of(IReadOnlyList<Models.TestResultDTO> history, DateTime? startedUtc)
    {
        List<Models.TestResultDTO> scored = [.. history.Where(result => result.Score is not null).OrderBy(result => result.CompletedAt)];
        if (scored.Count == 0)
        {
            return new CourseMeasurement(null, null);
        }

        Models.TestResultDTO latest = scored[^1];
        if (startedUtc is not { } started)
        {
            return new CourseMeasurement(null, latest.Score);
        }

        Models.TestResultDTO? baseline =
            scored.LastOrDefault(result => result.CompletedAt <= started && started - result.CompletedAt <= BaselineFreshFor)
            ?? scored.FirstOrDefault(result => result.CompletedAt > started);
        return baseline is null || ReferenceEquals(baseline, latest)
            ? new CourseMeasurement(null, latest.Score)
            : new CourseMeasurement(baseline.Score, latest.Score);
    }
}
