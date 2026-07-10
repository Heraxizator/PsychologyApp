using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Features.Onboarding;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingViewModel
{
    private bool _practiceRemindersEnabled = true;
    public bool PracticeRemindersEnabled
    {
        get => _practiceRemindersEnabled;
        set => SetProperty(ref _practiceRemindersEnabled, value);
    }

    private int _practiceReminderHour = UserPreferences.DefaultPracticeReminderHour;
    public string PracticeReminderHour
    {
        get => UserPreferences.GetPracticeReminderHourLabel(_practiceReminderHour);
        set
        {
            int normalized = UserPreferences.ParsePracticeReminderHourKey(value);
            if (_practiceReminderHour == normalized)
            {
                return;
            }

            _practiceReminderHour = normalized;
            OnPropertyChanged(nameof(PracticeReminderHour));
        }
    }

    public IReadOnlyList<string> PracticeReminderHourOptions =>
        UserPreferences.GetPracticeReminderHourOptions();

    public string OnboardingRemindersLabel => AppStrings.OnboardingRemindersLabel;
    public string OnboardingReminderHourLabel => AppStrings.OnboardingReminderHourLabel;
    public string OnboardingReminderHourPickerTitle => AppStrings.SettingsPracticeReminderHourPickerTitle;

    private async Task StartPracticeAsync()
    {
        OnboardingRecommendationResult recommendation =
            await _onboardingRecommendationResolver.ResolveAsync(SelectedConcern);
        PersistOnboardingCompletion(recommendation.Concern);
        await _onCompleted(recommendation.TechniqueId);
    }

    private async Task CompleteWithoutPracticeAsync()
    {
        OnboardingRecommendationResult recommendation =
            await _onboardingRecommendationResolver.ResolveAsync(SelectedConcern);
        PersistOnboardingCompletion(recommendation.Concern);
        await _onCompleted(null);
    }

    private void PersistOnboardingCompletion(string concern) =>
        _userPreferencesStore.CompleteOnboarding(
            concern,
            PracticeRemindersEnabled,
            _practiceReminderHour);
}
