using PsychologyApp.Application.Articles;

namespace PsychologyApp.Application.Abstractions.Persistence;

/// <summary>What the person has read and the courses they go through, kept in the encrypted database (course answers are their own words).</summary>
public interface IReadingRepository
{
    Task<ReadingState?> GetAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(ReadingState state, CancellationToken cancellationToken = default);
}
