#if ANDROID
using Android.App;
using Android.Content;
using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

[BroadcastReceiver(Exported = false)]
public sealed class QuoteReminderBootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null ||
            intent?.Action is not (Intent.ActionBootCompleted or QuoteReminderConstants.ActionBoot))
        {
            return;
        }

        try
        {
            IServiceProvider? services = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services;
            if (services?.GetService<IQuoteReminderCoordinator>() is not IQuoteReminderCoordinator coordinator)
            {
                return;
            }

            coordinator.SyncAsync().GetAwaiter().GetResult();
        }
        catch
        {
        }
    }
}
#endif
