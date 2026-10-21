using PsychologyApp.Presentation.Features.ManageProfile;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>Saving the settings screen changes what it shows and keeps everything else as it was saved.</summary>
public sealed class SettingsPreferencesPresenterTests
{
    [Fact]
    public void What_the_settings_screen_does_not_edit_is_kept()
    {
        // Onboarding is not on the settings screen: saving a theme must not send the person back through it.
        UserPreferencesState saved = new() { HasCompletedOnboarding = true, Theme = "light" };

        UserPreferencesState built = new SettingsPreferencesPresenter().BuildState(
            "Русский", "Тёмная", "Синий", "Скруглённая", "Средний",
            isBold: false, questionnaireAutoAdvance: true,
            practiceRemindersEnabled: true, practiceReminderHour: 19,
            quoteRemindersEnabled: false, quoteReminderHour: 9,
            moodRemindersEnabled: false, moodReminderHour: 20,
            chatRemindersEnabled: false, chatReminderHour: 19,
            onboardingConcern: "explore",
            saved);

        Assert.True(built.HasCompletedOnboarding);
    }
}
