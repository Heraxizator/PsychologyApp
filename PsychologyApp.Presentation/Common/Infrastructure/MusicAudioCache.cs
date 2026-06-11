using System.Security.Cryptography;
using System.Text;

namespace PsychologyApp.Presentation.Common;

public readonly record struct AudioCacheResult(string Uri, bool UsedNetwork, bool DownloadFailed = false);

public static class MusicAudioCache
{
    public static bool IsCached(string remoteUrl) =>
        !string.IsNullOrWhiteSpace(remoteUrl) && File.Exists(GetCachePath(remoteUrl));

    public static async Task<AudioCacheResult> ResolvePlaybackUriAsync(
        string remoteUrl,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(remoteUrl))
        {
            return new AudioCacheResult(remoteUrl, UsedNetwork: false);
        }

        string cachePath = GetCachePath(remoteUrl);
        if (File.Exists(cachePath))
        {
            return new AudioCacheResult(cachePath, UsedNetwork: false);
        }

        try
        {
            using HttpClient client = new();
            byte[] bytes = await client.GetByteArrayAsync(remoteUrl, cancellationToken);
            string? directory = Path.GetDirectoryName(cachePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllBytesAsync(cachePath, bytes, cancellationToken);
            return new AudioCacheResult(cachePath, UsedNetwork: true);
        }
        catch
        {
            return new AudioCacheResult(remoteUrl, UsedNetwork: true, DownloadFailed: true);
        }
    }

    public static async Task PrefetchAsync(IEnumerable<string> remoteUrls, CancellationToken cancellationToken = default)
    {
        foreach (string url in remoteUrls)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (IsCached(url))
            {
                continue;
            }

            await ResolvePlaybackUriAsync(url, cancellationToken);
        }
    }

    private static string GetCachePath(string remoteUrl)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(remoteUrl));
        string fileName = Convert.ToHexString(hash).ToLowerInvariant() + ".mp3";
        return Path.Combine(GetCacheDirectory(), fileName);
    }

    private static string GetCacheDirectory()
    {
        try
        {
            return Path.Combine(FileSystem.CacheDirectory, "music");
        }
        catch
        {
            return Path.Combine(Path.GetTempPath(), "PsychologyApp", "music");
        }
    }
}
