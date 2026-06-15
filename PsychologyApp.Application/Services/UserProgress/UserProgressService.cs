using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Services.UserProgress;

public sealed class UserProgressService(IUserProgressRepository repository) : IUserProgressService
{
    public Task SaveTestResultAsync(string testId, int? score, string summary, string? detailJson = null, CancellationToken cancellationToken = default) =>
        repository.SaveTestResultAsync(new TestResultDTO
        {
            TestId = testId,
            Score = score,
            Summary = summary,
            DetailJson = detailJson,
            CompletedAt = DateTime.UtcNow
        }, cancellationToken);

    public Task<TestResultDTO?> GetLatestTestResultAsync(string testId, CancellationToken cancellationToken = default) =>
        repository.GetLatestTestResultAsync(testId, cancellationToken);

    public Task<IReadOnlyList<TestResultDTO>> GetTestResultHistoryAsync(string testId, int limit = 20, CancellationToken cancellationToken = default) =>
        repository.GetTestResultHistoryAsync(testId, limit, cancellationToken);

    public Task<long> CountTestResultsAsync(CancellationToken cancellationToken = default) =>
        repository.CountTestResultsAsync(cancellationToken);

    public Task<DateTime?> GetLastTechniqueCompletionDateAsync(CancellationToken cancellationToken = default) =>
        repository.GetLastTechniqueCompletionDateAsync(cancellationToken);

    public Task RecordTechniqueCompletionAsync(string itemKey, string moduleName, string pageName, int durationSeconds = 0, CancellationToken cancellationToken = default) =>
        repository.RecordCompletionAsync(new CompletionDTO
        {
            CompletionKind = "technique",
            ItemKey = itemKey,
            ModuleName = moduleName,
            PageName = pageName,
            CompletedAt = DateTime.UtcNow,
            DurationSeconds = durationSeconds
        }, cancellationToken);

    public Task<long> CountTechniqueCompletionsAsync(CancellationToken cancellationToken = default) =>
        repository.CountTechniqueCompletionsAsync(cancellationToken);

    public Task<IReadOnlyList<CompletionDTO>> GetRecentTechniqueCompletionsAsync(int limit = 20, CancellationToken cancellationToken = default) =>
        repository.GetRecentTechniqueCompletionsAsync(limit, cancellationToken);

    public async Task<int> GetStreakDaysAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<DateOnly> dates = await repository.GetCompletionDatesAsync(cancellationToken);
        if (dates.Count == 0)
        {
            return 0;
        }

        int streak = 0;
        DateOnly expected = DateOnly.FromDateTime(DateTime.Today);

        foreach (DateOnly date in dates)
        {
            if (date == expected)
            {
                streak++;
                expected = expected.AddDays(-1);
                continue;
            }

            if (date < expected)
            {
                break;
            }
        }

        return streak;
    }

    public Task<DateTime?> GetLastPracticeDateAsync(string itemKey, CancellationToken cancellationToken = default) =>
        repository.GetLastCompletionForItemAsync(itemKey, cancellationToken);

    public Task<IReadOnlyDictionary<string, DateTime>> GetLastPracticeDatesAsync(
        IReadOnlyList<string> itemKeys,
        CancellationToken cancellationToken = default) =>
        repository.GetLastPracticeDatesAsync(itemKeys, cancellationToken);

    public Task SaveSessionDraftAsync(string techniqueKey, string payloadJson, CancellationToken cancellationToken = default) =>
        repository.SaveSessionDraftAsync(techniqueKey, payloadJson, cancellationToken);

    public Task<string?> GetSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default) =>
        repository.GetSessionDraftAsync(techniqueKey, cancellationToken);

    public Task<IReadOnlySet<string>> GetSessionDraftKeysAsync(
        IReadOnlyList<string> techniqueKeys,
        CancellationToken cancellationToken = default) =>
        repository.GetSessionDraftKeysAsync(techniqueKeys, cancellationToken);

    public Task DeleteSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default) =>
        repository.DeleteSessionDraftAsync(techniqueKey, cancellationToken);

    public Task RecordMoodAsync(int moodLevel, string? note = null, CancellationToken cancellationToken = default) =>
        repository.RecordMoodAsync(new MoodEntryDTO
        {
            MoodLevel = moodLevel,
            Note = note,
            RecordedAt = DateTime.UtcNow
        }, cancellationToken);

    public Task<IReadOnlyList<MoodEntryDTO>> GetRecentMoodsAsync(int limit = 7, CancellationToken cancellationToken = default) =>
        repository.GetRecentMoodsAsync(limit, cancellationToken);
}
