namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// What may be downloaded and kept as a playable track. A server that answers "200 OK" with an error page, a captive-portal login
/// or a half file must not end up in the cache under a ".mp3" name, because the cache is trusted forever once a file exists.
/// </summary>
public static class AudioDownloadPolicy
{
    public const long MaxTrackBytes = 50L * 1024 * 1024;

    /// <summary>The whole cache folder may not grow past this; the least recently used tracks are removed first.</summary>
    public const long MaxCacheBytes = 200L * 1024 * 1024;

    /// <summary>Tracks are fetched over HTTPS only.</summary>
    public static bool IsAllowedUrl(string? url) =>
        Uri.TryCreate(url, UriKind.Absolute, out Uri? uri)
        && uri.Scheme == Uri.UriSchemeHttps
        && !string.IsNullOrEmpty(uri.Host);

    /// <summary>A missing type is tolerated (some static hosts omit it); HTML, JSON and other text are not audio.</summary>
    public static bool IsAcceptableContentType(string? mediaType) =>
        string.IsNullOrWhiteSpace(mediaType)
        || mediaType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase)
        || mediaType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase)
        || mediaType.Equals("application/ogg", StringComparison.OrdinalIgnoreCase)
        || mediaType.Equals("binary/octet-stream", StringComparison.OrdinalIgnoreCase)
        || mediaType.Equals("video/mp4", StringComparison.OrdinalIgnoreCase);

    /// <summary>The first bytes of a file look like a known audio container: MP3 (ID3 tag or frame sync), Ogg, FLAC, WAV or MP4/M4A.</summary>
    public static bool LooksLikeAudio(ReadOnlySpan<byte> head)
    {
        if (head.Length < 4)
        {
            return false;
        }

        if (head[0] == 'I' && head[1] == 'D' && head[2] == '3')
        {
            return true;
        }

        if (head[0] == 0xFF && (head[1] & 0xE0) == 0xE0)
        {
            return true;
        }

        if (head.StartsWith("OggS"u8) || head.StartsWith("fLaC"u8) || head.StartsWith("RIFF"u8))
        {
            return true;
        }

        return head.Length >= 8 && head.Slice(4, 4).SequenceEqual("ftyp"u8);
    }
}
