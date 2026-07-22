using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesViewModel
{
    public string PageTitle => AppStrings.PracticeHomeTitle;
    public string MyTechniquesLabel => AppStrings.PracticeMyTechniques;
    public string PracticeCatalogLabel => AppStrings.PracticeCatalog;
    public string PracticeCatalogHint => AppStrings.PracticeCatalogHint;
    public string CreateButtonText => AppStrings.PracticeCreate;
    public string ProfileToolbarText => AppStrings.ProfileTitle;
    public string TodayForYouLabel => AppStrings.TodayForYou;
    public string TodayStartPracticeText => AppStrings.TodayStartPractice;
    public string TodayMoodQuestion => AppStrings.TodayMoodQuestion;
    public string PracticeEmptyTitle => AppStrings.PracticeEmptyTitle;
    public string PracticeEmptyBody => AppStrings.PracticeEmptyBody;
    public string LoadingText => AppStrings.PracticeLoadingText;
    public string LoadingMoreText => AppStrings.PracticeLoadingMoreText;
    public string FailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(MyTechniquesLabel),
            nameof(PracticeCatalogLabel),
            nameof(PracticeCatalogHint),
            nameof(CreateButtonText),
            nameof(ProfileToolbarText),
            nameof(TodayForYouLabel),
            nameof(TodayReasonText),
            nameof(TodayActionText),
            nameof(EngagementNudgeText),
            nameof(ShowEngagementNudge),
            nameof(TodayMoodQuestion),
            nameof(TodayMoodDisplay),
            nameof(HasTodayMood),
            nameof(MoodHistorySummary),
            nameof(HasMoodHistorySummary),
            nameof(WeeklyInsightText),
            nameof(HasWeeklyInsight),
            nameof(TherapyProgramBanner),
            nameof(HasTherapyProgramBanner),
            nameof(ClinicalRiskBanner),
            nameof(HasClinicalRiskBanner),
            nameof(StreakDisplay),
            nameof(PracticeEmptyTitle),
            nameof(PracticeEmptyBody),
            nameof(LoadingText),
            nameof(LoadingMoreText),
            nameof(FailedText),
            nameof(RetryText));
        UpdateTodayRecommendation();
        ReloadLocalizedContent();
    }
}
