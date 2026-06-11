using System.Collections.Concurrent;
using PsychologyApp.Presentation.Abstractions;

namespace PsychologyApp.Testing.Tests;

public sealed class FakeTestAssetReader : ITestAssetReader
{
    private readonly ConcurrentDictionary<string, byte[]> _assets = new(StringComparer.Ordinal);

    public FakeTestAssetReader Register(string assetPath, string json) =>
        Register(assetPath, System.Text.Encoding.UTF8.GetBytes(json));

    public FakeTestAssetReader Register(string assetPath, byte[] content)
    {
        _assets[assetPath] = content;
        return this;
    }

    public Task<Stream> OpenAsync(string assetPath, CancellationToken cancellationToken = default)
    {
        if (!_assets.TryGetValue(assetPath, out byte[]? content))
        {
            throw new FileNotFoundException($"Test asset not registered: {assetPath}", assetPath);
        }

        return Task.FromResult<Stream>(new MemoryStream(content, writable: false));
    }
}
