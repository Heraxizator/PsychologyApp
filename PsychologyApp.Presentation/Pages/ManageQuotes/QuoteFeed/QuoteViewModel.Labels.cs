using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;

public partial class QuoteViewModel
{
    public string PageTitle => AppStrings.MotivatorTitle;
    public string ProfileToolbarText => AppStrings.ProfileTitle;
    public string QuotesLoadingText => AppStrings.QuotesLoading;
    public string QuotesEmptyTitle => AppStrings.QuotesEmptyTitle;
    public string QuotesEmptyBody => AppStrings.QuotesEmptyBody;
    public string QuotesRefreshButton => AppStrings.QuotesRefreshButton;
    public string LoadErrorText => AppStrings.LoadError;
    public string RetryText => AppStrings.RetryQuestion;
    public string FeedAllLabel => AppStrings.QuotesFeedAll;
    public string FeedForYouLabel => AppStrings.QuotesFeedForYou;
    public string FeedFavoritesLabel => AppStrings.QuotesFeedFavorites;
    public string ThemeAllLabel => AppStrings.QuotesThemeAll;
    public string AllReadTitle => AppStrings.QuotesAllReadTitle;
    public string AllReadBody => AppStrings.QuotesAllReadBody;
    public string ShowFavoritesButtonText => AppStrings.QuotesShowFavorites;
    public string ShowAgainButtonText => AppStrings.QuotesShowAgain;
    public string DailyQuoteTitle => AppStrings.QuotesDailyTitle;
    public string SearchPlaceholder => AppStrings.QuotesSearchPlaceholder;
    public string SearchFilteringText => AppStrings.QuotesSearching;
    public string SearchEmptyTitle => AppStrings.QuotesSearchEmptyTitle;
    public string SearchEmptyBody => AppStrings.QuotesSearchEmptyBody;
    public string ForYouEmptyTitle => AppStrings.QuotesForYouEmptyTitle;
    public string ForYouEmptyBody => AppStrings.QuotesForYouEmptyBody;
    public string FavoritesEmptyTitle => AppStrings.ProfileQuotesEmpty;
    public string FavoritesEmptyBody => AppStrings.QuotesFavoritesEmptyBody;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(ProfileToolbarText),
            nameof(QuotesLoadingText),
            nameof(QuotesEmptyTitle),
            nameof(QuotesEmptyBody),
            nameof(QuotesRefreshButton),
            nameof(LoadErrorText),
            nameof(RetryText),
            nameof(FeedAllLabel),
            nameof(FeedForYouLabel),
            nameof(FeedFavoritesLabel),
            nameof(ThemeAllLabel),
            nameof(AllReadTitle),
            nameof(AllReadBody),
            nameof(ShowFavoritesButtonText),
            nameof(ShowAgainButtonText),
            nameof(DailyQuoteTitle),
            nameof(SearchPlaceholder),
            nameof(SearchFilteringText),
            nameof(SearchEmptyTitle),
            nameof(SearchEmptyBody),
            nameof(ForYouEmptyTitle),
            nameof(ForYouEmptyBody),
            nameof(FavoritesEmptyTitle),
            nameof(FavoritesEmptyBody));
        NotifyEmptyStateProperties();
        EnsureFeedFilters();
        EnsureThemeFilters();

        string currentLanguage = UserPreferences.GetPersistedLanguage();
        if (!_initialized)
        {
            _feedLanguage = currentLanguage;
            return;
        }

        if (string.Equals(_feedLanguage, currentLanguage, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _feedLanguage = currentLanguage;
        ReloadFeedForLanguageAsync().FireAndForget();
    }

    private async Task ReloadFeedForLanguageAsync()
    {
        await _languageContentReloader.EnsureReloadedAsync();
        _feedCoordinator.ResetKnownQuotes();
        await RunInitAsync(seedNewQuote: false);
    }
}
