using PsychologyApp.Presentation.Entities.Audio;
using PsychologyApp.Presentation.Features.PlayMusic;

namespace PsychologyApp.Presentation.Pages.MusicPlayer;

public partial class MusicPlayerViewModel
{
    public void UpdatePlaybackProgressFromService() =>
        ApplyProgress(_playbackPresenter.CreateProgress(_playbackService.Position, _playbackService.Duration));

    public void UpdatePlaybackProgress(TimeSpan position, TimeSpan duration) =>
        ApplyProgress(_playbackPresenter.CreateProgress(position, duration));

    public async Task SeekToFractionAsync(double fraction)
    {
        TimeSpan position = await _playbackPresenter.SeekToFractionAsync(_playbackService, fraction);
        ApplyProgress(_playbackPresenter.CreateProgress(position, _playbackService.Duration));
    }

    private async Task PlayTrackAsync(Audio item)
    {
        ActiveTrack = item;
        IsBuffering = true;

        bool started = await _playbackPresenter.PlayTrackAsync(_playbackService, item);
        SetPlaybackState(started);
        if (started)
        {
            RefreshCacheFlags();
        }

        IsBuffering = false;
    }

    private async Task PlayNextAsync()
    {
        Audio? next = _playbackPresenter.ResolveAdjacentTrack(ActiveTrack, FilteredItems, AllItems, step: 1);
        if (next is not null)
        {
            await PlayTrackAsync(next);
        }
    }

    private async Task PlayPreviousAsync()
    {
        Audio? previous = _playbackPresenter.ResolveAdjacentTrack(ActiveTrack, FilteredItems, AllItems, step: -1);
        if (previous is not null)
        {
            await PlayTrackAsync(previous);
        }
    }

    public void SetPlaybackState(bool isPlaying) => IsPlaying = isPlaying;

    public void SyncActiveTrackFlags() =>
        MusicPlaybackPresenter.SyncActiveTrackFlags(AllItems, ActiveTrack, IsPlaying);

    private void ApplyProgress(MusicPlaybackProgress progress)
    {
        PositionDisplay = progress.PositionDisplay;
        DurationDisplay = progress.DurationDisplay;
        ProgressFraction = progress.ProgressFraction;
        OnPropertyChanged(nameof(ProgressDisplay));
    }
}
