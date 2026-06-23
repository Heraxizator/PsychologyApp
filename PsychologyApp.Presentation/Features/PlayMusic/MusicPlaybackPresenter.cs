using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Entities.Audio;

namespace PsychologyApp.Presentation.Features.PlayMusic;

public sealed class MusicPlaybackProgress
{
    public required string PositionDisplay { get; init; }
    public required string DurationDisplay { get; init; }
    public required double ProgressFraction { get; init; }
}

public sealed class MusicPlaybackPresenter(
    ILogger<MusicPlaybackPresenter> logger,
    MusicPlaylistPresenter playlistPresenter)
{
    public MusicPlaybackProgress CreateProgress(TimeSpan position, TimeSpan duration) =>
        new()
        {
            PositionDisplay = FormatTime(position),
            DurationDisplay = FormatTime(duration),
            ProgressFraction = duration.TotalSeconds > 0
                ? Math.Clamp(position.TotalSeconds / duration.TotalSeconds, 0, 1)
                : 0
        };

    public static void SyncActiveTrackFlags(IEnumerable<Audio> allItems, Audio? activeTrack, bool isPlaying)
    {
        foreach (Audio item in allItems)
        {
            bool isActive = ReferenceEquals(item, activeTrack);
            item.IsActive = isActive;
            item.IsPlayingThis = isActive && isPlaying;
        }
    }

    public Audio? ResolveAdjacentTrack(
        Audio? activeTrack,
        IReadOnlyList<Audio> filteredItems,
        IReadOnlyList<Audio> allItems,
        int step) =>
        playlistPresenter.ResolveAdjacentTrack(activeTrack, filteredItems, allItems, step);

    public async Task<bool> PlayTrackAsync(IAudioPlaybackService playbackService, Audio item)
    {
        if (string.IsNullOrWhiteSpace(item.URL))
        {
            return false;
        }

        try
        {
            await playbackService.PlayAsync(item.URL);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to play audio track.");
            return false;
        }
    }

    public async Task<bool> TogglePlayPauseAsync(IAudioPlaybackService playbackService, bool isPlaying)
    {
        if (isPlaying)
        {
            await playbackService.PauseAsync();
            return false;
        }

        await playbackService.ResumeAsync();
        return true;
    }

    public async Task<TimeSpan> SeekToFractionAsync(IAudioPlaybackService playbackService, double fraction)
    {
        if (playbackService.Duration.TotalSeconds <= 0)
        {
            return playbackService.Position;
        }

        double clamped = Math.Clamp(fraction, 0, 1);
        TimeSpan target = TimeSpan.FromSeconds(playbackService.Duration.TotalSeconds * clamped);
        await playbackService.SeekAsync(target);
        return playbackService.Position;
    }

    public static string FormatTime(TimeSpan time)
    {
        if (time.TotalHours >= 1)
        {
            return $"{(int)time.TotalHours}:{time.Minutes:D2}:{time.Seconds:D2}";
        }

        return $"{time.Minutes}:{time.Seconds:D2}";
    }
}
