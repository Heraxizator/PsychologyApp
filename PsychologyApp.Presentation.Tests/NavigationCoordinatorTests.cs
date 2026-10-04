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
        }, "First");

        await gateAcquired.Task;

        NavigationRunStatus status = await NavigationCoordinator.RunPushAsync(() => Task.CompletedTask, "Second");

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
        }, "First");

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
    [Fact]
    public async Task RunPushAsync_DoubleTapWhileFirstIsOpening_OpensThePageOnce()
    {
        TaskCompletionSource release = new();
        TaskCompletionSource firstStarted = new();
        int opened = 0;

        Task<NavigationRunStatus> first = NavigationCoordinator.RunPushAsync(async () =>
        {
            Interlocked.Increment(ref opened);
            firstStarted.SetResult();
            await release.Task;
        }, "Profile");
        await firstStarted.Task;

        NavigationRunStatus second = await NavigationCoordinator.RunPushAsync(() =>
        {
            Interlocked.Increment(ref opened);
            return Task.CompletedTask;
        }, "Profile");
        release.SetResult();

        Assert.Equal(NavigationRunStatus.DroppedDuplicate, second);
        Assert.Equal(NavigationRunStatus.Completed, await first);
        Assert.Equal(1, opened);
    }

    [Fact]
    public async Task RunPushAsync_SecondTapRightAfterTheFirstOpened_IsDropped()
    {
        int opened = 0;
        Task Open()
        {
            opened++;
            return Task.CompletedTask;
        }

        Assert.Equal(NavigationRunStatus.Completed, await NavigationCoordinator.RunPushAsync(Open, "Profile"));
        Assert.Equal(NavigationRunStatus.DroppedDuplicate, await NavigationCoordinator.RunPushAsync(Open, "Profile"));

        Assert.Equal(1, opened);
    }

    [Fact]
    public async Task RunPushAsync_ManyTapsAtOnce_OpenTheSamePageOnce()
    {
        int opened = 0;
        TaskCompletionSource release = new();

        Task<NavigationRunStatus>[] taps = Enumerable.Range(0, 10)
            .Select(_ => NavigationCoordinator.RunPushAsync(async () =>
            {
                Interlocked.Increment(ref opened);
                await release.Task;
            }, "Journal"))
            .ToArray();
        release.SetResult();
        NavigationRunStatus[] results = await Task.WhenAll(taps);

        Assert.Equal(1, opened);
        Assert.Equal(1, results.Count(status => status == NavigationRunStatus.Completed));
        Assert.Equal(9, results.Count(status => status == NavigationRunStatus.DroppedDuplicate));
    }

    [Fact]
    public async Task RunPushAsync_DifferentDestinations_AreBothOpenedInOrder()
    {
        List<string> opened = [];

        NavigationRunStatus first = await NavigationCoordinator.RunPushAsync(() => { opened.Add("Question"); return Task.CompletedTask; }, "Question");
        NavigationRunStatus second = await NavigationCoordinator.RunPushAsync(() => { opened.Add("Result"); return Task.CompletedTask; }, "Result");

        Assert.Equal(NavigationRunStatus.Completed, first);
        Assert.Equal(NavigationRunStatus.Completed, second);
        Assert.Equal(["Question", "Result"], opened);
    }

    [Fact]
    public async Task RunPushAsync_AfterAFailedPush_TheRetryIsNotBlocked()
    {
        NavigationRunStatus failed = await NavigationCoordinator.RunPushAsync(
            () => Task.FromException(new InvalidOperationException("boom")), "Profile");
        bool retried = false;

        NavigationRunStatus retry = await NavigationCoordinator.RunPushAsync(() => { retried = true; return Task.CompletedTask; }, "Profile");

        Assert.Equal(NavigationRunStatus.Failed, failed);
        Assert.Equal(NavigationRunStatus.Completed, retry);
        Assert.True(retried);
    }

    [Fact]
    public async Task RunPushAsync_AfterTheDuplicateWindow_OpensTheSamePageAgain()
    {
        int opened = 0;
        Task Open()
        {
            opened++;
            return Task.CompletedTask;
        }

        await NavigationCoordinator.RunPushAsync(Open, "Profile");
        await Task.Delay(750);
        NavigationRunStatus again = await NavigationCoordinator.RunPushAsync(Open, "Profile");

        Assert.Equal(NavigationRunStatus.Completed, again);
        Assert.Equal(2, opened);
    }

    [Fact]
    public async Task RunCompletionPushAsync_DoubleTap_OpensTheResultOnce()
    {
        int opened = 0;
        Task Open()
        {
            opened++;
            return Task.CompletedTask;
        }

        await NavigationCoordinator.RunCompletionPushAsync(Open, "TestResult");
        NavigationRunStatus second = await NavigationCoordinator.RunCompletionPushAsync(Open, "TestResult");

        Assert.Equal(NavigationRunStatus.DroppedDuplicate, second);
        Assert.Equal(1, opened);
    }

    [Fact]
    public async Task RunPushAsync_WithoutADestination_IsNotDeduplicated()
    {
        int opened = 0;
        Task Open()
        {
            opened++;
            return Task.CompletedTask;
        }

        await NavigationCoordinator.RunPushAsync(Open, destination: null);
        await NavigationCoordinator.RunPushAsync(Open, destination: null);

        Assert.Equal(2, opened);
    }
}
