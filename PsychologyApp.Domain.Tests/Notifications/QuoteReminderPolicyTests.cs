using PsychologyApp.Domain.Notifications;
using Xunit;

namespace PsychologyApp.Domain.Tests.Notifications;

public sealed class QuoteReminderPolicyTests
{
    [Fact]
    public void ResolveNextFireLocal_ReturnsNullWhenDisabled()
    {
        Assert.Null(QuoteReminderPolicy.ResolveNextFireLocal(
            remindersEnabled: false,
            hasCompletedOnboarding: true,
            reminderHour: 9,
            nowLocal: new DateTime(2026, 7, 4, 10, 0, 0)));
    }

    [Fact]
    public void ResolveNextFireLocal_ReturnsTodayWhenBeforeHour()
    {
        DateTime? fire = QuoteReminderPolicy.ResolveNextFireLocal(
            remindersEnabled: true,
            hasCompletedOnboarding: true,
            reminderHour: 19,
            nowLocal: new DateTime(2026, 7, 4, 10, 0, 0));

        Assert.Equal(new DateTime(2026, 7, 4, 19, 0, 0), fire);
    }
}
