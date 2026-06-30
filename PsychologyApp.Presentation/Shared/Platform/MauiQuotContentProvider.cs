using System.Text.Json;
using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Models.Quotes;
using PsychologyApp.Presentation.Serialization;

namespace PsychologyApp.Presentation.Shared.Platform;

public sealed class MauiQuotContentProvider : IQuotContentProvider
{
    private readonly ILogger<MauiQuotContentProvider> _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private string? _loadedAsset;
    private IReadOnlyList<QuotSeed>? _cache;

    public MauiQuotContentProvider(ILogger<MauiQuotContentProvider> logger)
    {
        _logger = logger;
        UserPreferences.Changed += InvalidateCache;
    }

    public async Task<IReadOnlyList<QuotSeed>> LoadAllAsync(CancellationToken cancellationToken = default) =>
        await EnsureCacheAsync(cancellationToken);

    private void InvalidateCache()
    {
        _cache = null;
        _loadedAsset = null;
    }

    private async Task<IReadOnlyList<QuotSeed>> EnsureCacheAsync(CancellationToken cancellationToken)
    {
        string assetPath = ContentAssets.QuotesFile;
        if (_cache is not null && _loadedAsset == assetPath)
        {
            return _cache;
        }

        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (_cache is not null && _loadedAsset == assetPath)
            {
                return _cache;
            }

            await using Stream stream = await FileSystem.OpenAppPackageFileAsync(assetPath);
            List<QuoteJsonEntry>? entries = await JsonSerializer.DeserializeAsync(
                stream,
                AppJsonSerializerContext.Default.ListQuoteJsonEntry,
                cancellationToken);

            if (entries is null || entries.Count == 0)
            {
                _logger.LogWarning("Quote asset {AssetPath} is empty or failed to deserialize.", assetPath);
                throw new InvalidOperationException($"Embedded quote catalog is empty: {assetPath}.");
            }

            IReadOnlyList<QuotSeed> seeds = entries
                .Where(entry => !string.IsNullOrWhiteSpace(entry.Text))
                .Select(entry => new QuotSeed(
                    entry.Author ?? string.Empty,
                    entry.Text.Trim(),
                    string.IsNullOrWhiteSpace(entry.Theme) ? "general" : entry.Theme.Trim()))
                .ToList();

            if (seeds.Count == 0)
            {
                _logger.LogWarning("Quote asset {AssetPath} contains no valid entries.", assetPath);
                throw new InvalidOperationException($"Embedded quote catalog has no valid entries: {assetPath}.");
            }

            _cache = seeds;
            _loadedAsset = assetPath;
            return _cache;
        }
        finally
        {
            _gate.Release();
        }
    }
}
