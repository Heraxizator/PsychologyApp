using PsychologyApp.Presentation.Entities.Quote;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;

public partial class QuoteViewModel
{
    private readonly SemaphoreSlim _initGate = new(1, 1);
    private bool _initialized;

    public bool HasInitialized => _initialized;

    public Task EnsureInitializedAsync() =>
        _initialized ? Task.CompletedTask : RunInitAsync(seedNewQuote: false);

    public async Task TryApplyPendingFeedAsync()
    {
        if (!ApplyPendingQuoteFeedIfNeeded())
        {
            return;
        }

        if (!_initialized)
        {
            await EnsureInitializedAsync();
            return;
        }

        await SwitchFeedAsync(_feedCoordinator.FeedMode);
    }

    public Task ReloadFromPullAsync() => RunInitAsync(seedNewQuote: false);

    private bool ApplyPendingQuoteFeedIfNeeded()
    {
        string? pendingKey = UserPreferences.ConsumePendingQuoteFeed();
        if (string.IsNullOrWhiteSpace(pendingKey))
        {
            return false;
        }

        QuoteFeedMode mode = _feedCoordinator.ParseFeedKey(pendingKey);
        _feedCoordinator.SetFeedMode(mode);
        _feedCoordinator.SyncFeedFilterSelection(FeedFilters);
        OnPropertyChanged(nameof(FeedMode));
        OnPropertyChanged(nameof(ShowDailyQuoteHeader));
        return true;
    }

    private async Task RunInitAsync(bool seedNewQuote)
    {
        await _initGate.WaitAsync();
        try
        {
            ApplyPendingQuoteFeedIfNeeded();

            int generation = ++_feedLoadGeneration;
            if (await LoadFeedAsync(seedNewQuote, isInitialLoad: true, generation))
            {
                _initialized = true;
                _feedLanguage = UserPreferences.GetPersistedLanguage();
            }
        }
        finally
        {
            _initGate.Release();
        }
    }

    private async Task ReloadFeedAsync(bool seedNewQuote)
    {
        await _initGate.WaitAsync();
        try
        {
            int generation = ++_feedLoadGeneration;
            await LoadFeedAsync(seedNewQuote, isInitialLoad: false, generation);
        }
        finally
        {
            _initGate.Release();
        }
    }
}
