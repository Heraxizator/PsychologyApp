using PsychologyApp.Application.Abstractions.Integration;

namespace PsychologyApp.Application.Quot;

public sealed class QuoteCatalogLookup(IQuotContentProvider quotContentProvider) : IQuoteCatalogLookup
{
    public async Task<int?> TryGetIndexByTextAsync(string text, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<QuotSeed> seeds = await quotContentProvider.LoadAllAsync(cancellationToken);
        for (int index = 0; index < seeds.Count; index++)
        {
            if (string.Equals(seeds[index].Text, text, StringComparison.Ordinal))
            {
                return index;
            }
        }

        return null;
    }

    public async Task<QuotSeed?> GetSeedAtAsync(int catalogIndex, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<QuotSeed> seeds = await quotContentProvider.LoadAllAsync(cancellationToken);
        return catalogIndex >= 0 && catalogIndex < seeds.Count ? seeds[catalogIndex] : null;
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<QuotSeed> seeds = await quotContentProvider.LoadAllAsync(cancellationToken);
        return seeds.Count;
    }
}
