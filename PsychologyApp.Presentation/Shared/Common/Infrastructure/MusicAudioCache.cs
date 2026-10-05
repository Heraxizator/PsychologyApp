using System.Collections.Concurrent;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace PsychologyApp.Presentation.Shared.Common;

public readonly record struct AudioCacheResult(string Uri, bool UsedNetwork, bool DownloadFailed = false);

public static class MusicAudioCache
{
    private static readonly TimeSpan HttpTimeout = TimeSpan.FromSeconds(30);

    // A whole track, headers and body, may not take longer than this however slowly the server trickles it.
    private static readonly TimeSpan DownloadDeadline = TimeSpan.FromMinutes(3);

    private static readonly HttpClient HttpClient = CreateHttpClient();

    // The playlist asks IsCached for every track on the UI thread: the path (a SHA-256 of the URL plus a JNI call
    // for the cache directory) is computed once per URL, and a track once found on disk is not stat-ed again.
    // Playback still checks the file itself, so a cache the OS cleared is simply downloaded again.
    private static readonly ConcurrentDictionary<string, string> CachePaths = new(StringComparer.Ordinal);
    private static readonly ConcurrentDictionary<string, byte> KnownCached = new(StringComparer.Ordinal);

    // Files whose first bytes were checked in this run (a cached file is trusted only once it has been seen to be audio).
    private static readonly ConcurrentDictionary<string, byte> Verified = new(StringComparer.Ordinal);

    // One download per track at a time: the playlist prefetch and a tap on Play for the same track used to write the same
    // ".part" file, and the loser reported a failed download.
    private static readonly ConcurrentDictionary<string, Lazy<Task<AudioCacheResult>>> InFlight = new(StringComparer.Ordinal);

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

    /// <summary>Forgets a track that would not play (a damaged or wrong file), so the next attempt downloads it again.</summary>
    public static void Invalidate(string remoteUrlOrCachePath)
    {
        foreach ((string url, string path) in CachePaths.Where(pair => pair.Key == remoteUrlOrCachePath || pair.Value == remoteUrlOrCachePath).ToList())
        {
            KnownCached.TryRemove(url, out _);
            Verified.TryRemove(url, out _);
            TryDelete(path);
        }
    }

    public static async Task<AudioCacheResult> ResolvePlaybackUriAsync(
        string remoteUrl,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(remoteUrl))
        {
            return new AudioCacheResult(remoteUrl, UsedNetwork: false);
        }

        if (!AudioDownloadPolicy.IsAllowedUrl(remoteUrl))
        {
            return new AudioCacheResult(remoteUrl, UsedNetwork: false, DownloadFailed: true);
        }

        string cachePath = GetCachePath(remoteUrl);
        if (File.Exists(cachePath))
        {
            if (IsVerifiedAudio(remoteUrl, cachePath))
            {
                Touch(cachePath);
                return new AudioCacheResult(cachePath, UsedNetwork: false);
            }

            // Something that is not audio is sitting under the track's name (an old error page): it was never a track.
            Invalidate(remoteUrl);
        }

        KnownCached.TryRemove(remoteUrl, out _);

        // The download runs on its own deadline and is shared; a caller that gives up only stops waiting for it. The download removes
        // itself from the table when it ends (not the callers: one that gave up early would leave a finished, possibly failed, result
        // there for every later tap to receive without ever trying again).
        Lazy<Task<AudioCacheResult>> download = InFlight.GetOrAdd(
            remoteUrl,
            url => new Lazy<Task<AudioCacheResult>>(() => DownloadAndForgetAsync(url, GetCachePath(url))));
        return await download.Value.WaitAsync(cancellationToken).ConfigureAwait(false);
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

            try
            {
                await ResolvePlaybackUriAsync(url, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
    }

    private static async Task<AudioCacheResult> DownloadAndForgetAsync(string remoteUrl, string cachePath)
    {
        try
        {
            return await DownloadAsync(remoteUrl, cachePath).ConfigureAwait(false);
        }
        finally
        {
            InFlight.TryRemove(remoteUrl, out _);
        }
    }

    private static async Task<AudioCacheResult> DownloadAsync(string remoteUrl, string cachePath)
    {
        string partialPath = cachePath + ".part";
        using CancellationTokenSource deadline = new(DownloadDeadline);
        try
        {
            using HttpResponseMessage response = await HttpClient.GetAsync(
                remoteUrl,
                HttpCompletionOption.ResponseHeadersRead,
                deadline.Token).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            if (!AudioDownloadPolicy.IsAcceptableContentType(response.Content.Headers.ContentType?.MediaType)
                || response.Content.Headers.ContentLength is > AudioDownloadPolicy.MaxTrackBytes)
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
            byte[] head = new byte[16];
            int headLength = 0;
            await using (Stream body = await response.Content.ReadAsStreamAsync(deadline.Token).ConfigureAwait(false))
            await using (FileStream file = new(partialPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                byte[] buffer = new byte[81920];
                long total = 0;
                int read;
                while ((read = await body.ReadAsync(buffer, deadline.Token).ConfigureAwait(false)) > 0)
                {
                    if (headLength < head.Length)
                    {
                        int take = Math.Min(read, head.Length - headLength);
                        Array.Copy(buffer, 0, head, headLength, take);
                        headLength += take;
                    }

                    total += read;
                    if (total > AudioDownloadPolicy.MaxTrackBytes)
                    {
                        throw new InvalidDataException("Audio file too large.");
                    }

                    await file.WriteAsync(buffer.AsMemory(0, read), deadline.Token).ConfigureAwait(false);
                }
            }

            // "200 OK" with an HTML error page or a login screen is not a track, and must not become one by being renamed.
            if (!AudioDownloadPolicy.LooksLikeAudio(head.AsSpan(0, headLength)))
            {
                TryDelete(partialPath);
                return new AudioCacheResult(remoteUrl, UsedNetwork: true, DownloadFailed: true);
            }

            File.Move(partialPath, cachePath, overwrite: true);
            KnownCached.TryAdd(remoteUrl, 0);
            Verified.TryAdd(remoteUrl, 0);
            TrimCache(keep: cachePath);
            return new AudioCacheResult(cachePath, UsedNetwork: true);
        }
        catch (Exception)
        {
            TryDelete(partialPath);
            return new AudioCacheResult(remoteUrl, UsedNetwork: true, DownloadFailed: true);
        }
    }

    private static bool IsVerifiedAudio(string remoteUrl, string cachePath)
    {
        if (Verified.ContainsKey(remoteUrl))
        {
            return true;
        }

        try
        {
            byte[] head = new byte[16];
            using FileStream file = new(cachePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            int read = file.Read(head, 0, head.Length);
            if (!AudioDownloadPolicy.LooksLikeAudio(head.AsSpan(0, read)))
            {
                return false;
            }
        }
        catch (IOException)
        {
            return false;
        }

        Verified.TryAdd(remoteUrl, 0);
        return true;
    }

    /// <summary>Removes the least recently used tracks until the folder is within <see cref="AudioDownloadPolicy.MaxCacheBytes"/>.</summary>
    private static void TrimCache(string keep)
    {
        try
        {
            string? directory = Path.GetDirectoryName(keep);
            if (directory is null || !Directory.Exists(directory))
            {
                return;
            }

            List<FileInfo> files = new DirectoryInfo(directory).GetFiles("*.mp3").OrderBy(f => f.LastWriteTimeUtc).ToList();
            long total = files.Sum(f => f.Length);
            foreach (FileInfo file in files)
            {
                if (total <= AudioDownloadPolicy.MaxCacheBytes)
                {
                    break;
                }

                if (string.Equals(file.FullName, keep, StringComparison.Ordinal))
                {
                    continue;
                }

                total -= file.Length;
                string path = file.FullName;
                foreach (string url in CachePaths.Where(pair => pair.Value == path).Select(pair => pair.Key).ToList())
                {
                    KnownCached.TryRemove(url, out _);
                    Verified.TryRemove(url, out _);
                }

                TryDelete(path);
            }
        }
        catch (IOException)
        {
            // Trimming is housekeeping; a failure must not turn a successful download into an error.
        }
    }

    // A played track becomes the most recently used one, so trimming removes what has not been listened to.
    private static void Touch(string path)
    {
        try
        {
            File.SetLastWriteTimeUtc(path, DateTime.UtcNow);
        }
        catch (IOException)
        {
        }
    }

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
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
