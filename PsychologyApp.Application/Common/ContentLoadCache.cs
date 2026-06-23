namespace PsychologyApp.Application.Common;

internal sealed class ContentLoadCache<T>
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IReadOnlyList<T>? _cache;
    private string? _scopeKey;

    public async Task<IReadOnlyList<T>> GetOrLoadAsync(
        Func<CancellationToken, Task<IReadOnlyList<T>>> loader,
        Func<string?>? scopeKeyProvider = null,
        CancellationToken cancellationToken = default)
    {
        string? scopeKey = scopeKeyProvider?.Invoke();
        if (_cache is not null
            && scopeKey is not null
            && !string.Equals(_scopeKey, scopeKey, StringComparison.OrdinalIgnoreCase))
        {
            _cache = null;
        }

        if (_cache is not null)
        {
            return _cache;
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (_cache is null)
            {
                _cache = await loader(cancellationToken);
                _scopeKey = scopeKey;
            }

            return _cache;
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Invalidate()
    {
        _cache = null;
        _scopeKey = null;
    }
}
