namespace PsychologyApp.Presentation.Services;

internal static class NavigationCoordinator
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static long _pushBlockedUntilUtcTicks;
    private static readonly TimeSpan PushCooldown = TimeSpan.FromMilliseconds(350);

    public static Task RunAsync(Func<Task> navigate) =>
        RunCoreAsync(navigate, applyPushCooldown: false);

    public static Task RunPushAsync(Func<Task> navigate) =>
        RunCoreAsync(navigate, applyPushCooldown: true);

    private static async Task RunCoreAsync(Func<Task> navigate, bool applyPushCooldown)
    {
        if (applyPushCooldown)
        {
            long now = DateTime.UtcNow.Ticks;
            if (now < Volatile.Read(ref _pushBlockedUntilUtcTicks))
            {
                return;
            }
        }

        if (!await Gate.WaitAsync(0).ConfigureAwait(false))
        {
            return;
        }

        try
        {
            await navigate().ConfigureAwait(false);
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
