using PsychologyApp.Application.Abstractions.Integration;

namespace PsychologyApp.Application.Quot;

public interface IQuoteCatalogLookup
{
    Task<int?> TryGetIndexByTextAsync(string text, CancellationToken cancellationToken = default);

    Task<QuotSeed?> GetSeedAtAsync(int catalogIndex, CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}
