using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

public partial class UserViewModel
{
    public string PageTitle => AppStrings.ProfileTitle;
    public string LoadingText => AppStrings.ProfileLoadingText;
    public string OptionsLabel => AppStrings.OptionsTitle;
    public string OptionsCardSubtitle => AppStrings.ProfileOptionsCardSubtitle;
    public string UserLabel => AppStrings.ProfileUserLabel;
    public string StandardUserLabel => AppStrings.ProfileStandardUser;
    public string TechniquesCompletedLabel => AppStrings.ProfileTechniquesCompleted;
    public string TestsCompletedLabel => AppStrings.ProfileTestsCompleted;
    public string StreakLabel => AppStrings.ProfileStreakDays;
    public string StreakHintText => AppStrings.ProfileStreakHint;
    public string LastPracticeDisplay { get; private set; } = string.Empty;
    public bool HasLastPractice => !string.IsNullOrWhiteSpace(LastPracticeDisplay);
    public string RecommendedLabel => AppStrings.ProfileRecommended;
    public string BestQuotesLabel => AppStrings.ProfileBestQuotes;
    public string QuotesEmptyText => AppStrings.ProfileQuotesEmpty;
    public string GoToQuotesTabText => AppStrings.QuotesGoToTab;
    public string QuotesSectionActionText =>
        ShowQuotesSectionAction ? AppStrings.ProfileQuotesSeeAll : string.Empty;
    public bool ShowQuotesSectionAction => IsQuotesReady && HasQuotes;
    public bool ShowQuotesEmpty => IsQuotesReady && !HasQuotes;
    public bool ShowQuotesPreview => IsQuotesReady && HasQuotes;
    public string QuotesSectionSubtitle =>
        HasQuotesSectionSubtitle
            ? AppStrings.FormatProfileQuotesPreviewSubtitle(DisplayQuotes.Count, Quotes.Count)
            : string.Empty;
    public bool HasQuotes => Quotes.Count > 0;
    public bool HasQuotesSectionSubtitle => IsQuotesReady && HasQuotes;
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
            nameof(LoadingText),
            nameof(OptionsLabel),
            nameof(OptionsCardSubtitle),
            nameof(UserLabel),
            nameof(StandardUserLabel),
            nameof(TechniquesCompletedLabel),
            nameof(TestsCompletedLabel),
            nameof(StreakLabel),
            nameof(StreakHintText),
            nameof(LastPracticeDisplay),
            nameof(HasLastPractice),
            nameof(RecommendedLabel),
            nameof(BestQuotesLabel),
            nameof(QuotesEmptyText),
            nameof(GoToQuotesTabText),
            nameof(QuotesSectionActionText),
            nameof(ShowQuotesSectionAction),
            nameof(ShowQuotesEmpty),
            nameof(ShowQuotesPreview),
            nameof(QuotesSectionSubtitle),
            nameof(HasQuotes),
            nameof(HasQuotesSectionSubtitle),
            nameof(QuotesSearchingText),
            nameof(QuotesLoadingText),
            nameof(LoadErrorText),
            nameof(RetryText),
            nameof(PracticeHistoryTitle),
            nameof(PracticeHistoryEmpty),
            nameof(HasPracticeHistory),
            nameof(ShowPracticeHistoryEmpty),
            nameof(MoodTrendTitle),
            nameof(MoodTrendHint),
            nameof(ShowMoodTrendHint),
            nameof(MoodNotesTitle),
            nameof(HasMoodNotes));
        InitTechniques();

        string currentLanguage = UserPreferences.GetPersistedLanguage();
        if (string.Equals(_feedLanguage, currentLanguage, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _feedLanguage = currentLanguage;
        ReloadProfileForLanguageAsync().FireAndForget();
    }

    private async Task ReloadProfileForLanguageAsync()
    {
        await _languageContentReloader.EnsureReloadedAsync();
        await RefreshAsync(forceQuotesReload: true);
    }
}
