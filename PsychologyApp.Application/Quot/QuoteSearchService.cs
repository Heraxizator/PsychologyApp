using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Common;

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

        IReadOnlyList<string> stems = SearchText.Stems(query);
        if (stems.Count == 0)
        {
            return [];
        }

        IReadOnlyList<QuotSeed> seeds = await quotContentProvider.LoadAllAsync(cancellationToken).ConfigureAwait(false);
        List<QuotSeed> results = [];

        foreach (QuotSeed seed in seeds)
        {
            if (results.Count >= maxResults)
            {
                break;
            }

            if (SearchText.Matches(seed.Text, stems) ||
                SearchText.Matches(seed.Author, stems) ||
                SearchText.Matches(seed.Theme, stems))
            {
                results.Add(seed);
            }
        }

        return results;
    }

}