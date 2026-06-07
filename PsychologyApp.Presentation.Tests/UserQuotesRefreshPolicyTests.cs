using PsychologyApp.Presentation.Infrastructure;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class UserQuotesRefreshPolicyTests
{
    [Theory]
    [InlineData(false, false, true)]
    [InlineData(false, true, true)]
    [InlineData(true, false, false)]
    [InlineData(true, true, true)]
    public void ShouldReload_ReturnsExpected(bool quotesLoadedOnce, bool forceReload, bool expected)
    {
        Assert.Equal(expected, UserQuotesRefreshPolicy.ShouldReload(quotesLoadedOnce, forceReload));
    }
}
