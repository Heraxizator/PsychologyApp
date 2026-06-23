using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IQuotRepository : IReadRepository<global::PsychologyApp.Domain.Entities.Quot>, IWriteRepository<global::PsychologyApp.Domain.Entities.Quot>
{
    Task<IEnumerable<global::PsychologyApp.Domain.Entities.Quot>> GetUnreadLatestAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<global::PsychologyApp.Domain.Entities.Quot>> GetLatestAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<global::PsychologyApp.Domain.Entities.Quot>> GetFavouritesAsync(int count, CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);
}
