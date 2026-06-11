using System.Text.Json;
using PsychologyApp.Application.Abstractions.Integration;

namespace PsychologyApp.Presentation.Platform;

public sealed class MauiQuotContentProvider : IQuotContentProvider
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly SemaphoreSlim _gate = new(1, 1);
    private string? _loadedAsset;
    private IReadOnlyList<QuotSeed>? _cache;

    public MauiQuotContentProvider()
    {
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
            List<JsonQuotEntry>? entries = await JsonSerializer.DeserializeAsync<List<JsonQuotEntry>>(
                stream,
                SerializerOptions,
                cancellationToken);

            if (entries is null || entries.Count == 0)
            {
                _cache = [];
                _loadedAsset = assetPath;
                return _cache;
            }

            _cache = entries
                .Where(entry => !string.IsNullOrWhiteSpace(entry.Text))
                .Select(entry => new QuotSeed(
                    entry.Author ?? string.Empty,
                    entry.Text.Trim(),
                    string.IsNullOrWhiteSpace(entry.Theme) ? "general" : entry.Theme.Trim()))
                .ToList();

            _loadedAsset = assetPath;
            return _cache;
        }
        finally
        {
            _gate.Release();
        }
    }

    private sealed class JsonQuotEntry
    {
        public string? Author { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? Theme { get; set; }
    }
}

