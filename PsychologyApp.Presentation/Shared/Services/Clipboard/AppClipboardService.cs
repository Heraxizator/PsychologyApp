using Microsoft.Maui.ApplicationModel.DataTransfer;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Shared.UI.Overlays;

namespace PsychologyApp.Presentation.Shared.Services.Clipboard;

public sealed class AppClipboardService(IToastService toastService) : IAppClipboardService
{
    public async Task CopyWithFeedbackAsync(
        string text,
        string feedbackMessage,
        AppToastKind kind = AppToastKind.Success)
    {
        await Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.Default.SetTextAsync(text);
        toastService.ShortToast(feedbackMessage, kind);
    }
}
