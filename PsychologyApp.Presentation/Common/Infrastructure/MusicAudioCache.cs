using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace PsychologyApp.Presentation.Common;

public readonly record struct AudioCacheResult(string Uri, bool UsedNetwork, bool DownloadFailed = false);

public static class MusicAudioCache
{
    private const int MaxDownloadBytes = 50 * 1024 * 1024;
    private static readonly TimeSpan HttpTimeout = TimeSpan.FromSeconds(30);
    private static readonly HttpClient HttpClient = CreateHttpClient();

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
            using HttpResponseMessage response = await HttpClient.GetAsync(
                remoteUrl,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentLength is > MaxDownloadBytes)
            {
                return new AudioCacheResult(remoteUrl, UsedNetwork: true, DownloadFailed: true);
            }

            byte[] bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            if (bytes.Length > MaxDownloadBytes)
            {
                return new AudioCacheResult(remoteUrl, UsedNetwork: true, DownloadFailed: true);
            }

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

    internal static HttpClient SharedHttpClient => HttpClient;

    private static HttpClient CreateHttpClient() =>
        new(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(5) })
        {
            Timeout = HttpTimeout
        };

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
