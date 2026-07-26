using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IPracticeProgressRepository
{
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

    Task UpdateSessionResultPostIntensityAsync(long sessionResultId, int postIntensity, CancellationToken cancellationToken = default);
    Task<SessionResultDTO?> GetSessionResultAsync(long sessionResultId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SessionResultDTO>> GetRecentSessionResultsAsync(int limit, CancellationToken cancellationToken = default);
    Task<int> CountDistinctTechniqueCompletionsForItemsBetweenAsync(
        IReadOnlyList<string> itemKeys,
        DateTime sinceUtc,
        DateTime beforeUtc,
        CancellationToken cancellationToken = default);
}
