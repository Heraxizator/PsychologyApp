using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace PsychologyApp.Presentation.Services.Toasts;

public class ToastService : IToastService
{
    public void LongToast(string message) =>
        Toast.Make(message, ToastDuration.Long).Show();

    public void ShortToast(string message) =>
        Toast.Make(message, ToastDuration.Short).Show();
}
