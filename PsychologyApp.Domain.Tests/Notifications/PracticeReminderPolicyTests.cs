using PsychologyApp.Domain.Notifications;
using Xunit;

namespace PsychologyApp.Domain.Tests.Notifications;

public sealed class PracticeReminderPolicyTests
{
    private static readonly DateTime TodayMorning = new(2026, 6, 30, 10, 0, 0, DateTimeKind.Local);
    private static readonly DateTime TodayEvening = new(2026, 6, 30, 20, 0, 0, DateTimeKind.Local);
    private const int ReminderHour = 19;

    [Fact]
    public void ResolveNextFireLocal_WhenDisabled_ReturnsNull() =>
        Assert.Null(PracticeReminderPolicy.ResolveNextFireLocal(
            remindersEnabled: false,
            hasCompletedOnboarding: true,
            lastPracticeUtc: null,
            ReminderHour,
            TodayMorning));

    [Fact]
    public void ResolveNextFireLocal_WhenOnboardingIncomplete_ReturnsNull() =>
        Assert.Null(PracticeReminderPolicy.ResolveNextFireLocal(
            remindersEnabled: true,
            hasCompletedOnboarding: false,
            lastPracticeUtc: null,
            ReminderHour,
            TodayMorning));

    [Fact]
    public void ResolveNextFireLocal_BeforeReminderHourWithoutPracticeToday_ReturnsToday()
    {
        DateTime? fire = PracticeReminderPolicy.ResolveNextFireLocal(
            remindersEnabled: true,
            hasCompletedOnboarding: true,
            lastPracticeUtc: null,
            ReminderHour,
            TodayMorning);

        Assert.Equal(new DateTime(2026, 6, 30, 19, 0, 0, DateTimeKind.Local), fire);
    }

    [Fact]
    public void ResolveNextFireLocal_AfterReminderHourWithoutPracticeToday_ReturnsTomorrow()
    {
        DateTime? fire = PracticeReminderPolicy.ResolveNextFireLocal(
            remindersEnabled: true,
            hasCompletedOnboarding: true,
            lastPracticeUtc: null,
            ReminderHour,
            TodayEvening);

        Assert.Equal(new DateTime(2026, 7, 1, 19, 0, 0, DateTimeKind.Local), fire);
    }

    [Fact]
    public void ResolveNextFireLocal_PracticedToday_ReturnsTomorrow()
    {
        DateTime lastPracticeUtc = TodayMorning.ToUniversalTime();
        DateTime? fire = PracticeReminderPolicy.ResolveNextFireLocal(
            remindersEnabled: true,
            hasCompletedOnboarding: true,
            lastPracticeUtc: lastPracticeUtc,
            ReminderHour,
            TodayMorning);

        Assert.Equal(new DateTime(2026, 7, 1, 19, 0, 0, DateTimeKind.Local), fire);
    }

    [Fact]
    public void ClampHour_ClampsToAllowedRange()
    {
        Assert.Equal(8, PracticeReminderPolicy.ClampHour(0));
        Assert.Equal(22, PracticeReminderPolicy.ClampHour(99));
        Assert.Equal(19, PracticeReminderPolicy.ClampHour(19));
    }
}
