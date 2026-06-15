using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.ViewModels.Motivator;

public partial class QuoteViewModel
{
    public string PageTitle => AppStrings.MotivatorTitle;
    public string QuotesLoadingText => AppStrings.QuotesLoading;
    public string QuotesEmptyTitle => AppStrings.QuotesEmptyTitle;
    public string QuotesEmptyBody => AppStrings.QuotesEmptyBody;
    public string QuotesRefreshButton => AppStrings.QuotesRefreshButton;
    public string LoadErrorText => AppStrings.LoadError;
    public string RetryText => AppStrings.RetryQuestion;
    public string FeedAllLabel => AppStrings.QuotesFeedAll;
    public string FeedFavoritesLabel => AppStrings.QuotesFeedFavorites;
    public string AllReadTitle => AppStrings.QuotesAllReadTitle;
    public string AllReadBody => AppStrings.QuotesAllReadBody;
    public string ShowFavoritesButtonText => AppStrings.QuotesShowFavorites;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(QuotesLoadingText),
            nameof(QuotesEmptyTitle),
            nameof(QuotesEmptyBody),
            nameof(QuotesRefreshButton),
            nameof(LoadErrorText),
            nameof(RetryText),
            nameof(FeedAllLabel),
            nameof(FeedFavoritesLabel),
            nameof(AllReadTitle),
            nameof(AllReadBody),
            nameof(ShowFavoritesButtonText));
        EnsureFeedFilters();
    }
}
