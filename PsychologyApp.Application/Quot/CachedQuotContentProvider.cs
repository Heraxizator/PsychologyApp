using PsychologyApp.Application.Abstractions.Integration;

namespace PsychologyApp.Application.Services.QuotService;

public sealed class CachedQuotContentProvider(IQuotContentProvider innerProvider) : IQuotContentProvider
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IReadOnlyList<QuotSeed>? _cache;

    public async Task<IReadOnlyList<QuotSeed>> LoadAllAsync(CancellationToken cancellationToken = default) =>
        await EnsureCacheAsync(cancellationToken);

    public void Invalidate() => _cache = null;

    private async Task<IReadOnlyList<QuotSeed>> EnsureCacheAsync(CancellationToken cancellationToken)
    {
        if (_cache is not null)
        {
            return _cache;
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (_cache is not null)
            {
                return _cache;
            }

            IReadOnlyList<QuotSeed> loaded = (await innerProvider.LoadAllAsync(cancellationToken)).ToList();
            if (loaded.Count == 0)
            {
                throw new InvalidOperationException("Embedded quote catalog is empty.");
            }

            _cache = loaded;
            return _cache;
        }
        catch
        {
            _cache = null;
            throw;
        }
        finally
        {
            _gate.Release();
        }
    }
}
