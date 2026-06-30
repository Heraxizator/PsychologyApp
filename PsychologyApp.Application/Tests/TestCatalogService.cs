using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Common;
using PsychologyApp.Application.Models.Tests;

namespace PsychologyApp.Application.Tests;

public interface ITestCatalogService
{
    Task<IReadOnlyList<TestDefinition>> GetCatalogAsync(CancellationToken cancellationToken = default);

    Task<TestDefinition?> GetByIdAsync(string testId, CancellationToken cancellationToken = default);

    void Invalidate();
}

public sealed class TestCatalogService(ITestCatalogProvider provider) : ITestCatalogService
{
    public Task<IReadOnlyList<TestDefinition>> GetCatalogAsync(CancellationToken cancellationToken = default) =>
        provider.LoadAllAsync(cancellationToken);

    public async Task<TestDefinition?> GetByIdAsync(string testId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(testId))
        {
            return null;
        }

        IReadOnlyList<TestDefinition> catalog = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return catalog.FirstOrDefault(test => string.Equals(test.TestId, testId, StringComparison.Ordinal));
    }

    public void Invalidate()
    {
        if (provider is CachedTestCatalogProvider cached)
        {
            cached.Invalidate();
        }
    }
}

public sealed class CachedTestCatalogProvider(
    ITestCatalogProvider innerProvider,
    Func<string>? languageKeyProvider = null) : ITestCatalogProvider
{
    private readonly ContentLoadCache<TestDefinition> _cache = new();

    public async Task<IReadOnlyList<TestDefinition>> LoadAllAsync(CancellationToken cancellationToken = default) =>
        await _cache.GetOrLoadAsync(
            async ct =>
            {
                IReadOnlyList<TestDefinition> loaded = (await innerProvider.LoadAllAsync(ct)).ToList();
                if (loaded.Count == 0)
                {
                    throw new InvalidOperationException("Embedded test catalog is empty.");
                }

                return loaded;
            },
            languageKeyProvider,
            cancellationToken);

    public void Invalidate() => _cache.Invalidate();
}
