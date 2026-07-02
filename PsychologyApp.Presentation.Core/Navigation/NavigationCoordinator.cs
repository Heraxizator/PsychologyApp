using Microsoft.Extensions.Logging;

namespace PsychologyApp.Presentation.Shared.Navigation;

public static class NavigationCoordinator
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static long _pushBlockedUntilUtcTicks;
    private static readonly TimeSpan PushCooldown = TimeSpan.FromMilliseconds(350);
    private static ILogger? _logger;

    private static readonly TimeSpan PushGateWait = TimeSpan.FromSeconds(4);
    private static readonly TimeSpan CompletionGateWait = TimeSpan.FromSeconds(8);

    public static void SetLogger(ILogger logger) => _logger = logger;

    public static void LogNavigationFailure(Exception exception, string message) =>
        _logger?.LogWarning(exception, message);

    public static async Task RunAsync(Func<Task> navigate)
    {
        _ = await RunCoreAsync(navigate, applyPushCooldown: false).ConfigureAwait(false);
    }

    public static Task<NavigationRunStatus> RunPushAsync(Func<Task> navigate) =>
        RunCoreAsync(navigate, applyPushCooldown: true, waitForGate: true, gateWait: PushGateWait);

    /// <summary>
    /// Single-shot push for test completion: no inter-push cooldown, longer gate wait.
    /// </summary>
    public static Task<NavigationRunStatus> RunCompletionPushAsync(Func<Task> navigate) =>
        RunCoreAsync(navigate, applyPushCooldown: false, waitForGate: true, gateWait: CompletionGateWait);

    private static async Task<NavigationRunStatus> RunCoreAsync(
        Func<Task> navigate,
        bool applyPushCooldown,
        bool waitForGate = false,
        TimeSpan? gateWait = null)
    {
        TimeSpan gateWaitDuration = gateWait ?? PushGateWait;

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
            ? await Gate.WaitAsync(gateWaitDuration).ConfigureAwait(false)
            : await Gate.WaitAsync(0).ConfigureAwait(false);

        if (!acquired)
        {
            string message = waitForGate
                ? "Navigation dropped: coordinator gate timeout."
                : "Navigation dropped: coordinator gate busy.";
            _logger?.LogWarning(message);
            return waitForGate ? NavigationRunStatus.DroppedTimeout : NavigationRunStatus.DroppedBusy;
        }

        try
        {
            await NavigationThread.InvokeAsync(navigate).ConfigureAwait(false);
            return NavigationRunStatus.Completed;
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Navigation operation failed.");
            return NavigationRunStatus.Failed;
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
