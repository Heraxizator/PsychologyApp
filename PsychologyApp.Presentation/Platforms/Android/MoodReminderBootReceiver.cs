#if ANDROID
using Android.App;
using Android.Content;
using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

[BroadcastReceiver(Exported = false)]
public sealed class MoodReminderBootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null ||
            intent?.Action is not (Intent.ActionBootCompleted or MoodReminderConstants.ActionBoot))
        {
            return;
        }

        try
        {
            IServiceProvider? services = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services;
            if (services?.GetService<IMoodReminderCoordinator>() is not IMoodReminderCoordinator coordinator)
            {
                return;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await coordinator.SyncAsync().ConfigureAwait(false);
                }
                catch
                {
                }
            });
        }
        catch
        {
        }
    }
}
#endif
