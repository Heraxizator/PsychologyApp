namespace PsychologyApp.Presentation.Shared.Navigation;

public static class NavigationRetry
{
    public static async Task<bool> TryExecuteWithRetryAsync<TStatus>(
        Func<Task<TStatus>> operation,
        Func<TStatus, bool> isCompleted,
        int maxAttempts = 3,
        TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
        where TStatus : struct, Enum
    {
        delay ??= TimeSpan.FromMilliseconds(150);

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            TStatus status = await operation().ConfigureAwait(false);
            if (isCompleted(status))
            {
                return true;
            }

            if (attempt < maxAttempts)
            {
                await Task.Delay(delay.Value, cancellationToken).ConfigureAwait(false);
            }
        }

        return false;
    }
}
