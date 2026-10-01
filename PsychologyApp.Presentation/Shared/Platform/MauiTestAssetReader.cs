using PsychologyApp.Presentation.Shared.Abstractions;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.Platform;

public sealed class MauiTestAssetReader : ITestAssetReader
{
    // Android asset streams complete synchronously, so callers awaiting with ConfigureAwait(false) would
    // still parse on the UI thread; opening on the pool moves their continuations there.
    public Task<Stream> OpenAsync(string assetPath, CancellationToken cancellationToken = default) =>
        Task.Run(() => OpenCoreAsync(assetPath), cancellationToken);

    private static async Task<Stream> OpenCoreAsync(string assetPath)
    {
        string localizedPath = ContentAssets.Localized(assetPath);

        try
        {
            return await FileSystem.OpenAppPackageFileAsync(localizedPath);
        }
        catch when (!string.Equals(localizedPath, assetPath, StringComparison.Ordinal))
        {
            return await FileSystem.OpenAppPackageFileAsync(assetPath);
        }
    }
}
