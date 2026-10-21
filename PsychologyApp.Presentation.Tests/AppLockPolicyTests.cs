using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>When the app lock asks for the phone's unlock: on every start, and after a real absence, not after the share sheet.</summary>
public sealed class AppLockPolicyTests
{
    [Fact]
    public void A_start_of_the_app_asks_when_the_lock_is_on() =>
        Assert.True(AppLockPolicy.NeedsUnlock(enabled: true, unlockedThisRun: false, awayFor: null));

    [Fact]
    public void Without_the_lock_nothing_is_asked() =>
        Assert.False(AppLockPolicy.NeedsUnlock(enabled: false, unlockedThisRun: false, awayFor: TimeSpan.FromHours(5)));

    [Fact]
    public void A_short_trip_to_the_share_sheet_or_a_call_does_not_lock_again() =>
        Assert.False(AppLockPolicy.NeedsUnlock(enabled: true, unlockedThisRun: true, awayFor: TimeSpan.FromSeconds(40)));

    [Fact]
    public void Coming_back_after_a_while_asks_again() =>
        Assert.True(AppLockPolicy.NeedsUnlock(enabled: true, unlockedThisRun: true, awayFor: AppLockPolicy.RelockAfter));

    [Fact]
    public void Staying_in_the_app_never_asks_again() =>
        Assert.False(AppLockPolicy.NeedsUnlock(enabled: true, unlockedThisRun: true, awayFor: null));
}
