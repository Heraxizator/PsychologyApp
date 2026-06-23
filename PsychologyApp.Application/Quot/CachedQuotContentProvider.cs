using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Common;

namespace PsychologyApp.Application.Quot;

public sealed class CachedQuotContentProvider(IQuotContentProvider innerProvider) : IQuotContentProvider
{
    private readonly ContentLoadCache<QuotSeed> _cache = new();

    public async Task<IReadOnlyList<QuotSeed>> LoadAllAsync(CancellationToken cancellationToken = default) =>
        await _cache.GetOrLoadAsync(async ct =>
        {
            IReadOnlyList<QuotSeed> loaded = (await innerProvider.LoadAllAsync(ct)).ToList();
            if (loaded.Count == 0)
            {
                throw new InvalidOperationException("Embedded quote catalog is empty.");
            }

            return loaded;
        }, cancellationToken: cancellationToken);

    public void Invalidate() => _cache.Invalidate();
}
