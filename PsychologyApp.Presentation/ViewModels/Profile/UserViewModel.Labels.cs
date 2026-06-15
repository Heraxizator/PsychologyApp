using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.ViewModels.Profile;

public partial class UserViewModel
{
    public string PageTitle => AppStrings.ProfileTitle;
    public string OptionsLabel => AppStrings.OptionsTitle;
    public string SettingsCardTitle => AppStrings.OptionsSettingsTitle;
    public string SettingsCardSubtitle => AppStrings.ProfileSettingsCardSubtitle;
    public string DonateTitle => AppStrings.OptionsDonateTitle;
    public string DonateSubtitle => AppStrings.OptionsDonateSubtitle;
    public string FeedbackCardTitle => AppStrings.OptionsFeedbackTitle;
    public string FeedbackCardSubtitle => AppStrings.OptionsFeedbackSubtitle;
    public string InfoCardTitle => AppStrings.OptionsAboutTitle;
    public string InfoCardSubtitle => AppStrings.OptionsAboutSubtitle;
    public string UserLabel => AppStrings.ProfileUserLabel;
    public string StandardUserLabel => AppStrings.ProfileStandardUser;
    public string TechniquesCompletedLabel => AppStrings.ProfileTechniquesCompleted;
    public string TestsCompletedLabel => AppStrings.ProfileTestsCompleted;
    public string StreakLabel => AppStrings.ProfileStreakDays;
    public string LastPracticeDisplay { get; private set; } = string.Empty;
    public bool HasLastPractice => !string.IsNullOrWhiteSpace(LastPracticeDisplay);
    public string RecommendedLabel => AppStrings.ProfileRecommended;
    public string BestQuotesLabel => AppStrings.ProfileBestQuotes;
    public string QuotesEmptyText => AppStrings.ProfileQuotesEmpty;
    public string GoToQuotesTabText => AppStrings.QuotesGoToTab;
    public bool HasQuotes => Quotes.Count > 0;
    public bool ShowQuotesEmptyCta => IsQuotesReady && !HasQuotes;
    public string QuotesSearchingText => AppStrings.QuotesSearching;
    public string QuotesLoadingText => AppStrings.QuotesLoading;
    public string LoadErrorText => AppStrings.LoadError;
    public string RetryText => AppStrings.RetryQuestion;
    public string PracticeHistoryTitle => AppStrings.PracticeHistoryTitle;
    public string PracticeHistoryEmpty => AppStrings.PracticeHistoryEmpty;
    public bool HasPracticeHistory => PracticeHistory.Count > 0;
    public bool ShowPracticeHistoryEmpty => !HasPracticeHistory;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(OptionsLabel),
            nameof(SettingsCardTitle),
            nameof(SettingsCardSubtitle),
            nameof(DonateTitle),
            nameof(DonateSubtitle),
            nameof(FeedbackCardTitle),
            nameof(FeedbackCardSubtitle),
            nameof(InfoCardTitle),
            nameof(InfoCardSubtitle),
            nameof(UserLabel),
            nameof(StandardUserLabel),
            nameof(TechniquesCompletedLabel),
            nameof(TestsCompletedLabel),
            nameof(StreakLabel),
            nameof(LastPracticeDisplay),
            nameof(HasLastPractice),
            nameof(RecommendedLabel),
            nameof(BestQuotesLabel),
            nameof(QuotesEmptyText),
            nameof(QuotesSearchingText),
            nameof(QuotesLoadingText),
            nameof(LoadErrorText),
            nameof(RetryText),
            nameof(PracticeHistoryTitle),
            nameof(PracticeHistoryEmpty),
            nameof(HasPracticeHistory),
            nameof(ShowPracticeHistoryEmpty),
            nameof(GoToQuotesTabText),
            nameof(HasQuotes),
            nameof(ShowQuotesEmptyCta));
        InitTechniques();
        RefreshAsync(forceQuotesReload: false).FireAndForget();
    }
}
