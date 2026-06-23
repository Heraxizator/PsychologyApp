using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Core.Common;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingViewModel
{
    public string WelcomeTitle => AppStrings.OnboardingWelcomeTitle;
    public string WelcomeBody => AppStrings.OnboardingWelcomeBody;
    public string ConcernTitle => AppStrings.OnboardingConcernTitle;
    public string DisclaimerTitle => AppStrings.OnboardingDisclaimerTitle;
    public string DisclaimerBody => AppStrings.OnboardingDisclaimerBody;
    public string StartLabel => AppStrings.OnboardingStart;
    public string SkipLabel => AppStrings.OnboardingSkip;
    public string NextLabel => AppStrings.OnboardingNext;
    public string ConcernAnxiety => AppStrings.OnboardingConcernAnxiety;
    public string ConcernBody => AppStrings.OnboardingConcernBody;
    public string ConcernMood => AppStrings.OnboardingConcernMood;
    public string ConcernExplore => AppStrings.OnboardingConcernExplore;

    public string ConcernAnxietyVariant => SelectedConcern == OnboardingConcernKeys.Anxiety ? "Primary" : "Secondary";
    public string ConcernBodyVariant => SelectedConcern == OnboardingConcernKeys.Body ? "Primary" : "Secondary";
    public string ConcernMoodVariant => SelectedConcern == OnboardingConcernKeys.Mood ? "Primary" : "Secondary";
    public string ConcernExploreVariant => SelectedConcern == OnboardingConcernKeys.Explore ? "Primary" : "Secondary";

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(WelcomeTitle),
            nameof(WelcomeBody),
            nameof(ConcernTitle),
            nameof(DisclaimerTitle),
            nameof(DisclaimerBody),
            nameof(StartLabel),
            nameof(SkipLabel),
            nameof(NextLabel),
            nameof(ConcernAnxiety),
            nameof(ConcernBody),
            nameof(ConcernMood),
            nameof(ConcernExplore));
        NotifyConcernSelection();
    }

    private void NotifyConcernSelection()
    {
        Notify(
            nameof(ConcernAnxietyVariant),
            nameof(ConcernBodyVariant),
            nameof(ConcernMoodVariant),
            nameof(ConcernExploreVariant));
    }
}
