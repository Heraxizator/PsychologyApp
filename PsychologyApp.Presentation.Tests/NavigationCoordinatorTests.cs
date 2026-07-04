using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PsychologyApp.Presentation.Shared.Navigation;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class NavigationCoordinatorTests : IDisposable
{
    public NavigationCoordinatorTests() => NavigationCoordinator.ResetForTests();

    public void Dispose()
    {
        NavigationCoordinator.ResetForTests();
        NavigationCoordinator.SetLogger(NullLogger.Instance);
    }

    [Fact]
    public async Task RunPushAsync_ExecutesNavigationDelegate()
    {
        bool executed = false;

        NavigationRunStatus status = await NavigationCoordinator.RunPushAsync(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        Assert.True(executed);
        Assert.Equal(NavigationRunStatus.Completed, status);
    }

    [Fact]
    public async Task RunAsync_ExecutesNavigationDelegate()
    {
        bool executed = false;

        await NavigationCoordinator.RunAsync(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        Assert.True(executed);
    }

    [Fact]
    public async Task RunPushAsync_WhenGateBusy_ReturnsDroppedTimeout()
    {
        Mock<ILogger> logger = new();
        NavigationCoordinator.SetLogger(logger.Object);
        TaskCompletionSource<bool> holdGate = new();
        TaskCompletionSource gateAcquired = new();

        Task first = NavigationCoordinator.RunPushAsync(async () =>
        {
            gateAcquired.SetResult();
            await holdGate.Task;
        });

        await gateAcquired.Task;

        NavigationRunStatus status = await NavigationCoordinator.RunPushAsync(() => Task.CompletedTask);

        Assert.Equal(NavigationRunStatus.DroppedTimeout, status);
        logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("gate timeout", StringComparison.OrdinalIgnoreCase)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        holdGate.SetResult(true);
        await first;
    }

    [Fact]
    public async Task RunPushAsync_WhenNavigationThrows_ReturnsFailed()
    {
        Mock<ILogger> logger = new();
        NavigationCoordinator.SetLogger(logger.Object);

        NavigationRunStatus status = await NavigationCoordinator.RunPushAsync(() =>
            Task.FromException(new InvalidOperationException("Push failed")));

        Assert.Equal(NavigationRunStatus.Failed, status);
    }

    [Fact]
    public async Task RunAsync_WhenGateBusyWithoutWait_SkipsNavigation()
    {
        Mock<ILogger> logger = new();
        NavigationCoordinator.SetLogger(logger.Object);
        TaskCompletionSource<bool> holdGate = new();
        TaskCompletionSource gateAcquired = new();

        Task first = NavigationCoordinator.RunPushAsync(async () =>
        {
            gateAcquired.SetResult();
            await holdGate.Task;
        });

        await gateAcquired.Task;

        bool secondExecuted = false;
        await NavigationCoordinator.RunAsync(() =>
        {
            secondExecuted = true;
            return Task.CompletedTask;
        });

        Assert.False(secondExecuted);
        logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("gate busy", StringComparison.OrdinalIgnoreCase)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        holdGate.SetResult(true);
        await first;
    }
}
