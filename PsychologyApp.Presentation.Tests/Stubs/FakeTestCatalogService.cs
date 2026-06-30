using Moq;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;

namespace PsychologyApp.Presentation.Tests;

internal sealed class FakeTestCatalogService : ITestCatalogService
{
    private IReadOnlyList<TestDefinition> _catalog = [];

    public int GetCatalogCallCount { get; private set; }
    public int GetByIdCallCount { get; private set; }

    public FakeTestCatalogService WithCatalog(params TestDefinition[] definitions)
    {
        _catalog = definitions;
        return this;
    }

    public Task<IReadOnlyList<TestDefinition>> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        GetCatalogCallCount++;
        return Task.FromResult(_catalog);
    }

    public Task<TestDefinition?> GetByIdAsync(string testId, CancellationToken cancellationToken = default)
    {
        GetByIdCallCount++;
        TestDefinition? match = _catalog.FirstOrDefault(item =>
            string.Equals(item.TestId, testId, StringComparison.Ordinal));
        return Task.FromResult(match);
    }

    public void Invalidate()
    {
    }
}
