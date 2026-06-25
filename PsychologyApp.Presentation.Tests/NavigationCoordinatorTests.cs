using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PsychologyApp.Presentation.Shared.Navigation;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class NavigationCoordinatorTests
{
    [Fact]
    public async Task RunPushAsync_ExecutesNavigationDelegate()
    {
        bool executed = false;

        await NavigationCoordinator.RunPushAsync(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        Assert.True(executed);
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
    public async Task RunAsync_WhenGateBusy_LogsDroppedNavigation()
    {
        Mock<ILogger> logger = new();
        NavigationCoordinator.SetLogger(logger.Object);
        TaskCompletionSource<bool> holdGate = new();

        try
        {
            Task first = NavigationCoordinator.RunAsync(async () => await holdGate.Task);
            await Task.Delay(50);

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
        finally
        {
            NavigationCoordinator.SetLogger(NullLogger.Instance);
        }
    }
}
