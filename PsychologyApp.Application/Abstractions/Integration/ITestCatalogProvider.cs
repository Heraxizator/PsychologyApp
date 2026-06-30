namespace PsychologyApp.Application.Abstractions.Integration;

using PsychologyApp.Application.Models.Tests;

public interface ITestCatalogProvider
{
    Task<IReadOnlyList<TestDefinition>> LoadAllAsync(CancellationToken cancellationToken = default);
}
