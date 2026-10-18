using PsychologyApp.Application.Body;

namespace PsychologyApp.Application.Abstractions.Persistence;

/// <summary>Body check-ins, kept in the encrypted database (the notes are the person's own words).</summary>
public interface IBodyRepository
{
    Task<BodyLog?> GetAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(BodyLog log, CancellationToken cancellationToken = default);
}
