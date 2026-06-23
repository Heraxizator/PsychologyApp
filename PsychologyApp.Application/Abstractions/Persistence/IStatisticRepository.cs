using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IStatisticRepository : IReadRepository<global::PsychologyApp.Domain.Entities.Statistic>, IWriteRepository<global::PsychologyApp.Domain.Entities.Statistic>
{
    Task<long> CountDistinctPagesAsync(CancellationToken cancellationToken = default);
    Task<long> CountByPageNameAsync(string pageName, CancellationToken cancellationToken = default);
    Task<IEnumerable<global::PsychologyApp.Domain.Entities.Statistic>> GetRecentAsync(int limit, CancellationToken cancellationToken = default);
}
