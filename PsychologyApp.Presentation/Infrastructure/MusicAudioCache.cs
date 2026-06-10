using System.Security.Cryptography;
using System.Text;

namespace PsychologyApp.Presentation.Infrastructure;

public static class MusicAudioCache
{
    public static async Task<string> ResolvePlaybackUriAsync(string remoteUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(remoteUrl))
        {
            return remoteUrl;
        }

        string cachePath = GetCachePath(remoteUrl);
        if (File.Exists(cachePath))
        {
            return cachePath;
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
            return cachePath;
        }
        catch
        {
            return remoteUrl;
        }
    }

    private static string GetCachePath(string remoteUrl)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(remoteUrl));
        string fileName = Convert.ToHexString(hash).ToLowerInvariant() + ".mp3";
        return Path.Combine(FileSystem.CacheDirectory, "music", fileName);
    }
}
