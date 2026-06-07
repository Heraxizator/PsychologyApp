using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IUserProgressRepository
{
    Task SaveTestResultAsync(TestResultDTO result, CancellationToken cancellationToken = default);
    Task<TestResultDTO?> GetLatestTestResultAsync(string testId, CancellationToken cancellationToken = default);
    Task<long> CountTestResultsAsync(CancellationToken cancellationToken = default);

    Task RecordCompletionAsync(CompletionDTO completion, CancellationToken cancellationToken = default);
    Task<long> CountTechniqueCompletionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DateOnly>> GetCompletionDatesAsync(CancellationToken cancellationToken = default);
    Task<DateTime?> GetLastCompletionForItemAsync(string itemKey, CancellationToken cancellationToken = default);

    Task SaveSessionDraftAsync(string techniqueKey, string payloadJson, CancellationToken cancellationToken = default);
    Task<string?> GetSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default);
    Task DeleteSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default);

    Task RecordMoodAsync(MoodEntryDTO entry, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MoodEntryDTO>> GetRecentMoodsAsync(int limit, CancellationToken cancellationToken = default);
}
