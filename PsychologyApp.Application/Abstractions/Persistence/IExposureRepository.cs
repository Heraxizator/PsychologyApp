using PsychologyApp.Domain.Exposure;

namespace PsychologyApp.Application.Abstractions.Persistence;

/// <summary>The person's fear ladder, kept in the encrypted database with the rest of their records.</summary>
public interface IExposureRepository
{
    Task<ExposurePlan?> GetAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(ExposurePlan plan, CancellationToken cancellationToken = default);

    Task DeleteAsync(CancellationToken cancellationToken = default);
}
