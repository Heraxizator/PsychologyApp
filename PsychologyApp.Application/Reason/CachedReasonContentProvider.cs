using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Common;
using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Application.Reason;

public sealed class CachedReasonContentProvider(
    IReasonContentProvider innerProvider,
    Func<string>? languageKeyProvider = null) : IReasonContentProvider
{
    private readonly ContentLoadCache<global::PsychologyApp.Domain.Entities.Reason> _cache = new();

    public async Task<IEnumerable<global::PsychologyApp.Domain.Entities.Reason>> LoadReasonsAsync(CancellationToken cancellationToken = default) =>
        await EnsureCacheAsync(cancellationToken);

    public async Task<IReadOnlyList<global::PsychologyApp.Domain.Entities.Reason>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<global::PsychologyApp.Domain.Entities.Reason> cache = await EnsureCacheAsync(cancellationToken);
        return cache
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public void Invalidate() => _cache.Invalidate();

    private Task<IReadOnlyList<global::PsychologyApp.Domain.Entities.Reason>> EnsureCacheAsync(CancellationToken cancellationToken) =>
        _cache.GetOrLoadAsync(
            async ct => (await innerProvider.LoadReasonsAsync(ct)).ToList(),
            languageKeyProvider,
            cancellationToken);
}
