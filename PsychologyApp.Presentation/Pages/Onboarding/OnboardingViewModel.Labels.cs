using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingViewModel
{
    public string WelcomeTitle => AppStrings.OnboardingWelcomeTitle;
    public string WelcomeBody => AppStrings.OnboardingWelcomeBody;
    public string AppName => AppStrings.OnboardingAppName;
    public string AppTagline => AppStrings.OnboardingAppTagline;
    public string ValueOfflineLabel => AppStrings.OnboardingValueOffline;
    public string ValueNoJudgmentLabel => AppStrings.OnboardingValueNoJudgment;
    public string ValueOnDeviceLabel => AppStrings.OnboardingValueOnDevice;
    public string OverviewTitle => AppStrings.OnboardingOverviewTitle;
    public string OverviewSubtitle => AppStrings.OnboardingOverviewSubtitle;
    public string OverviewLead => AppStrings.OnboardingOverviewLead;
    public string ConcernTitle => AppStrings.OnboardingConcernTitle;
    public string ConcernSubtitle => AppStrings.OnboardingConcernSubtitle;
    public string ConcernFooterHint => AppStrings.OnboardingConcernFooterHint;
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
    public string ConcernAnxietyHint => AppStrings.OnboardingConcernAnxietyHint;
    public string ConcernBodyHint => AppStrings.OnboardingConcernBodyHint;
    public string ConcernMoodHint => AppStrings.OnboardingConcernMoodHint;
    public string ConcernExploreHint => AppStrings.OnboardingConcernExploreHint;

    public string ConcernAnxietyIcon => "Psychology";
    public string ConcernBodyIcon => "MonitorHeart";
    public string ConcernMoodIcon => "SentimentSatisfied";
    public string ConcernExploreIcon => "Explore";

    public bool ConcernAnxietyIsSelected => SelectedConcern == OnboardingConcernKeys.Anxiety;
    public bool ConcernBodyIsSelected => SelectedConcern == OnboardingConcernKeys.Body;
    public bool ConcernMoodIsSelected => SelectedConcern == OnboardingConcernKeys.Mood;
    public bool ConcernExploreIsSelected => SelectedConcern == OnboardingConcernKeys.Explore;

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
            nameof(AppTagline),
            nameof(ValueOfflineLabel),
            nameof(ValueNoJudgmentLabel),
            nameof(ValueOnDeviceLabel),
            nameof(OverviewTitle),
            nameof(OverviewSubtitle),
            nameof(OverviewLead),
            nameof(ConcernTitle),
            nameof(ConcernSubtitle),
            nameof(ConcernFooterHint),
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
            nameof(ConcernExplore),
            nameof(ConcernAnxietyHint),
            nameof(ConcernBodyHint),
            nameof(ConcernMoodHint),
            nameof(ConcernExploreHint));
        NotifyConcernSelection();
        NotifyRecommendation();
    }

    private void NotifyConcernSelection()
    {
        Notify(
            nameof(ConcernAnxietyVariant),
            nameof(ConcernBodyVariant),
            nameof(ConcernMoodVariant),
            nameof(ConcernExploreVariant),
            nameof(ConcernAnxietyIsSelected),
            nameof(ConcernBodyIsSelected),
            nameof(ConcernMoodIsSelected),
            nameof(ConcernExploreIsSelected));
    }
}
