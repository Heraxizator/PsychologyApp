using PsychologyApp.Presentation.Models.Tests;

namespace PsychologyApp.Presentation.Services.Tests;

public interface ITestCatalogService
{
    Task<IReadOnlyList<TestDefinition>> GetCatalogAsync(CancellationToken cancellationToken = default);

    Task<TestDefinition?> GetByIdAsync(string testId, CancellationToken cancellationToken = default);

    void Invalidate();
}
