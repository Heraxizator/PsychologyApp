using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IStatisticRepository : IReadRepository<Statistic>, IWriteRepository<Statistic>
{
    Task<long> CountDistinctPagesAsync(CancellationToken cancellationToken = default);
    Task<long> CountByPageNameAsync(string pageName, CancellationToken cancellationToken = default);
    Task<IEnumerable<Statistic>> GetRecentAsync(int limit, CancellationToken cancellationToken = default);
}
