using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Conversation;

namespace PsychologyApp.Application.Articles;

public enum CourseDayOutcome
{
    Done,

    /// <summary>The answer read as a crisis: the day is not closed, the screen leads to help.</summary>
    Crisis,

    /// <summary>The day is not open yet (an earlier day is not done, or it opens tomorrow).</summary>
    NotOpen
}

public interface IReadingService
{
    Task<ReadingState> GetAsync(CancellationToken cancellationToken = default);

    Task OpenedAsync(string articleId, CancellationToken cancellationToken = default);

    Task MarkReadAsync(string articleId, CancellationToken cancellationToken = default);

    /// <returns>Whether the article is saved now.</returns>
    Task<bool> ToggleSavedAsync(string articleId, CancellationToken cancellationToken = default);

    Task StartCourseAsync(string courseId, CancellationToken cancellationToken = default);

    Task<CourseDayOutcome> CompleteDayAsync(string courseId, int day, string? reflection, CancellationToken cancellationToken = default);

    Task ResetCourseAsync(string courseId, CancellationToken cancellationToken = default);

    CourseDayState StateOf(int day, CourseProgress? progress);
}

public sealed class ReadingService(IReadingRepository repository, ICrisisDetector crisisDetector, TimeProvider? timeProvider = null) : IReadingService
{
    public const int MaxReflection = 1_000;

    private readonly TimeProvider _time = timeProvider ?? TimeProvider.System;

    // Every change is a read-modify-write of one document: two taps in a row must not lose each other's change.
    private readonly SemaphoreSlim _writeGate = new(1, 1);

    public async Task<ReadingState> GetAsync(CancellationToken cancellationToken = default) =>
        await repository.GetAsync(cancellationToken).ConfigureAwait(false) ?? new ReadingState();

    public Task OpenedAsync(string articleId, CancellationToken cancellationToken = default) =>
        UpdateAsync(state => state with { LastArticleId = articleId, LastOpenedUtc = Now() }, cancellationToken);

    public Task MarkReadAsync(string articleId, CancellationToken cancellationToken = default) =>
        UpdateAsync(state => state.Read.Contains(articleId) ? state : state with { Read = [.. state.Read, articleId] }, cancellationToken);

    public async Task<bool> ToggleSavedAsync(string articleId, CancellationToken cancellationToken = default)
    {
        ReadingState updated = await UpdateAsync(
            state => state with { Saved = state.Saved.Contains(articleId) ? [.. state.Saved.Where(id => id != articleId)] : [.. state.Saved, articleId] },
            cancellationToken).ConfigureAwait(false);
        return updated.Saved.Contains(articleId);
    }

    public Task StartCourseAsync(string courseId, CancellationToken cancellationToken = default) =>
        UpdateAsync(
            state => state.Course(courseId) is not null
                ? state
                : state with { Courses = [.. state.Courses, new CourseProgress(courseId, Now(), [])] },
            cancellationToken);

    public async Task<CourseDayOutcome> CompleteDayAsync(string courseId, int day, string? reflection, CancellationToken cancellationToken = default)
    {
        string? words = string.IsNullOrWhiteSpace(reflection) ? null : reflection.Trim();
        if (words is { Length: > MaxReflection })
        {
            words = words[..MaxReflection];
        }

        if (crisisDetector.IsCrisis(words))
        {
            return CourseDayOutcome.Crisis;
        }

        CourseDayOutcome outcome = CourseDayOutcome.NotOpen;
        await UpdateAsync(
            state =>
            {
                CourseProgress progress = state.Course(courseId) ?? new CourseProgress(courseId, Now(), []);
                if (StateOf(day, progress) != CourseDayState.Open)
                {
                    return state;
                }

                outcome = CourseDayOutcome.Done;
                int total = ArticleContent.FindCourse(courseId, english: false)?.Days.Count ?? int.MaxValue;
                List<CourseDayRecord> days = [.. progress.Days, new CourseDayRecord(day, Now(), words)];
                CourseProgress updated = progress with { Days = days, FinishedUtc = days.Count >= total ? Now() : null };
                return state with { Courses = [.. state.Courses.Where(course => course.CourseId != courseId), updated] };
            },
            cancellationToken).ConfigureAwait(false);
        return outcome;
    }

    public Task ResetCourseAsync(string courseId, CancellationToken cancellationToken = default) =>
        UpdateAsync(state => state with { Courses = [.. state.Courses.Where(course => course.CourseId != courseId)] }, cancellationToken);

    public CourseDayState StateOf(int day, CourseProgress? progress) =>
        CourseRules.StateOf(day, progress, DateOnly.FromDateTime(_time.GetLocalNow().DateTime), _time.LocalTimeZone);

    private DateTime Now() => _time.GetUtcNow().UtcDateTime;

    private async Task<ReadingState> UpdateAsync(Func<ReadingState, ReadingState> change, CancellationToken cancellationToken)
    {
        await _writeGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ReadingState current = await GetAsync(cancellationToken).ConfigureAwait(false);
            ReadingState updated = change(current);
            if (!ReferenceEquals(updated, current))
            {
                await repository.SaveAsync(updated, cancellationToken).ConfigureAwait(false);
            }

            return updated;
        }
        finally
        {
            _writeGate.Release();
        }
    }
}
