using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Features.RunTests;

public interface ITestCatalogService
{
    Task<IReadOnlyList<TestDefinition>> GetCatalogAsync(CancellationToken cancellationToken = default);

    Task<TestDefinition?> GetByIdAsync(string testId, CancellationToken cancellationToken = default);

    void Invalidate();
}
