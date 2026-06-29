using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;

namespace PsychologyApp.Presentation.Shared.Navigation;

internal static class NavigationCoordinator
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static long _pushBlockedUntilUtcTicks;
    private static readonly TimeSpan PushCooldown = TimeSpan.FromMilliseconds(350);
    private static ILogger? _logger;

    internal static void SetLogger(ILogger logger) => _logger = logger;

    internal static Action? NavigationDroppedHandler { get; set; }

    public static Task RunAsync(Func<Task> navigate) =>
        RunCoreAsync(navigate, applyPushCooldown: false);

    private static readonly TimeSpan PushGateWait = TimeSpan.FromSeconds(2);

    public static Task RunPushAsync(Func<Task> navigate) =>
        RunCoreAsync(navigate, applyPushCooldown: true, waitForGate: true);

    private static async Task RunCoreAsync(Func<Task> navigate, bool applyPushCooldown, bool waitForGate = false)
    {
        if (applyPushCooldown)
        {
            long now = DateTime.UtcNow.Ticks;
            long blockedUntil = Volatile.Read(ref _pushBlockedUntilUtcTicks);
            if (now < blockedUntil)
            {
                TimeSpan wait = TimeSpan.FromTicks(blockedUntil - now);
                if (wait > TimeSpan.Zero)
                {
                    await Task.Delay(wait).ConfigureAwait(false);
                }
            }
        }

        bool acquired = waitForGate
            ? await Gate.WaitAsync(PushGateWait).ConfigureAwait(false)
            : await Gate.WaitAsync(0).ConfigureAwait(false);

        if (!acquired)
        {
            string message = waitForGate
                ? "Navigation dropped: coordinator gate timeout."
                : "Navigation dropped: coordinator gate busy.";
            _logger?.LogWarning(message);
            NavigationDroppedHandler?.Invoke();
            return;
        }

        try
        {
            await MainThread.InvokeOnMainThreadAsync(navigate).ConfigureAwait(false);
        }
        finally
        {
            if (applyPushCooldown)
            {
                Volatile.Write(ref _pushBlockedUntilUtcTicks, DateTime.UtcNow.Add(PushCooldown).Ticks);
            }

            Gate.Release();
        }
    }
}
