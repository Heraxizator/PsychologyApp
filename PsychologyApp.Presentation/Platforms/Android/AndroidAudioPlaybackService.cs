using Android.Content;
using Android.Media;
using PsychologyApp.Presentation.Features.PlayMusic;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Platforms.Android;

/// <summary>
/// Plays the meditation tracks with the platform MediaPlayer. MediaElement brought ExoPlayer, a media session and
/// their bindings (several MB of the app) for what is one local file at a time. Created on the UI thread, so its
/// callbacks arrive there too.
/// </summary>
public sealed class AndroidAudioPlaybackService : Java.Lang.Object, IAudioPlaybackService, AudioManager.IOnAudioFocusChangeListener
{
    private MediaPlayer? _player;
    private TaskCompletionSource? _prepared;
    private AudioFocusRequestClass? _focusRequest;
    private bool _isPrepared;
    private bool _isPlaying;

    public bool IsPlaying => _isPlaying;

    public TimeSpan Position => _isPrepared && _player is not null
        ? TimeSpan.FromMilliseconds(_player.CurrentPosition)
        : TimeSpan.Zero;

    public TimeSpan Duration => _isPrepared && _player is not null && _player.Duration > 0
        ? TimeSpan.FromMilliseconds(_player.Duration)
        : TimeSpan.Zero;

    public event EventHandler? PlaybackEnded;

    public event EventHandler? PlaybackFailed;

    public async Task PlayAsync(string uri, CancellationToken cancellationToken = default)
    {
        AudioCacheResult result = await MusicAudioCache.ResolvePlaybackUriAsync(uri, cancellationToken);
        if (result.DownloadFailed)
        {
            throw new InvalidOperationException("Audio download failed.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        // A track tapped while the previous one is still preparing replaces it; the earlier call just stops waiting.
        _prepared?.TrySetCanceled();
        MediaPlayer player = EnsurePlayer();
        player.Reset();
        _isPrepared = false;
        _isPlaying = false;
        player.SetDataSource(result.Uri);

        _prepared = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        player.PrepareAsync();
        await _prepared.Task;
        cancellationToken.ThrowIfCancellationRequested();

        _isPrepared = true;
        Start();
    }

    public Task PauseAsync(CancellationToken cancellationToken = default)
    {
        if (_isPrepared && _player is { IsPlaying: true } player)
        {
            player.Pause();
        }

        _isPlaying = false;
        AbandonFocus();
        return Task.CompletedTask;
    }

    public Task ResumeAsync(CancellationToken cancellationToken = default)
    {
        if (_isPrepared)
        {
            Start();
        }

        return Task.CompletedTask;
    }

    public Task SeekAsync(TimeSpan position, CancellationToken cancellationToken = default)
    {
        if (_isPrepared && _player is not null && _player.Duration > 0)
        {
            _player.SeekTo((int)position.TotalMilliseconds);
        }

        return Task.CompletedTask;
    }

    /// <summary>Frees the native player when its page goes away.</summary>
    public void Release()
    {
        AbandonFocus();
        _prepared?.TrySetCanceled();
        if (_player is null)
        {
            return;
        }

        _player.Prepared -= OnPrepared;
        _player.Completion -= OnCompletion;
        _player.Error -= OnError;
        _player.Release();
        _player.Dispose();
        _player = null;
        _isPrepared = false;
        _isPlaying = false;
    }

    // Another app or a call took the audio: stop like a finished track, so the play button shows "play" again.
    public void OnAudioFocusChange(AudioFocus focusChange)
    {
        if (_isPlaying && focusChange is AudioFocus.Loss or AudioFocus.LossTransient or AudioFocus.LossTransientCanDuck)
        {
            PauseAsync();
            PlaybackEnded?.Invoke(this, EventArgs.Empty);
        }
    }

    private MediaPlayer EnsurePlayer()
    {
        if (_player is not null)
        {
            return _player;
        }

        _player = new MediaPlayer();
        _player.SetAudioAttributes(new AudioAttributes.Builder()!
            .SetUsage(AudioUsageKind.Media)!
            .SetContentType(AudioContentType.Music)!
            .Build()!);
        _player.Prepared += OnPrepared;
        _player.Completion += OnCompletion;
        _player.Error += OnError;
        return _player;
    }

    private void Start()
    {
        RequestFocus();
        _player?.Start();
        _isPlaying = true;
    }

    private void RequestFocus()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26) || _focusRequest is not null || AudioService() is not { } audio)
        {
            return;
        }

        AudioFocusRequestClass request = new AudioFocusRequestClass.Builder(AudioFocus.Gain)
            .SetAudioAttributes(new AudioAttributes.Builder()!
                .SetUsage(AudioUsageKind.Media)!
                .SetContentType(AudioContentType.Music)!
                .Build()!)
            .SetOnAudioFocusChangeListener(this)
            .Build()!;
        _focusRequest = request;
        audio.RequestAudioFocus(request);
    }

    private void AbandonFocus()
    {
        if (_focusRequest is null || !OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            return;
        }

        AudioService()?.AbandonAudioFocusRequest(_focusRequest);
        _focusRequest = null;
    }

    private static AudioManager? AudioService() =>
        global::Android.App.Application.Context.GetSystemService(Context.AudioService) as AudioManager;

    private void OnPrepared(object? sender, EventArgs e) => _prepared?.TrySetResult();

    private void OnCompletion(object? sender, EventArgs e)
    {
        _isPlaying = false;
        AbandonFocus();
        PlaybackEnded?.Invoke(this, EventArgs.Empty);
    }

    private void OnError(object? sender, MediaPlayer.ErrorEventArgs e)
    {
        e.Handled = true;
        _isPlaying = false;
        _isPrepared = false;
        AbandonFocus();
        _prepared?.TrySetException(new InvalidOperationException($"MediaPlayer error {e.What}/{e.Extra}."));
        PlaybackFailed?.Invoke(this, EventArgs.Empty);
    }
}
