using PsychologyApp.Presentation.Entities.Quote;

namespace PsychologyApp.Presentation.Pages.QuoteFeed;

public partial class QuoteViewModel
{
    private async Task SelectFeedAsync(string? key)
    {
        QuoteFeedMode mode = _feedCoordinator.ParseFeedKey(key);
        await SwitchFeedAsync(mode);
    }

    private void EnsureFeedFilters() =>
        _feedCoordinator.EnsureFeedFilters(FeedFilters, FeedAllLabel, FeedFavoritesLabel);

    private async Task SwitchFeedAsync(QuoteFeedMode mode)
    {
        if (!_feedCoordinator.TrySwitchFeed(mode))
        {
            _feedCoordinator.SyncFeedFilterSelection(FeedFilters);
            return;
        }

        _feedCoordinator.SyncFeedFilterSelection(FeedFilters);
        await RunInitAsync(seedNewQuote: false);
    }

    private void UpdateAllReadEmptyState() =>
        ShowAllReadEmpty = _feedCoordinator.ShouldShowAllReadEmpty(QuotesObservableCollection.Count, IsDone);
}
