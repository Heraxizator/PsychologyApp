namespace PsychologyApp.Presentation.Shared.Services.Speech;

public enum SpeechInputStatus
{
    Recognized,
    NothingHeard,
    PermissionDenied,
    Unavailable
}

public sealed record SpeechInputResult(SpeechInputStatus Status, string Text = "");

/// <summary>
/// Dictation instead of typing. Recognition runs on the device only: like the rest of the app, the person's words never leave the phone,
/// so where the system has no on-device recogniser the microphone is simply not offered.
/// </summary>
public interface ISpeechInput
{
    bool IsSupported { get; }

    /// <summary>Listens until the person stops talking (or <see cref="StopAsync"/> is called) and returns what was said.</summary>
    /// <param name="onPartial">The words heard so far, on the UI thread, to show while the person is still speaking.</param>
    Task<SpeechInputResult> ListenAsync(Action<string> onPartial, CancellationToken cancellationToken = default);

    /// <summary>Ends the current listening early; <see cref="ListenAsync"/> then returns what was heard up to this point.</summary>
    Task StopAsync();
}

public sealed class NullSpeechInput : ISpeechInput
{
    public bool IsSupported => false;

    public Task<SpeechInputResult> ListenAsync(Action<string> onPartial, CancellationToken cancellationToken = default) =>
        Task.FromResult(new SpeechInputResult(SpeechInputStatus.Unavailable));

    public Task StopAsync() => Task.CompletedTask;
}
