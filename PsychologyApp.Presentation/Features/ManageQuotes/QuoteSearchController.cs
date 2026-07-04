using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Quot;
using PsychologyApp.Presentation.Entities.Quote;
using PsychologyApp.Presentation.Features.ManageQuotes.Index;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.ManageQuotes;

public sealed class QuoteSearchController
{
    private readonly IQuoteSearchService _quoteSearchService;
    private readonly QuoteItemCommandsFactory _quoteCommandsFactory;
    private readonly QuoteFeedState _feedState;
    private readonly ILogger _logger;
    private readonly Action _onStateChanged;
    private readonly Action _onSearchCleared;
    private readonly Action _onFail;
    private CancellationTokenSource? _searchDebounceCts;
    private string _query = string.Empty;

    public QuoteSearchController(
        IQuoteSearchService quoteSearchService,
        QuoteItemCommandsFactory quoteCommandsFactory,
        QuoteFeedState feedState,
        ILogger logger,
        Action onStateChanged,
        Action onSearchCleared,
        Action onFail)
    {
        _quoteSearchService = quoteSearchService;
        _quoteCommandsFactory = quoteCommandsFactory;
        _feedState = feedState;
        _logger = logger;
        _onStateChanged = onStateChanged;
        _onSearchCleared = onSearchCleared;
        _onFail = onFail;
    }

    public string Query
    {
        get => _query;
        set
        {
            if (_query == value)
            {
                return;
            }

            _query = value;
            _onStateChanged();
            SearchAsync().FireAndForget();
        }
    }

    public bool IsSearching => !string.IsNullOrWhiteSpace(_query);

    public void ClearSilently()
    {
        CancelPendingSearch();

        if (string.IsNullOrEmpty(_query))
        {
            return;
        }

        _query = string.Empty;
        _onStateChanged();
    }

    public void CancelPendingSearch()
    {
        _searchDebounceCts?.Cancel();
        _searchDebounceCts?.Dispose();
        _searchDebounceCts = null;
    }

    private async Task SearchAsync()
    {
        if (!IsSearching)
        {
            await UiThread.RunAsync(() =>
            {
                _feedState.RestoreFeedDisplay();
                _onSearchCleared();
                _onStateChanged();
            });
            return;
        }

        CancelPendingSearch();
        _searchDebounceCts = new CancellationTokenSource();
        CancellationToken token = _searchDebounceCts.Token;

        try
        {
            await Task.Delay(300, token);
            IReadOnlyList<Application.Abstractions.Integration.QuotSeed> seeds =
                await _quoteSearchService.SearchCatalogAsync(_query, cancellationToken: token);

            List<QuoteItem> results = seeds
                .Select(seed => _quoteCommandsFactory.CreateSearchResultItem(
                    seed.Author,
                    seed.Text,
                    seed.Theme,
                    RefreshSearchResultBindingAsync,
                    _onFail))
                .ToList();

            await UiThread.RunAsync(() =>
            {
                _feedState.DisplayItems.ReplaceRange(results);
                _onStateChanged();
            });
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Quote search failed.");
        }
    }

    private Task RefreshSearchResultBindingAsync(QuoteItem quoteItem)
    {
        int index = _feedState.DisplayItems.IndexOf(quoteItem);
        if (index < 0)
        {
            return Task.CompletedTask;
        }

        return UiThread.RunAsync(() => _feedState.DisplayItems[index] = quoteItem);
    }
}
