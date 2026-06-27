using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.Services.Toasts;

public class ToastService : IToastService
{
    public void LongToast(string message) => Show(message, ToastDuration.Long);

    public void ShortToast(string message) => Show(message, ToastDuration.Short);

    private static void Show(string message, ToastDuration duration)
    {
        if (MainThread.IsMainThread)
        {
            Toast.Make(message, duration).Show().FireAndForget();
            return;
        }

        MainThread.BeginInvokeOnMainThread(() =>
            Toast.Make(message, duration).Show().FireAndForget());
    }
}
