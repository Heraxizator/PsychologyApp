using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class MoodReminderCoordinatorTests
{
    [Fact]
    public async Task SyncAsync_SchedulesWhenEnabledAndOnboarded()
    {
        var store = new InMemoryUserPreferencesStore();
        store.Save(new UserPreferencesState
        {
            HasCompletedOnboarding = true,
            MoodRemindersEnabled = true,
            MoodReminderHour = 20
        });

        var scheduler = new RecordingMoodReminderScheduler();
        var coordinator = new MoodReminderCoordinator(new Mock<IUserProgressService>().Object, store, scheduler);

        await coordinator.SyncAsync();

        Assert.True(scheduler.WasScheduled);
        Assert.False(scheduler.WasCancelled);
        Assert.Equal(AppStrings.MoodReminderTitle, scheduler.LastTitle);
        Assert.Equal(AppStrings.MoodReminderBody, scheduler.LastBody);
    }

    [Fact]
    public async Task SyncAsync_AutomaticHourFollowsUsualCheckIns()
    {
        var store = new InMemoryUserPreferencesStore();
        store.Save(new UserPreferencesState
        {
            HasCompletedOnboarding = true,
            MoodRemindersEnabled = true,
            MoodReminderHour = UserPreferences.MoodReminderAutoHour
        });
        // Five past evenings at 21:xx local time, none today, so today's reminder is not covered.
        MoodEntryDTO[] moods = [.. Enumerable.Range(1, 5).Select(daysAgo => new MoodEntryDTO
        {
            MoodLevel = 3,
            RecordedAt = DateTime.Today.AddDays(-daysAgo).AddHours(21.25).ToUniversalTime()
        })];
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(service => service.GetMoodsAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(moods);
        var scheduler = new RecordingMoodReminderScheduler();

        await new MoodReminderCoordinator(progress.Object, store, scheduler).SyncAsync();

        Assert.Equal(21, scheduler.LastFireAt?.Hour);
    }

    [Fact]
    public async Task SyncAsync_CancelsWhenDisabled()
    {
        var store = new InMemoryUserPreferencesStore();
        store.Save(new UserPreferencesState
        {
            HasCompletedOnboarding = true,
            MoodRemindersEnabled = false
        });

        var scheduler = new RecordingMoodReminderScheduler();
        var coordinator = new MoodReminderCoordinator(new Mock<IUserProgressService>().Object, store, scheduler);

        await coordinator.SyncAsync();

        Assert.True(scheduler.WasCancelled);
        Assert.False(scheduler.WasScheduled);
    }

    [Fact]
    public void SettingsPresenter_BuildState_PersistsMoodReminderFields()
    {
        var presenter = new PsychologyApp.Presentation.Features.ManageProfile.SettingsPreferencesPresenter();
        UserPreferencesState saved = new()
        {
            HasCompletedOnboarding = true,
            MoodRemindersEnabled = false,
            MoodReminderHour = 9
        };

        UserPreferencesState built = presenter.BuildState(
            UserPreferences.DefaultLanguage,
            UserPreferences.DefaultTheme,
            UserPreferences.DefaultColor,
            UserPreferences.DefaultForm,
            UserPreferences.DefaultSize,
            isBold: false,
            questionnaireAutoAdvance: true,
            practiceRemindersEnabled: true,
            practiceReminderHour: UserPreferences.DefaultPracticeReminderHour,
            quoteRemindersEnabled: false,
            quoteReminderHour: UserPreferences.DefaultQuoteReminderHour,
            moodRemindersEnabled: true,
            moodReminderHour: 20,
            chatRemindersEnabled: false,
            chatReminderHour: UserPreferences.DefaultChatReminderHour,
            onboardingConcern: OnboardingConcernKeys.Anxiety,
            saved);

        Assert.True(built.MoodRemindersEnabled);
        Assert.Equal(20, built.MoodReminderHour);
    }

    private sealed class RecordingMoodReminderScheduler : IMoodReminderScheduler
    {
        public bool IsSupported => true;
        public bool WasScheduled { get; private set; }
        public bool WasCancelled { get; private set; }
        public string? LastTitle { get; private set; }
        public string? LastBody { get; private set; }
        public DateTime? LastFireAt { get; private set; }

        public Task RequestPermissionIfNeededAsync(CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public void Schedule(DateTime fireAtLocal, string title, string body)
        {
            WasScheduled = true;
            LastFireAt = fireAtLocal;
            LastTitle = title;
            LastBody = body;
        }

        public void Cancel() => WasCancelled = true;
    }
}
