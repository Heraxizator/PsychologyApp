using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Application.Practice.Catalog;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice;

public sealed class BuiltInTechniqueCatalogProvider(Func<string>? languageKeyProvider = null) : ITechniqueCatalogProvider
{
    public Task<IReadOnlyList<BuiltInTechniqueDefinition>> LoadAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string languageKey = languageKeyProvider?.Invoke() ?? "ru";
        IReadOnlyDictionary<TechniqueId, BuiltInTechniqueDefinition> definitions =
            string.Equals(languageKey, "en", StringComparison.OrdinalIgnoreCase)
                ? BuiltInTechniqueCatalogRegistry.English
                : BuiltInTechniqueCatalogRegistry.Russian;

        IReadOnlyList<BuiltInTechniqueDefinition> ordered = Enum.GetValues<TechniqueId>()
            .Select(id => definitions[id])
            .ToList();

        return Task.FromResult(ordered);
    }
}
