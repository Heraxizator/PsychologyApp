#if !ANDROID
using CommunityToolkit.Maui.Views;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.PlayMusic;

public sealed class MediaElementAudioPlaybackService : IAudioPlaybackService
{
    private MediaElement? _player;
    private Func<MediaElement>? _createPlayer;
    private bool _isPlaying;

    public bool IsPlaying => _isPlaying;

    public TimeSpan Position => _player?.Position ?? TimeSpan.Zero;

    public TimeSpan Duration => _player?.Duration ?? TimeSpan.Zero;

    public event EventHandler? PlaybackEnded;

    public event EventHandler? PlaybackFailed;

    /// <summary>
    /// The player is only created on the first play: building a MediaElement spins up ExoPlayer and a media session,
    /// which froze the music page for seconds on open even when nothing was ever played.
    /// </summary>
    public void AttachLazily(Func<MediaElement> createPlayer) => _createPlayer = createPlayer;

    public void Detach()
    {
        _createPlayer = null;
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

    private MediaElement RequirePlayer()
    {
        if (_player is not null)
        {
            return _player;
        }

        _player = _createPlayer?.Invoke() ?? throw new InvalidOperationException("MediaElement is not attached.");
        _player.MediaEnded += OnMediaEnded;
        _player.MediaFailed += OnMediaFailed;
        return _player;
    }

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
#endif
