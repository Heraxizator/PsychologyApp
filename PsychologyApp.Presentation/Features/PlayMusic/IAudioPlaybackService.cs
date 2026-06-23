namespace PsychologyApp.Presentation.Features.PlayMusic;

public interface IAudioPlaybackService
{
    Task PlayAsync(string uri, CancellationToken cancellationToken = default);
    Task PauseAsync(CancellationToken cancellationToken = default);
    Task ResumeAsync(CancellationToken cancellationToken = default);
    Task SeekAsync(TimeSpan position, CancellationToken cancellationToken = default);
    bool IsPlaying { get; }
    TimeSpan Position { get; }
    TimeSpan Duration { get; }
    event EventHandler? PlaybackEnded;
    event EventHandler? PlaybackFailed;
}
