using PsychologyApp.Application.Models.Practice;

namespace PsychologyApp.Application.Abstractions.Integration;

public interface ITechniqueCatalogProvider
{
    Task<IReadOnlyList<BuiltInTechniqueDefinition>> LoadAllAsync(CancellationToken cancellationToken = default);
}
