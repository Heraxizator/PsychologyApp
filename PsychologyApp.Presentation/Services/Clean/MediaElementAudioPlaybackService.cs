using CommunityToolkit.Maui.Views;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Services.Clean;

public sealed class MediaElementAudioPlaybackService : IAudioPlaybackService
{
    private MediaElement? _player;
    private bool _isPlaying;

    public bool IsPlaying => _isPlaying;

    public TimeSpan Position => _player?.Position ?? TimeSpan.Zero;

    public TimeSpan Duration => _player?.Duration ?? TimeSpan.Zero;

    public event EventHandler? PlaybackEnded;

    public event EventHandler? PlaybackFailed;

    public void Attach(MediaElement player)
    {
        if (_player is not null)
        {
            _player.MediaEnded -= OnMediaEnded;
            _player.MediaFailed -= OnMediaFailed;
        }

        _player = player;
        _player.MediaEnded += OnMediaEnded;
        _player.MediaFailed += OnMediaFailed;
    }

    public void Detach()
    {
        if (_player is null)
        {
            return;
        }

        _player.MediaEnded -= OnMediaEnded;
        _player.MediaFailed -= OnMediaFailed;
        _player = null;
        _isPlaying = false;
    }

    public async Task PlayAsync(string uri, CancellationToken cancellationToken = default)
    {
        MediaElement player = RequirePlayer();
        AudioCacheResult result = await MusicAudioCache.ResolvePlaybackUriAsync(uri);
        if (result.DownloadFailed)
        {
            throw new InvalidOperationException("Audio download failed.");
        }

        cancellationToken.ThrowIfCancellationRequested();
        player.Source = MediaSource.FromUri(result.Uri);
        player.Play();
        _isPlaying = true;
    }

    public Task PauseAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _player?.Pause();
        _isPlaying = false;
        return Task.CompletedTask;
    }

    public Task ResumeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _player?.Play();
        _isPlaying = true;
        return Task.CompletedTask;
    }

    public async Task SeekAsync(TimeSpan position, CancellationToken cancellationToken = default)
    {
        MediaElement player = RequirePlayer();
        if (player.Duration.TotalSeconds <= 0)
        {
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        await player.SeekTo(position);
    }

    private MediaElement RequirePlayer() =>
        _player ?? throw new InvalidOperationException("MediaElement is not attached.");

    private void OnMediaEnded(object? sender, EventArgs e)
    {
        _isPlaying = false;
        PlaybackEnded?.Invoke(this, EventArgs.Empty);
    }

    private void OnMediaFailed(object? sender, EventArgs e)
    {
        _isPlaying = false;
        PlaybackFailed?.Invoke(this, EventArgs.Empty);
    }
}
