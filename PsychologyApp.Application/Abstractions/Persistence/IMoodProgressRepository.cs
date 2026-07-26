using PsychologyApp.Application.Models;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IMoodProgressRepository
{
    Task RecordMoodAsync(MoodEntryDTO entry, CancellationToken cancellationToken = default);
    Task UpdateMoodEntryAsync(long moodEntryId, int moodLevel, string? note, CancellationToken cancellationToken = default);
    Task DeleteMoodEntryAsync(long moodEntryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MoodEntryDTO>> GetRecentMoodsAsync(int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MoodEntryDTO>> GetMoodsAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        int limit,
        CancellationToken cancellationToken = default);
}
