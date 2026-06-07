using PsychologyApp.Presentation.Infrastructure;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public class UiAnimationsTests
{
    [Fact]
    public void Constants_MatchUiKitDurations()
    {
        Assert.Equal(200u, UiAnimations.MicroDuration);
        Assert.Equal(300u, UiAnimations.MediumDuration);
        Assert.Equal(120u, UiAnimations.FastDuration);
        Assert.Equal(50u, UiAnimations.StaggerDelay);
        Assert.Equal(0.97, UiAnimations.PressScale);
        Assert.Equal(14, UiAnimations.SlideOffset);
        Assert.Equal(10, UiAnimations.StaggerCap);
    }

    [Fact]
    public void CanAnimate_WithNullView_ReturnsFalse()
    {
        Assert.False(UiAnimations.CanAnimate(null));
    }

    [Fact]
    public void ComputeRevealDelay_IsBoundedByCap()
    {
        int delay = UiAnimations.ComputeRevealDelay(99, cap: 10);
        Assert.Equal(9 * (int)UiAnimations.StaggerDelay, delay);
    }

    [Fact]
    public void ComputeRevealDelay_IncreasesWithIndex()
    {
        int first = UiAnimations.ComputeRevealDelay(0);
        int second = UiAnimations.ComputeRevealDelay(1);
        Assert.True(second > first);
    }

    [Fact]
    public void ComputeRevealDelay_WithNegativeIndex_ReturnsZero()
    {
        Assert.Equal(0, UiAnimations.ComputeRevealDelay(-1));
    }

    [Fact]
    public async Task FadeInAsync_WithNullView_CompletesWithoutException()
    {
        Task task = UiAnimations.FadeInAsync(null);
        await task;
        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task SafeRevealAsync_WithNullView_CompletesWithoutException()
    {
        Task task = UiAnimations.SafeRevealAsync(null);
        await task;
        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task SafeRevealPremiumAsync_WithNullView_CompletesWithoutException()
    {
        Task task = UiAnimations.SafeRevealPremiumAsync(null);
        await task;
        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task RevealPremiumAsync_WithNullView_CompletesWithoutException()
    {
        Task task = UiAnimations.RevealPremiumAsync(null);
        await task;
        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task SlideInAsync_WithNullView_CompletesWithoutException()
    {
        Task task = UiAnimations.SlideInAsync(null);
        await task;
        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task PressScaleAsync_WithNullView_CompletesWithoutException()
    {
        Task task = UiAnimations.PressScaleAsync(null);
        await task;
        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task RevealChildrenStaggeredAsync_WithNullLayout_CompletesWithoutException()
    {
        Task task = UiAnimations.RevealChildrenStaggeredAsync(null);
        await task;
        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task CrossfadeAsync_WithNullViews_CompletesWithoutException()
    {
        Task task = UiAnimations.CrossfadeAsync(null, null);
        await task;
        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task HideAsync_WithNullView_CompletesWithoutException()
    {
        Task task = UiAnimations.HideAsync(null);
        await task;
        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task SafeHideAsync_WithNullView_CompletesWithoutException()
    {
        Task task = UiAnimations.SafeHideAsync(null);
        await task;
        Assert.True(task.IsCompletedSuccessfully);
    }
}
