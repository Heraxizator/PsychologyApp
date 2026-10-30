using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Speech;

namespace PsychologyApp.Presentation.Shared.UI.Components;

/// <summary>
/// The microphone of one input: starts and stops listening, shows what is being heard, and adds the result to the text.
/// Only one input listens at a time; starting another microphone stops the first one.
/// </summary>
public sealed class VoiceDictation
{
    private static VoiceDictation? _active;

    private readonly Func<string> _readText;
    private readonly Action<string> _writeText;
    private CancellationTokenSource? _listening;
    private TaskCompletionSource? _listenFinished;

    public VoiceDictation(Func<string> readText, Action<string> writeText)
    {
        _readText = readText;
        _writeText = writeText;
    }

    public static bool IsAvailable => Speech.IsSupported;

    public bool IsListening => _listening is not null;

    /// <summary>The words heard so far while listening, or a short message after it ends (nothing heard, no permission...).</summary>
    public string StatusText { get; private set; } = string.Empty;

    /// <summary>Raised on the UI thread when <see cref="IsListening"/> or <see cref="StatusText"/> changes.</summary>
    public event EventHandler? Changed;

    private static ISpeechInput Speech =>
        Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services.GetService<ISpeechInput>() ?? new NullSpeechInput();

    /// <summary>A press held this long is "talk while holding"; a shorter one is a tap that leaves the microphone on.</summary>
    private const int HoldMilliseconds = 450;

    /// <summary>A tap this soon after a press or release was the same touch, already handled.</summary>
    private const int SameTouchMilliseconds = 800;

    private DateTime _pressedAt;
    private DateTime _lastTouchAt = DateTime.MinValue;
    private bool _pressStartedListening;
    private bool _pressed;

    /// <summary>
    /// Makes <paramref name="button"/> the microphone: hold it to talk and let go to finish, as in messengers; or tap once to start
    /// and once more to stop. A plain tap also comes from screen readers and keyboards, which have no press and release.
    /// </summary>
    public void Attach(View button, Action? beforeStart = null)
    {
        PointerGestureRecognizer pointer = new();
        pointer.PointerPressed += (_, _) => OnPressed(beforeStart);
        pointer.PointerReleased += (_, _) => OnReleased();
        pointer.PointerExited += (_, _) =>
        {
            // The finger slid off the button: the same as letting go.
            if (_pressed)
            {
                OnReleased();
            }
        };
        button.GestureRecognizers.Add(pointer);
        button.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => OnTapped(beforeStart)) });
    }

    private void OnPressed(Action? beforeStart)
    {
        _pressed = true;
        _pressedAt = _lastTouchAt = DateTime.UtcNow;
        _pressStartedListening = !IsListening;
        if (_pressStartedListening)
        {
            beforeStart?.Invoke();
            ToggleAsync().FireAndForget();
            ShowHoldingWhenHeldAsync().FireAndForget();
        }
    }

    private void OnReleased()
    {
        _pressed = false;
        _lastTouchAt = DateTime.UtcNow;
        bool held = (_lastTouchAt - _pressedAt).TotalMilliseconds >= HoldMilliseconds;

        // Let go after holding: the words are done. A touch on a microphone that was already on stops it too.
        if (IsListening && (held || !_pressStartedListening))
        {
            Speech.StopAsync().FireAndForget();
        }
    }

    private void OnTapped(Action? beforeStart)
    {
        if (_pressed || (DateTime.UtcNow - _lastTouchAt).TotalMilliseconds < SameTouchMilliseconds)
        {
            return;
        }

        if (!IsListening)
        {
            beforeStart?.Invoke();
        }

        ToggleAsync().FireAndForget();
    }

    /// <summary>Once the press turns into a hold, the hint says that letting go finishes.</summary>
    private async Task ShowHoldingWhenHeldAsync()
    {
        await Task.Delay(HoldMilliseconds);
        if (_pressed && IsListening && StatusText == AppStrings.PracticeVoiceListening)
        {
            UiHaptics.Tick();
            SetStatus(AppStrings.PracticeVoiceHolding);
        }
    }

    public async Task ToggleAsync()
    {
        if (IsListening)
        {
            await StopListeningAsync(cancel: false);
            return;
        }

        if (_active is { } other && other != this)
        {
            await other.StopAsync();
        }

        _active = this;
        _listening = new CancellationTokenSource();
        TaskCompletionSource listenFinished = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _listenFinished = listenFinished;
        UiHaptics.Tick();
        SetStatus(AppStrings.PracticeVoiceListening);

        try
        {
            CancellationToken token = _listening.Token;
            SpeechInputResult result = await Speech.ListenAsync(heard => SetStatus(heard), token);
            token.ThrowIfCancellationRequested();
            if (result.Status == SpeechInputStatus.Recognized)
            {
                _writeText(AnswerText.AppendSpoken(_readText(), result.Text));
                UiHaptics.Tick();
            }

            SetStatus(result.Status switch
            {
                SpeechInputStatus.NothingHeard => AppStrings.PracticeVoiceNothingHeard,
                SpeechInputStatus.PermissionDenied => AppStrings.PracticeVoicePermissionDenied,
                SpeechInputStatus.Unavailable => AppStrings.PracticeVoiceUnavailable,
                _ => string.Empty
            }, stopped: true);
        }
        catch (OperationCanceledException)
        {
            SetStatus(string.Empty, stopped: true);
        }
        catch (Exception ex)
        {
            BestEffort.Report(ex);
            SetStatus(AppStrings.PracticeVoiceUnavailable, stopped: true);
        }
        finally
        {
            listenFinished.TrySetResult();
            if (ReferenceEquals(_listenFinished, listenFinished))
            {
                _listenFinished = null;
            }
        }
    }

    /// <summary>Stops listening without keeping the words, e.g. when the page closes.</summary>
    public Task StopAsync() => StopListeningAsync(cancel: true);

    private async Task StopListeningAsync(bool cancel)
    {
        Task? finished = _listenFinished?.Task;
        if (_listening is null && finished is null)
        {
            return;
        }

        if (cancel)
        {
            _listening?.Cancel();
        }

        await Speech.StopAsync();
        if (finished is not null)
        {
            await finished;
        }
    }

    private void SetStatus(string text, bool stopped = false)
    {
        if (stopped)
        {
            _listening?.Dispose();
            _listening = null;
            if (_active == this)
            {
                _active = null;
            }
        }

        StatusText = text;
        MainThread.BeginInvokeOnMainThread(() => Changed?.Invoke(this, EventArgs.Empty));
    }
}
