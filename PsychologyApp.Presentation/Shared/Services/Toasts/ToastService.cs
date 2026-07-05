using CommunityToolkit.Maui.Extensions;
using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.UI.Overlays;

namespace PsychologyApp.Presentation.Shared.Services.Toasts;

public class ToastService(IPageHost pageHost, ILogger<ToastService> logger) : IToastService
{
    private static readonly TimeSpan ShortDuration = TimeSpan.FromSeconds(2.2);
    private static readonly TimeSpan LongDuration = TimeSpan.FromSeconds(3.6);
    private static readonly TimeSpan GateTimeout = TimeSpan.FromSeconds(5);
    private readonly SemaphoreSlim _toastGate = new(1, 1);
    private AppToastPopup? _activePopup;

    public void LongToast(string message, AppToastKind kind = AppToastKind.Info) =>
        Show(message, LongDuration, kind);

    public void ShortToast(string message, AppToastKind kind = AppToastKind.Info) =>
        Show(message, ShortDuration, kind);

    private void Show(string message, TimeSpan duration, AppToastKind kind)
    {
        if (MainThread.IsMainThread)
        {
            ShowOnMainThread(message, duration, kind).FireAndForget();
            return;
        }

        MainThread.BeginInvokeOnMainThread(() => ShowOnMainThread(message, duration, kind).FireAndForget());
    }

    private async Task ShowOnMainThread(string message, TimeSpan duration, AppToastKind kind)
    {
        if (!await _toastGate.WaitAsync(GateTimeout))
        {
            logger.LogWarning("Toast dropped: queue gate timeout.");
            return;
        }

        try
        {
            Page? page = pageHost.GetActivePage();
            if (page is null || string.IsNullOrWhiteSpace(message))
            {
                logger.LogWarning("Toast dropped: active page unavailable.");
                return;
            }

            await CloseActivePopupAsync();

            var popup = new AppToastPopup(message, kind);
            _activePopup = popup;
            Task showTask = page.ShowPopupAsync(popup, AppPopupOptions.Toast);
            await popup.WaitUntilReadyAsync();
            await Task.Delay(duration);
            await popup.DismissAsync();
            await showTask;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Toast display failed.");
        }
        finally
        {
            _activePopup = null;
            _toastGate.Release();
        }
    }

    private async Task CloseActivePopupAsync()
    {
        if (_activePopup is null)
        {
            return;
        }

        try
        {
            await _activePopup.CloseAsync();
        }
        catch (InvalidOperationException)
        {
        }

        _activePopup = null;
    }
}
