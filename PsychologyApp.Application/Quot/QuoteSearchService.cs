using PsychologyApp.Application.Abstractions.Integration;

namespace PsychologyApp.Application.Quot;

public sealed class QuoteSearchService(IQuotContentProvider quotContentProvider) : IQuoteSearchService
{
    public async Task<IReadOnlyList<QuotSeed>> SearchCatalogAsync(
        string query,
        int maxResults = 50,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return [];
        }

        string normalizedQuery = query.Trim();
        IReadOnlyList<QuotSeed> seeds = await quotContentProvider.LoadAllAsync(cancellationToken);
        List<QuotSeed> results = [];

        foreach (QuotSeed seed in seeds)
        {
            if (results.Count >= maxResults)
            {
                break;
            }

            if (ContainsIgnoreCase(seed.Text, normalizedQuery) ||
                ContainsIgnoreCase(seed.Author, normalizedQuery) ||
                ContainsIgnoreCase(seed.Theme, normalizedQuery))
            {
                results.Add(seed);
            }
        }

        return results;
    }

    private static bool ContainsIgnoreCase(string? value, string query) =>
        !string.IsNullOrEmpty(value) &&
        value.Contains(query, StringComparison.OrdinalIgnoreCase);
}
