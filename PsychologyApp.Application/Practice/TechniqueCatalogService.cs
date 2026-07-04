using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Common;
using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Application.Practice.Catalog;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice;

public sealed class TechniqueCatalogService(ITechniqueCatalogProvider provider) : ITechniqueCatalogService
{
    private readonly SemaphoreSlim _indexGate = new(1, 1);
    private IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition>? _byId;

    public IReadOnlyList<BuiltInTechniqueDefinition> GetAll() =>
        EnsureIndex().Values.ToList();

    public BuiltInTechniqueDefinition Get(TechniqueId techniqueId) =>
        EnsureIndex()[techniqueId];

    public IReadOnlyList<TechniqueListEntry> GetBuiltInListEntries() =>
        BuildListEntries(EnsureIndex());

    public async Task<IReadOnlyList<BuiltInTechniqueDefinition>> GetAllAsync(CancellationToken cancellationToken = default) =>
        (await EnsureIndexAsync(cancellationToken).ConfigureAwait(false)).Values.ToList();

    public async Task<BuiltInTechniqueDefinition> GetAsync(
        TechniqueId techniqueId,
        CancellationToken cancellationToken = default) =>
        (await EnsureIndexAsync(cancellationToken).ConfigureAwait(false))[techniqueId];

    public async Task<IReadOnlyList<TechniqueListEntry>> GetBuiltInListEntriesAsync(
        CancellationToken cancellationToken = default) =>
        BuildListEntries(await EnsureIndexAsync(cancellationToken).ConfigureAwait(false));

    public void Invalidate()
    {
        _byId = null;
        if (provider is CachedTechniqueCatalogProvider cached)
        {
            cached.Invalidate();
        }
    }

    private IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> EnsureIndex() =>
        EnsureIndexAsync(CancellationToken.None).GetAwaiter().GetResult();

    private async Task<IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition>> EnsureIndexAsync(
        CancellationToken cancellationToken)
    {
        if (_byId is not null)
        {
            return _byId;
        }

        await _indexGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_byId is not null)
            {
                return _byId;
            }

            IReadOnlyList<BuiltInTechniqueDefinition> loaded =
                await provider.LoadAllAsync(cancellationToken).ConfigureAwait(false);
            _byId = Enum.GetValues<TechniqueId>()
                .Zip(loaded)
                .ToDictionary(pair => pair.First, pair => pair.Second);
            return _byId;
        }
        finally
        {
            _indexGate.Release();
        }
    }

    private static IReadOnlyList<TechniqueListEntry> BuildListEntries(
        IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> byId) =>
        Enum.GetValues<TechniqueId>()
            .Select(id =>
            {
                BuiltInTechniqueDefinition definition = byId[id];
                return new TechniqueListEntry(
                    id,
                    definition.ListNumber,
                    definition.ListDate,
                    definition.ListTitle,
                    definition.ListSubtitle,
                    definition.Theme,
                    definition.Author,
                    definition.ListDurationMinutes,
                    definition.ListIcon);
            })
            .ToList();
}

public sealed class CachedTechniqueCatalogProvider(
    ITechniqueCatalogProvider innerProvider,
    Func<string>? languageKeyProvider = null) : ITechniqueCatalogProvider
{
    private readonly ContentLoadCache<BuiltInTechniqueDefinition> _cache = new();

    public async Task<IReadOnlyList<BuiltInTechniqueDefinition>> LoadAllAsync(CancellationToken cancellationToken = default) =>
        await _cache.GetOrLoadAsync(
            async ct => (await innerProvider.LoadAllAsync(ct)).ToList(),
            languageKeyProvider,
            cancellationToken);

    public void Invalidate() => _cache.Invalidate();
}
