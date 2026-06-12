using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IQuotRepository : IReadRepository<Quot>, IWriteRepository<Quot>
{
    Task<IEnumerable<Quot>> GetUnreadLatestAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<Quot>> GetLatestAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<Quot>> GetFavouritesAsync(int count, CancellationToken cancellationToken = default);
}
