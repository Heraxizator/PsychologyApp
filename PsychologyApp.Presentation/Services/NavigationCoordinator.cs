namespace PsychologyApp.Presentation.Services;

internal static class NavigationCoordinator
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static long _blockedUntilUtcTicks;
    private static readonly TimeSpan Cooldown = TimeSpan.FromMilliseconds(600);

    public static async Task RunAsync(Func<Task> navigate)
    {
        long now = DateTime.UtcNow.Ticks;
        if (now < Volatile.Read(ref _blockedUntilUtcTicks))
        {
            return;
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
            Volatile.Write(ref _blockedUntilUtcTicks, DateTime.UtcNow.Add(Cooldown).Ticks);
            Gate.Release();
        }
    }
}
