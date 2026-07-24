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
        var coordinator = new MoodReminderCoordinator(store, scheduler);

        await coordinator.SyncAsync();

        Assert.True(scheduler.WasScheduled);
        Assert.False(scheduler.WasCancelled);
        Assert.Equal(AppStrings.MoodReminderTitle, scheduler.LastTitle);
        Assert.Equal(AppStrings.MoodReminderBody, scheduler.LastBody);
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
        var coordinator = new MoodReminderCoordinator(store, scheduler);

        await coordinator.SyncAsync();

        Assert.True(scheduler.WasCancelled);
        Assert.False(scheduler.WasScheduled);
    }

    [Fact]
    public void MoodReminderPrefs_RoundTripThroughUserPreferences()
    {
        UserPreferences.UseInMemoryStorage(new UserPreferencesState
        {
            HasCompletedOnboarding = true,
            MoodRemindersEnabled = false,
            MoodReminderHour = UserPreferences.DefaultMoodReminderHour
        });

        try
        {
            UserPreferencesState current = UserPreferences.Load();
            UserPreferences.Save(new UserPreferencesState
            {
                Language = current.Language,
                Theme = current.Theme,
                Color = current.Color,
                Form = current.Form,
                Size = current.Size,
                IsBold = current.IsBold,
                QuestionnaireAutoAdvance = current.QuestionnaireAutoAdvance,
                HasCompletedOnboarding = current.HasCompletedOnboarding,
                OnboardingConcern = current.OnboardingConcern,
                PracticeRemindersEnabled = current.PracticeRemindersEnabled,
                PracticeReminderHour = current.PracticeReminderHour,
                QuoteRemindersEnabled = current.QuoteRemindersEnabled,
                QuoteReminderHour = current.QuoteReminderHour,
                MoodRemindersEnabled = true,
                MoodReminderHour = 20
            });

            UserPreferencesState loaded = UserPreferences.Load();
            Assert.True(loaded.MoodRemindersEnabled);
            Assert.Equal(20, loaded.MoodReminderHour);
        }
        finally
        {
            UserPreferences.ResetInMemoryStorage();
        }
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

        public Task RequestPermissionIfNeededAsync(CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public void Schedule(DateTime fireAtLocal, string title, string body)
        {
            WasScheduled = true;
            LastTitle = title;
            LastBody = body;
        }

        public void Cancel() => WasCancelled = true;
    }
}
