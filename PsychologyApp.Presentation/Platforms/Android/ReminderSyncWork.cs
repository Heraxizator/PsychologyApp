#if ANDROID
using Android.Content;
using Microsoft.Extensions.DependencyInjection;

namespace PsychologyApp.Presentation.Platforms.Android;

/// <summary>
/// Runs a reminder coordinator's "schedule the next alarm" from a broadcast receiver, for all four reminder kinds.
/// <see cref="BroadcastReceiver.GoAsync"/> keeps the receiver (and the process) alive until the asynchronous sync has finished;
/// a bare fire-and-forget task may be cut off when <c>OnReceive</c> returns, which would leave no next alarm set. The services come
/// from <c>MauiApplication.Current</c>, which is built in <c>MainApplication.OnCreate</c> before any receiver runs.
/// </summary>
internal static class ReminderSyncWork
{
    internal static void Run<TCoordinator>(BroadcastReceiver receiver, Func<TCoordinator, Task> sync)
        where TCoordinator : class
    {
        BroadcastReceiver.PendingResult? pending = receiver.GoAsync();
        _ = Task.Run(async () =>
        {
            try
            {
                IServiceProvider? services = MauiApplication.Current?.Services;
                if (services?.GetService<TCoordinator>() is TCoordinator coordinator)
                {
                    await sync(coordinator).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{typeof(TCoordinator).Name} sync from a broadcast failed: {ex.Message}");
            }
            finally
            {
                pending?.Finish();
            }
        });
    }
}
#endif
