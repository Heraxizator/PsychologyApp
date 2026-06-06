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
            if (_cache is null)
            {
                _cache = (await innerProvider.LoadAllAsync(cancellationToken)).ToList();
            }

            return _cache;
        }
        finally
        {
            _gate.Release();
        }
    }
}
