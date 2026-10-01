using System.Collections.Concurrent;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace PsychologyApp.Presentation.Shared.Common;

public readonly record struct AudioCacheResult(string Uri, bool UsedNetwork, bool DownloadFailed = false);

public static class MusicAudioCache
{
    private const int MaxDownloadBytes = 50 * 1024 * 1024;
    private static readonly TimeSpan HttpTimeout = TimeSpan.FromSeconds(30);
    private static readonly HttpClient HttpClient = CreateHttpClient();

    // The playlist asks IsCached for every track on the UI thread: the path (a SHA-256 of the URL plus a JNI call
    // for the cache directory) is computed once per URL, and a track once found on disk is not stat-ed again.
    // Playback still checks the file itself, so a cache the OS cleared is simply downloaded again.
    private static readonly ConcurrentDictionary<string, string> CachePaths = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, byte> KnownCached = new(StringComparer.Ordinal);
    private static string? _cacheDirectory;

    public static bool IsCached(string remoteUrl)
    {
        if (string.IsNullOrWhiteSpace(remoteUrl))
        {
            return false;
        }

        if (KnownCached.ContainsKey(remoteUrl))
        {
            return true;
        }

        if (!File.Exists(GetCachePath(remoteUrl)))
        {
            return false;
        }

        KnownCached.TryAdd(remoteUrl, 0);
        return true;
    }

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

        KnownCached.TryRemove(remoteUrl, out _);

        string partialPath = cachePath + ".part";
        try
        {
            using HttpResponseMessage response = await HttpClient.GetAsync(
                remoteUrl,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentLength is > MaxDownloadBytes)
            {
                return new AudioCacheResult(remoteUrl, UsedNetwork: true, DownloadFailed: true);
            }

            string? directory = Path.GetDirectoryName(cachePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Streamed to a temp file rather than buffered whole: a track is several MB, and one large array per
            // track meant large-object allocations and GC pauses. The rename keeps a partial file from ever
            // counting as cached.
            await using (Stream body = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
            await using (FileStream file = File.Create(partialPath))
            {
                byte[] buffer = new byte[81920];
                long total = 0;
                int read;
                while ((read = await body.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
                {
                    total += read;
                    if (total > MaxDownloadBytes)
                    {
                        throw new InvalidDataException("Audio file too large.");
                    }

                    await file.WriteAsync(buffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
                }
            }

            File.Move(partialPath, cachePath, overwrite: true);
            KnownCached.TryAdd(remoteUrl, 0);
            return new AudioCacheResult(cachePath, UsedNetwork: true);
        }
        catch
        {
            TryDelete(partialPath);
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

            await ResolvePlaybackUriAsync(url, cancellationToken).ConfigureAwait(false);
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch
        {
        }
    }

    internal static HttpClient SharedHttpClient => HttpClient;

    private static HttpClient CreateHttpClient() =>
        new(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(5) })
        {
            Timeout = HttpTimeout
        };

    private static string GetCachePath(string remoteUrl) =>
        CachePaths.GetOrAdd(remoteUrl, ComputeCachePath);

    private static string ComputeCachePath(string remoteUrl)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(remoteUrl));
        string fileName = Convert.ToHexString(hash).ToLowerInvariant() + ".mp3";
        return Path.Combine(GetCacheDirectory(), fileName);
    }

    private static string GetCacheDirectory() => _cacheDirectory ??= ResolveCacheDirectory();

    private static string ResolveCacheDirectory()
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
