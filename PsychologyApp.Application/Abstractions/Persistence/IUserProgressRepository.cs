using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IUserProgressRepository
{
    Task SaveTestResultAsync(TestResultDTO result, CancellationToken cancellationToken = default);
    Task<TestResultDTO?> GetLatestTestResultAsync(string testId, CancellationToken cancellationToken = default);
    Task<TestResultDTO?> GetMostRecentTestResultAsync(TimeSpan within, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TestResultDTO>> GetTestResultHistoryAsync(string testId, int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TestResultDTO>> GetLatestTestResultsAsync(IReadOnlyList<string> testIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(string TestId, int Count)>> GetTestResultCountsAsync(IReadOnlyList<string> testIds, CancellationToken cancellationToken = default);
    Task<long> CountTestResultsAsync(CancellationToken cancellationToken = default);
    Task<DateTime?> GetLastTechniqueCompletionDateAsync(CancellationToken cancellationToken = default);

    Task RecordCompletionAsync(CompletionDTO completion, CancellationToken cancellationToken = default);
    Task<long> RecordSessionOutcomeAsync(SessionOutcomeRequest request, CancellationToken cancellationToken = default);
    Task<long> CountTechniqueCompletionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CompletionDTO>> GetRecentTechniqueCompletionsAsync(int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DateOnly>> GetCompletionDatesAsync(CancellationToken cancellationToken = default);
    Task<DateTime?> GetLastCompletionForItemAsync(string itemKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<string, DateTime>> GetLastPracticeDatesAsync(IReadOnlyList<string> itemKeys, CancellationToken cancellationToken = default);

    Task SaveSessionDraftAsync(string techniqueKey, string payloadJson, CancellationToken cancellationToken = default);
    Task<string?> GetSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default);
    Task<IReadOnlySet<string>> GetSessionDraftKeysAsync(IReadOnlyList<string> techniqueKeys, CancellationToken cancellationToken = default);
    Task DeleteSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default);

    Task RecordMoodAsync(MoodEntryDTO entry, CancellationToken cancellationToken = default);
    Task UpdateMoodEntryAsync(long moodEntryId, int moodLevel, string? note, CancellationToken cancellationToken = default);
    Task DeleteMoodEntryAsync(long moodEntryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MoodEntryDTO>> GetRecentMoodsAsync(int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MoodEntryDTO>> GetMoodsAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        int limit,
        CancellationToken cancellationToken = default);

    Task UpdateSessionResultPostIntensityAsync(long sessionResultId, int postIntensity, CancellationToken cancellationToken = default);
    Task<SessionResultDTO?> GetSessionResultAsync(long sessionResultId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SessionResultDTO>> GetRecentSessionResultsAsync(int limit, CancellationToken cancellationToken = default);
    Task<int> CountDistinctTechniqueCompletionsForItemsBetweenAsync(
        IReadOnlyList<string> itemKeys,
        DateTime sinceUtc,
        DateTime beforeUtc,
        CancellationToken cancellationToken = default);
}
