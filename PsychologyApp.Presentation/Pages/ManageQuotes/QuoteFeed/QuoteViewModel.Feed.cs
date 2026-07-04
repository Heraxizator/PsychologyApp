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
            QuotesObservableCollection.Clear();
            DisplayItems.Clear();
            ShowAllReadEmpty = false;
            OnPropertyChanged(nameof(FeedMode));
            OnPropertyChanged(nameof(ShowDailyQuoteHeader));
            OnPropertyChanged(nameof(ShowForYouEmpty));
            OnPropertyChanged(nameof(ShowFavoritesEmpty));
            NotifyEmptyStateProperties();
        });

        await ReloadFeedAsync(seedNewQuote: false);
    }

    private void ClearSearchQuerySilently()
    {
        _searchDebounceCts?.Cancel();
        _searchDebounceCts?.Dispose();
        _searchDebounceCts = null;

        if (string.IsNullOrEmpty(_searchQuery))
        {
            return;
        }

        _searchQuery = string.Empty;
        OnPropertyChanged(nameof(SearchQuery));
        OnPropertyChanged(nameof(IsSearching));
        OnPropertyChanged(nameof(IsFeedFiltersVisible));
        OnPropertyChanged(nameof(ShowDailyQuoteHeader));
        OnPropertyChanged(nameof(ShowForYouEmpty));
        OnPropertyChanged(nameof(ShowFavoritesEmpty));
        NotifyEmptyStateProperties();
    }

    private async Task UpdateAllReadEmptyStateAsync(CancellationToken cancellationToken = default)
    {
        ShowAllReadEmpty = await _feedCoordinator.ShouldShowAllReadEmptyAsync(
            QuotesObservableCollection.Count,
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
