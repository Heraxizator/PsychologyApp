using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Services.ReasonService;

public sealed class CachedReasonContentProvider(IReasonContentProvider innerProvider) : IReasonContentProvider
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IReadOnlyList<Reason>? _cache;

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

    private async Task<IReadOnlyList<Reason>> EnsureCacheAsync(CancellationToken cancellationToken)
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
                _cache = (await innerProvider.LoadReasonsAsync(cancellationToken)).ToList();
            }

            return _cache;
        }
        finally
        {
            _gate.Release();
        }
    }
}
