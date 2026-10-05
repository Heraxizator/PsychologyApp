using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Abstractions.Persistence;

public interface IStatisticRepository : IReadRepository<global::PsychologyApp.Domain.Entities.Statistic>, IWriteRepository<global::PsychologyApp.Domain.Entities.Statistic>
{
    Task<long> CountDistinctPagesAsync(CancellationToken cancellationToken = default);
    Task<long> CountByPageNameAsync(string pageName, CancellationToken cancellationToken = default);
    Task<IEnumerable<global::PsychologyApp.Domain.Entities.Statistic>> GetRecentAsync(int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes visit rows older than the cut-off (UTC) except the latest row of each page, so the number of distinct pages seen is kept
    /// while the table stops growing by one row per screen view. Returns how many rows were removed.
    /// </summary>
    Task<int> PruneAsync(DateTime olderThanUtc, CancellationToken cancellationToken = default) => Task.FromResult(0);
}
