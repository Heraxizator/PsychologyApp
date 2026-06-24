using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Core.Common;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingViewModel
{
    public string WelcomeTitle => AppStrings.OnboardingWelcomeTitle;
    public string WelcomeBody => AppStrings.OnboardingWelcomeBody;
    public string AppName => AppStrings.QuoteShareFooter;
    public string OverviewTitle => AppStrings.OnboardingOverviewTitle;
    public string OverviewSubtitle => AppStrings.OnboardingOverviewSubtitle;
    public string ConcernTitle => AppStrings.OnboardingConcernTitle;
    public string ConcernSubtitle => AppStrings.OnboardingConcernSubtitle;
    public string FinishTitle => AppStrings.OnboardingFinishTitle;
    public string FinishSubtitle => AppStrings.OnboardingFinishSubtitle(RecommendedTitle);
    public string RecommendedCaption => AppStrings.OnboardingRecommendedCaption;
    public string DisclaimerTitle => AppStrings.OnboardingDisclaimerTitle;
    public string DisclaimerBody => AppStrings.OnboardingDisclaimerBody;
    public string StartLabel => AppStrings.OnboardingStart;
    public string SkipLabel => AppStrings.OnboardingSkip;
    public string NextLabel => AppStrings.OnboardingNext;
    public string BackLabel => AppStrings.OnboardingBack;
    public string StepLabel => AppStrings.OnboardingStepOf(Step + 1, StepCount);

    public string ModulePracticeTitle => AppStrings.ShellTabPracticeShort;
    public string ModulePracticeHint => AppStrings.OnboardingModulePracticeHint;
    public string ModuleTestsTitle => AppStrings.ShellTabDetectorShort;
    public string ModuleTestsHint => AppStrings.OnboardingModuleTestsHint;
    public string ModuleSomaticTitle => AppStrings.ShellTabSomaticShort;
    public string ModuleSomaticHint => AppStrings.OnboardingModuleSomaticHint;
    public string ModuleMusicTitle => AppStrings.ShellTabMusicShort;
    public string ModuleMusicHint => AppStrings.OnboardingModuleMusicHint;
    public string ModuleQuotesTitle => AppStrings.ShellTabMotivatorShort;
    public string ModuleQuotesHint => AppStrings.OnboardingModuleQuotesHint;

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
            nameof(AppName),
            nameof(OverviewTitle),
            nameof(OverviewSubtitle),
            nameof(ConcernTitle),
            nameof(ConcernSubtitle),
            nameof(FinishTitle),
            nameof(FinishSubtitle),
            nameof(RecommendedCaption),
            nameof(DisclaimerTitle),
            nameof(DisclaimerBody),
            nameof(StartLabel),
            nameof(SkipLabel),
            nameof(NextLabel),
            nameof(BackLabel),
            nameof(StepLabel),
            nameof(ModulePracticeTitle),
            nameof(ModulePracticeHint),
            nameof(ModuleTestsTitle),
            nameof(ModuleTestsHint),
            nameof(ModuleSomaticTitle),
            nameof(ModuleSomaticHint),
            nameof(ModuleMusicTitle),
            nameof(ModuleMusicHint),
            nameof(ModuleQuotesTitle),
            nameof(ModuleQuotesHint),
            nameof(ConcernAnxiety),
            nameof(ConcernBody),
            nameof(ConcernMood),
            nameof(ConcernExplore));
        NotifyConcernSelection();
        NotifyRecommendation();
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
