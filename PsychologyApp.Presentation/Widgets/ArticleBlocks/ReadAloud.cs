using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Widgets.ArticleBlocks;

/// <summary>
/// Reads an article aloud with the phone's own speech engine, part by part (a long text in one go is cut off by some engines).
/// Only the app's own text is spoken: nothing the person wrote. Stops when the screen is left.
/// </summary>
public sealed class ReadAloud
{
    private CancellationTokenSource? _speaking;

    public bool IsSpeaking => _speaking is not null;

    public event Action? Changed;

    public async Task ToggleAsync(IReadOnlyList<string> parts)
    {
        if (_speaking is not null)
        {
            Stop();
            return;
        }

        CancellationTokenSource speaking = new();
        _speaking = speaking;
        Changed?.Invoke();
        try
        {
            string language = AppStrings.IsEnglish(AppStrings.Language) ? "en" : "ru";
            Locale? locale = (await TextToSpeech.Default.GetLocalesAsync())
                .FirstOrDefault(candidate => candidate.Language.StartsWith(language, StringComparison.OrdinalIgnoreCase));
            SpeechOptions options = new() { Locale = locale };
            foreach (string part in parts)
            {
                speaking.Token.ThrowIfCancellationRequested();
                await TextToSpeech.Default.SpeakAsync(part, options, speaking.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // Stopped by the person or by leaving the screen.
        }
        catch (Exception speechError)
        {
            // No speech engine or no voice for the language: the text is still there to read.
            BestEffort.Report(speechError);
        }
        finally
        {
            if (ReferenceEquals(_speaking, speaking))
            {
                _speaking = null;
                Changed?.Invoke();
            }

            speaking.Dispose();
        }
    }

    public void Stop()
    {
        CancellationTokenSource? speaking = _speaking;
        _speaking = null;
        speaking?.Cancel();
        Changed?.Invoke();
    }
}
