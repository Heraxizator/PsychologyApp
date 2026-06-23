using PsychologyApp.Presentation.Shared.Abstractions;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.Platform;

public sealed class MauiTestAssetReader : ITestAssetReader
{
    public async Task<Stream> OpenAsync(string assetPath, CancellationToken cancellationToken = default)
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
