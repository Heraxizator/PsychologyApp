using System.Globalization;
using CommunityToolkit.Maui.Media;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Shared.Services.Speech;

/// <summary>
/// <see cref="ISpeechInput"/> on the toolkit's offline recogniser: Android 13+ (on-device <c>SpeechRecognizer</c>) and iOS 13+
/// (<c>RequiresOnDeviceRecognition</c>). Older systems would send audio to a server, so there the microphone is not offered.
/// </summary>
public sealed class DeviceSpeechInput : ISpeechInput
{
    // Some recognisers keep going after an explicit stop; then what was heard so far is used.
    private static readonly TimeSpan StopGrace = TimeSpan.FromMilliseconds(1500);

    private TaskCompletionSource? _stopRequested;
    private bool? _isSupported;

    public bool IsSupported => _isSupported ??= DetectSupport();

    public async Task<SpeechInputResult> ListenAsync(Action<string> onPartial, CancellationToken cancellationToken = default)
    {
        if (!IsSupported)
        {
            return new SpeechInputResult(SpeechInputStatus.Unavailable);
        }

        ISpeechToText recognizer = OfflineSpeechToText.Default;
        bool granted;
        try
        {
            granted = await recognizer.RequestPermissions(cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new SpeechInputResult(SpeechInputStatus.Unavailable);
        }

        if (!granted)
        {
            return new SpeechInputResult(SpeechInputStatus.PermissionDenied);
        }

        string lastPartial = string.Empty;
        TaskCompletionSource<SpeechToTextResult> completed = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource stopRequested = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _stopRequested = stopRequested;

        void OnUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs e)
        {
            lastPartial = e.RecognitionResult ?? string.Empty;
            string heard = lastPartial;
            MainThread.BeginInvokeOnMainThread(() => onPartial(heard));
        }

        void OnCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs e) => completed.TrySetResult(e.RecognitionResult);

        recognizer.RecognitionResultUpdated += OnUpdated;
        recognizer.RecognitionResultCompleted += OnCompleted;
        try
        {
            await recognizer.StartListenAsync(
                new SpeechToTextOptions { Culture = CurrentCulture(), ShouldReportPartialResults = true },
                cancellationToken);

            using CancellationTokenRegistration registration = cancellationToken.Register(() => completed.TrySetCanceled(cancellationToken));
            Task first = await Task.WhenAny(completed.Task, stopRequested.Task);
            if (first == stopRequested.Task)
            {
                await SafeStopAsync(recognizer);
                await Task.WhenAny(completed.Task, Task.Delay(StopGrace, cancellationToken));
            }

            if (completed.Task.IsCompletedSuccessfully)
            {
                SpeechToTextResult result = completed.Task.Result;
                string text = result.IsSuccessful && !string.IsNullOrWhiteSpace(result.Text) ? result.Text : lastPartial;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return new SpeechInputResult(SpeechInputStatus.Recognized, text.Trim());
                }

                return new SpeechInputResult(result.IsSuccessful ? SpeechInputStatus.NothingHeard : SpeechInputStatus.Unavailable);
            }

            return string.IsNullOrWhiteSpace(lastPartial)
                ? new SpeechInputResult(SpeechInputStatus.NothingHeard)
                : new SpeechInputResult(SpeechInputStatus.Recognized, lastPartial.Trim());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // No language pack on the device, the recogniser is busy, or the platform refused: typing still works.
            return new SpeechInputResult(SpeechInputStatus.Unavailable);
        }
        finally
        {
            recognizer.RecognitionResultUpdated -= OnUpdated;
            recognizer.RecognitionResultCompleted -= OnCompleted;
            _stopRequested = null;
            if (recognizer.CurrentState != SpeechToTextState.Stopped)
            {
                await SafeStopAsync(recognizer);
            }
        }
    }

    public Task StopAsync()
    {
        _stopRequested?.TrySetResult();
        return Task.CompletedTask;
    }

    private static async Task SafeStopAsync(ISpeechToText recognizer)
    {
        try
        {
            await recognizer.StopListenAsync(CancellationToken.None);
        }
        catch (Exception)
        {
            // Already stopped on its own.
        }
    }

    private static CultureInfo CurrentCulture() =>
        AppStrings.IsEnglish(AppStrings.Language) ? new CultureInfo("en-US") : new CultureInfo("ru-RU");

    private static bool DetectSupport()
    {
        try
        {
#if ANDROID
            return OperatingSystem.IsAndroidVersionAtLeast(33)
                && Android.Speech.SpeechRecognizer.IsOnDeviceRecognitionAvailable(Microsoft.Maui.ApplicationModel.Platform.AppContext);
#elif IOS
            return OperatingSystem.IsIOSVersionAtLeast(13);
#else
            return false;
#endif
        }
        catch (Exception)
        {
            return false;
        }
    }
}
