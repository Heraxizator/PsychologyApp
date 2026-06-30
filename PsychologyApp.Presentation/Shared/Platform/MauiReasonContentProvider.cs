using PsychologyApp.Application.Reason;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Domain.Entities;

namespace PsychologyApp.Presentation.Shared.Platform;

public sealed class MauiReasonContentProvider : IReasonContentProvider
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private string? _loadedAsset;
    private IReadOnlyList<Reason>? _cache;

    public MauiReasonContentProvider()
    {
        UserPreferences.Changed += InvalidateCache;
    }

    private void InvalidateCache()
    {
        _cache = null;
        _loadedAsset = null;
    }

    public async Task<IEnumerable<Reason>> LoadReasonsAsync(CancellationToken cancellationToken = default) =>
        await EnsureCacheAsync(cancellationToken);

    public async Task<IReadOnlyList<Reason>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Reason> cache = await EnsureCacheAsync(cancellationToken);
        return cache
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToList();
    }

    private async Task<IReadOnlyList<Reason>> EnsureCacheAsync(CancellationToken cancellationToken)
    {
        string assetPath = ContentAssets.PsychosomaticFile;
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

            List<string> lines = [];
            await using Stream fileStream = await FileSystem.OpenAppPackageFileAsync(assetPath);
            using StreamReader reader = new(fileStream);

            while (await reader.ReadLineAsync(cancellationToken) is { } line)
            {
                lines.Add(line);
            }

            List<Reason> reasons = ReasonTsvParser.ParseLines(lines).ToList();

            _cache = reasons;
            _loadedAsset = assetPath;
            return _cache;
        }
        finally
        {
            _gate.Release();
        }
    }
}

