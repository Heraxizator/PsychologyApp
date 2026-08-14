using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class UserQuotesRefreshPolicyTests
{
    [Fact]
    public void ShouldReload_ReturnsTrue_WhenNeverLoaded()
    {
        Assert.True(UserQuotesRefreshPolicy.ShouldReload(quotesLoadedOnce: false, forceReload: false));
    }

    [Fact]
    public void ShouldReload_ReturnsFalse_WhenAlreadyLoadedWithoutForce()
    {
        Assert.False(UserQuotesRefreshPolicy.ShouldReload(quotesLoadedOnce: true, forceReload: false));
    }

    [Fact]
    public void ShouldReload_ReturnsTrue_WhenForceReloadRequested()
    {
        Assert.True(UserQuotesRefreshPolicy.ShouldReload(quotesLoadedOnce: true, forceReload: true));
    }
}
