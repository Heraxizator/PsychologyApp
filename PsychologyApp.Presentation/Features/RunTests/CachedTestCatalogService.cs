using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;

namespace PsychologyApp.Presentation.Features.RunTests;

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
            IReadOnlyList<TestDefinition> loaded = await inner.LoadCatalogAsync(cancellationToken).ConfigureAwait(false);
            if (loaded.Count == 0)
            {
                throw new InvalidOperationException("Embedded test catalog is empty.");
            }

            _cache = loaded;
            _cacheKey = cacheKey;
            return _cache;
        }
        catch
        {
            _cache = null;
            _cacheKey = null;
            throw;
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
