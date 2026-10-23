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
        IReadOnlyList<string> candidates = ContentAssets.Candidates(assetPath);
        for (int i = 0; i < candidates.Count - 1; i++)
        {
            try
            {
                return await FileSystem.OpenAppPackageFileAsync(candidates[i]);
            }
            catch (Exception missing) when (missing is not OperationCanceledException)
            {
                // This language has no file of its own yet (on Android a missing asset is a Java exception, not
                // FileNotFoundException): the next one in line is shown instead.
            }
        }

        return await FileSystem.OpenAppPackageFileAsync(candidates[^1]);
    }
}
