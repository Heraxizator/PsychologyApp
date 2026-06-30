using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Common;
using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Application.Practice.Catalog;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice;
public sealed class TechniqueCatalogService(ITechniqueCatalogProvider provider) : ITechniqueCatalogService
{
    private IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition>? _byId;

    public IReadOnlyList<BuiltInTechniqueDefinition> GetAll() => EnsureIndex().Values.ToList();

    public BuiltInTechniqueDefinition Get(TechniqueId techniqueId) => EnsureIndex()[techniqueId];

    public IReadOnlyList<TechniqueListEntry> GetBuiltInListEntries() =>
        Enum.GetValues<TechniqueId>()
            .Select(id =>
            {
                BuiltInTechniqueDefinition definition = Get(id);
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

    public void Invalidate()
    {
        _byId = null;
        if (provider is CachedTechniqueCatalogProvider cached)
        {
            cached.Invalidate();
        }
    }

    private IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> EnsureIndex()
    {
        if (_byId is not null)
        {
            return _byId;
        }

        IReadOnlyList<BuiltInTechniqueDefinition> loaded = provider.LoadAllAsync().GetAwaiter().GetResult();
        _byId = Enum.GetValues<TechniqueId>()
            .Zip(loaded)
            .ToDictionary(pair => pair.First, pair => pair.Second);
        return _byId;
    }
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
