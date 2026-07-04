using PsychologyApp.Application.Abstractions.Integration;

namespace PsychologyApp.Application.Quot;

public interface IQuoteSearchService
{
    Task<IReadOnlyList<QuotSeed>> SearchCatalogAsync(
        string query,
        int maxResults = 50,
        CancellationToken cancellationToken = default);
}
