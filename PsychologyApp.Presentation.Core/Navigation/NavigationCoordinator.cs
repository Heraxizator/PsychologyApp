using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace PsychologyApp.Presentation.Shared.Navigation;

public static class NavigationCoordinator
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static long _pushBlockedUntilUtcTicks;
    private static readonly TimeSpan PushCooldown = TimeSpan.FromMilliseconds(350);
    private static ILogger? _logger;

    /// <summary>How long after a page opened a call for the same destination still counts as the same tap.</summary>
    private static readonly TimeSpan DuplicateWindow = TimeSpan.FromMilliseconds(600);

    private static readonly object DuplicateLock = new();
    private static readonly HashSet<string> InFlight = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, long> LastOpenedTicks = new(StringComparer.Ordinal);

    private static readonly TimeSpan PushGateWait = TimeSpan.FromSeconds(4);
    private static readonly TimeSpan CompletionGateWait = TimeSpan.FromSeconds(8);

    public static void SetLogger(ILogger logger) => _logger = logger;

    internal static void ResetForTests()
    {
        while (Gate.CurrentCount == 0)
        {
            try
            {
                Gate.Release();
            }
            catch (SemaphoreFullException)
            {
                break;
            }
        }

        while (Gate.CurrentCount > 1)
        {
            Gate.Wait(0);
        }

        Volatile.Write(ref _pushBlockedUntilUtcTicks, 0);

        lock (DuplicateLock)
        {
            InFlight.Clear();
            LastOpenedTicks.Clear();
        }
    }

    public static void LogNavigationFailure(Exception exception, string message) =>
        _logger?.LogWarning(exception, message);

    public static async Task RunAsync(Func<Task> navigate)
    {
        _ = await RunCoreAsync(navigate, applyPushCooldown: false).ConfigureAwait(false);
    }

    /// <summary>
    /// Opens a page. <paramref name="destination"/> names where it goes (by default the calling method, e.g. "GoToUserProfileAsync");
    /// a second call for the same destination while the first is still opening, or within <see cref="DuplicateWindow"/> after it
    /// opened, is a double tap and is dropped. Without this the gate only *queued* it, and the page opened twice, one after the other.
    /// Calls for different destinations are still queued in order (code that pushes two pages in a row keeps working).
    /// </summary>
    public static Task<NavigationRunStatus> RunPushAsync(Func<Task> navigate, [CallerMemberName] string? destination = null) =>
        RunGuardedAsync(
            destination,
            () => RunCoreAsync(navigate, applyPushCooldown: true, waitForGate: true, gateWait: PushGateWait));

    /// <summary>
    /// Single-shot push for test completion: no inter-push cooldown, longer gate wait, same double-tap guard.
    /// </summary>
    public static Task<NavigationRunStatus> RunCompletionPushAsync(Func<Task> navigate, [CallerMemberName] string? destination = null) =>
        RunGuardedAsync(
            destination,
            () => RunCoreAsync(navigate, applyPushCooldown: false, waitForGate: true, gateWait: CompletionGateWait));

    private static async Task<NavigationRunStatus> RunGuardedAsync(string? destination, Func<Task<NavigationRunStatus>> run)
    {
        if (destination is null)
        {
            return await run().ConfigureAwait(false);
        }

        if (!TryBeginPush(destination))
        {
            _logger?.LogInformation("Navigation dropped: {Destination} is already opening (double tap).", destination);
            return NavigationRunStatus.DroppedDuplicate;
        }

        NavigationRunStatus status = NavigationRunStatus.Failed;
        try
        {
            status = await run().ConfigureAwait(false);
            return status;
        }
        finally
        {
            EndPush(destination, opened: status == NavigationRunStatus.Completed);
        }
    }

    private static bool TryBeginPush(string destination)
    {
        lock (DuplicateLock)
        {
            if (InFlight.Contains(destination))
            {
                return false;
            }

            if (LastOpenedTicks.TryGetValue(destination, out long openedAt)
                && Environment.TickCount64 - openedAt < DuplicateWindow.TotalMilliseconds)
            {
                return false;
            }

            InFlight.Add(destination);
            return true;
        }
    }

    private static void EndPush(string destination, bool opened)
    {
        lock (DuplicateLock)
        {
            InFlight.Remove(destination);

            // A failed or dropped push must not block the retry that follows it.
            if (opened)
            {
                LastOpenedTicks[destination] = Environment.TickCount64;
            }
            else
            {
                LastOpenedTicks.Remove(destination);
            }
        }
    }

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
