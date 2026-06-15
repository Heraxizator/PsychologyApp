using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Services.UserProgress;

public interface IUserProgressService
{
    Task SaveTestResultAsync(string testId, int? score, string summary, string? detailJson = null, CancellationToken cancellationToken = default);
    Task<TestResultDTO?> GetLatestTestResultAsync(string testId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TestResultDTO>> GetTestResultHistoryAsync(string testId, int limit = 20, CancellationToken cancellationToken = default);
    Task<long> CountTestResultsAsync(CancellationToken cancellationToken = default);
    Task<DateTime?> GetLastTechniqueCompletionDateAsync(CancellationToken cancellationToken = default);

    Task RecordTechniqueCompletionAsync(string itemKey, string moduleName, string pageName, int durationSeconds = 0, CancellationToken cancellationToken = default);
    Task<long> CountTechniqueCompletionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CompletionDTO>> GetRecentTechniqueCompletionsAsync(int limit = 20, CancellationToken cancellationToken = default);
    Task<int> GetStreakDaysAsync(CancellationToken cancellationToken = default);
    Task<DateTime?> GetLastPracticeDateAsync(string itemKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<string, DateTime>> GetLastPracticeDatesAsync(IReadOnlyList<string> itemKeys, CancellationToken cancellationToken = default);

    Task SaveSessionDraftAsync(string techniqueKey, string payloadJson, CancellationToken cancellationToken = default);
    Task<string?> GetSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default);
    Task<IReadOnlySet<string>> GetSessionDraftKeysAsync(IReadOnlyList<string> techniqueKeys, CancellationToken cancellationToken = default);
    Task DeleteSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default);

    Task RecordMoodAsync(int moodLevel, string? note = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MoodEntryDTO>> GetRecentMoodsAsync(int limit = 7, CancellationToken cancellationToken = default);
}
