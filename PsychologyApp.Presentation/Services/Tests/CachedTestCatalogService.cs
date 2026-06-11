using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Tests;

namespace PsychologyApp.Presentation.Services.Tests;

public sealed class CachedTestCatalogService(TestCatalogService inner, ILogger<CachedTestCatalogService> logger) : ITestCatalogService
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IReadOnlyList<TestDefinition>? _cache;
    private string? _cacheKey;

    public void Invalidate()
    {
        _cache = null;
        _cacheKey = null;
    }

    public async Task<IReadOnlyList<TestDefinition>> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        string cacheKey = GetCacheKey();

        if (_cache is not null && string.Equals(_cacheKey, cacheKey, StringComparison.Ordinal))
        {
            return _cache;
        }

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_cache is not null && string.Equals(_cacheKey, cacheKey, StringComparison.Ordinal))
            {
                return _cache;
            }

            logger.LogDebug("Loading test catalog for language key {CacheKey}", cacheKey);
            _cache = await inner.LoadCatalogAsync(cancellationToken).ConfigureAwait(false);
            _cacheKey = cacheKey;
            return _cache;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<TestDefinition?> GetByIdAsync(string testId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(testId))
        {
            return null;
        }

        IReadOnlyList<TestDefinition> catalog = await GetCatalogAsync(cancellationToken).ConfigureAwait(false);
        return catalog.FirstOrDefault(test => string.Equals(test.TestId, testId, StringComparison.Ordinal));
    }

    private static string GetCacheKey() =>
        AppStrings.IsEnglish(AppStrings.Language) ? "en" : "ru";
}
