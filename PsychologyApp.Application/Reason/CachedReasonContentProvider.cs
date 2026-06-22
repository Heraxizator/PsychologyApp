using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Services.ReasonService;

public sealed class CachedReasonContentProvider(
    IReasonContentProvider innerProvider,
    Func<string>? languageKeyProvider = null) : IReasonContentProvider
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IReadOnlyList<Reason>? _cache;
    private string? _loadedLanguageKey;

    public async Task<IEnumerable<Reason>> LoadReasonsAsync(CancellationToken cancellationToken = default) =>
        await EnsureCacheAsync(cancellationToken);

    public async Task<IReadOnlyList<Reason>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Reason> cache = await EnsureCacheAsync(cancellationToken);
        return cache
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public void Invalidate()
    {
        _cache = null;
        _loadedLanguageKey = null;
    }

    private async Task<IReadOnlyList<Reason>> EnsureCacheAsync(CancellationToken cancellationToken)
    {
        string? languageKey = languageKeyProvider?.Invoke();
        if (_cache is not null
            && languageKey is not null
            && !string.Equals(_loadedLanguageKey, languageKey, StringComparison.OrdinalIgnoreCase))
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
                _cache = (await innerProvider.LoadReasonsAsync(cancellationToken)).ToList();
                _loadedLanguageKey = languageKey;
            }

            return _cache;
        }
        finally
        {
            _gate.Release();
        }
    }
}
