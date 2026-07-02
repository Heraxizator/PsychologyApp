using PsychologyApp.Presentation.Shared.Navigation;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class NavigationRetryTests
{
    [Fact]
    public async Task TryExecuteWithRetryAsync_ReturnsTrue_WhenFirstAttemptSucceeds()
    {
        int attempts = 0;

        bool completed = await NavigationRetry.TryExecuteWithRetryAsync(
            () =>
            {
                attempts++;
                return Task.FromResult(NavigationRunStatus.Completed);
            },
            status => status == NavigationRunStatus.Completed,
            maxAttempts: 3,
            delay: TimeSpan.FromMilliseconds(1));

        Assert.True(completed);
        Assert.Equal(1, attempts);
    }

    [Fact]
    public async Task TryExecuteWithRetryAsync_RetriesUntilCompleted()
    {
        int attempts = 0;

        bool completed = await NavigationRetry.TryExecuteWithRetryAsync(
            () =>
            {
                attempts++;
                return Task.FromResult(
                    attempts >= 2
                        ? NavigationRunStatus.Completed
                        : NavigationRunStatus.DroppedTimeout);
            },
            status => status == NavigationRunStatus.Completed,
            maxAttempts: 3,
            delay: TimeSpan.FromMilliseconds(1));

        Assert.True(completed);
        Assert.Equal(2, attempts);
    }

    [Fact]
    public async Task TryExecuteWithRetryAsync_ReturnsFalse_WhenAllAttemptsDrop()
    {
        int attempts = 0;

        bool completed = await NavigationRetry.TryExecuteWithRetryAsync(
            () =>
            {
                attempts++;
                return Task.FromResult(NavigationRunStatus.DroppedTimeout);
            },
            status => status == NavigationRunStatus.Completed,
            maxAttempts: 3,
            delay: TimeSpan.FromMilliseconds(1));

        Assert.False(completed);
        Assert.Equal(3, attempts);
    }
}
