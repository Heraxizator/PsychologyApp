using PsychologyApp.Presentation.Entities.Quote;
using PsychologyApp.Presentation.Features.ManageQuotes;

namespace PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;

public partial class QuoteViewModel
{
    private int _feedLoadGeneration;

    public QuoteFeedMode FeedMode => _feedCoordinator.FeedMode;

    private async Task SelectFeedAsync(string? key)
    {
        QuoteFeedMode mode = _feedCoordinator.ParseFeedKey(key);
        await SwitchFeedAsync(mode);
    }

    private void EnsureFeedFilters() =>
        _feedCoordinator.EnsureFeedFilters(FeedFilters, FeedAllLabel, FeedFavoritesLabel, FeedForYouLabel);

    private async Task SwitchFeedAsync(QuoteFeedMode mode)
    {
        _feedCoordinator.SetFeedMode(mode);
        _feedCoordinator.SyncFeedFilterSelection(FeedFilters);
        ClearSearchQuerySilently();

        await UiThread.RunAsync(() =>
        {
            _feedState.ClearFeed();
            ShowAllReadEmpty = false;
            OnPropertyChanged(nameof(FeedMode));
            OnPropertyChanged(nameof(ShowDailyQuoteHeader));
            NotifySearchRelatedProperties();
        });

        await ReloadFeedAsync(seedNewQuote: false);
    }

    private void ClearSearchQuerySilently() => _searchController.ClearSilently();

    private async Task UpdateAllReadEmptyStateAsync(CancellationToken cancellationToken = default)
    {
        ShowAllReadEmpty = await _feedCoordinator.ShouldShowAllReadEmptyAsync(
            _feedState.FeedItemCount,
            IsDone,
            _quotService,
            cancellationToken);
    }

    private async Task ResetReadStateAsync()
    {
        await _quotService.ResetReadStateAsync();
        await RunInitAsync(seedNewQuote: true);
    }
}
