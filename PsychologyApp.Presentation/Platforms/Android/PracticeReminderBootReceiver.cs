#if ANDROID
using Android.App;
using Android.Content;
using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Platforms.Android;

[BroadcastReceiver(Enabled = true, Exported = false)]
[IntentFilter([Intent.ActionBootCompleted])]
public sealed class PracticeReminderBootReceiver : BroadcastReceiver
{
    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null || intent?.Action != Intent.ActionBootCompleted)
        {
            return;
        }

        try
        {
            IServiceProvider? services = MauiApplication.Current?.Services;
            if (services?.GetService<IPracticeReminderCoordinator>() is not IPracticeReminderCoordinator coordinator)
            {
                return;
            }

            _ = coordinator.SyncAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Practice reminder boot sync failed: {ex.Message}");
        }
    }
}
#endif
