using PsychologyApp.Presentation.Shared.UI.Overlays;

namespace PsychologyApp.Presentation.Shared.Services.Clipboard;

public interface IAppClipboardService
{
    Task CopyWithFeedbackAsync(
        string text,
        string feedbackMessage,
        AppToastKind kind = AppToastKind.Success);
}
