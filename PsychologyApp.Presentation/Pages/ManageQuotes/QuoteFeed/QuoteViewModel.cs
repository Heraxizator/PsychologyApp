using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MvvmHelpers;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Quot;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.FilterChip;
using PsychologyApp.Presentation.Entities.Quote;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PsychologyApp.Presentation.Features.ManageQuotes.Index;

namespace PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;

public partial class QuoteViewModel : BaseViewModel
{
    public ObservableRangeCollection<QuoteItem> QuotesObservableCollection { get; } = [];
    public ObservableRangeCollection<QuoteItem> DisplayItems { get; } = [];
    public ObservableCollection<FilterChipTabItem> FeedFilters { get; } = [];
    public ICommand LoadMoreQuotesCommand { get; private set; } = default!;
    public ICommand SelectFeedCommand { get; private set; } = default!;
    public ICommand ShowFavoritesCommand { get; private set; } = default!;
    public ICommand ShowAgainCommand { get; private set; } = default!;
    public ICommand OpenProfileCommand { get; private set; } = default!;

    private readonly IQuotService _quotService;
    private readonly IQuoteSearchService _quoteSearchService;
    private readonly ILogger<QuoteViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private readonly IDatabaseReadySignal _databaseReadySignal;
    private readonly QuoteFeedCoordinator _feedCoordinator;
    private readonly QuoteItemCommandsFactory _quoteCommandsFactory;
    private readonly LanguageContentReloader _languageContentReloader;
    private CancellationTokenSource? _searchDebounceCts;
    private string? _feedLanguage;

    public QuoteViewModel(
        INavigationService navigationService,
        IQuotService quotService,
        IQuoteSearchService quoteSearchService,
        ILogger<QuoteViewModel> logger,
        IOptions<AppSettings> settings,
        QuoteFeedCoordinator feedCoordinator,
        QuoteItemCommandsFactory quoteCommandsFactory,
        IDatabaseReadySignal databaseReadySignal,
        LanguageContentReloader languageContentReloader)
    {
        try
        {
            _quotService = quotService;
            _quoteSearchService = quoteSearchService;
            _logger = logger;
            _settings = settings;
            _databaseReadySignal = databaseReadySignal;
            _feedCoordinator = feedCoordinator;
            _quoteCommandsFactory = quoteCommandsFactory;
            _languageContentReloader = languageContentReloader;
            BindNavigation(navigationService);
            OpenProfileCommand = new AsyncCommand(() => navigationService.GoToUserProfileAsync());
            Cancel = new Command(CancelProgress);
            LoadMoreQuotesCommand = new AsyncCommand(() => AddFreshQuotesAsync());
            SelectFeedCommand = new Command<string?>(key => SelectFeedAsync(key).FireAndForget());
            ShowFavoritesCommand = new AsyncCommand(() => SwitchFeedAsync(QuoteFeedMode.Favorites));
            ShowAgainCommand = new AsyncCommand(ResetReadStateAsync);
            Reload = new AsyncCommand(() => RunInitAsync(seedNewQuote: false));
            EnsureFeedFilters();
        }
        catch (Exception e)
        {
            SetFail();
            _logger.LogError(e, "QuoteViewModel initialization failed.");
        }
    }

    private async Task SearchQuotesAsync()
    {
        if (!IsSearching)
        {
            SyncDisplayItemsFromFeed();
            ShowAllReadEmpty = false;
            BumpFeedContentVersion();
            return;
        }

        _searchDebounceCts?.Cancel();
        _searchDebounceCts?.Dispose();
        _searchDebounceCts = new CancellationTokenSource();
        CancellationToken token = _searchDebounceCts.Token;

        try
        {
            await Task.Delay(300, token);
            IReadOnlyList<Application.Abstractions.Integration.QuotSeed> seeds =
                await _quoteSearchService.SearchCatalogAsync(SearchQuery, cancellationToken: token);

            await UiThread.RunAsync(() =>
            {
                DisplayItems.Clear();
                foreach (Application.Abstractions.Integration.QuotSeed seed in seeds)
                {
                    DisplayItems.Add(_quoteCommandsFactory.CreateSearchResultItem(
                        seed.Author,
                        seed.Text,
                        seed.Theme,
                        RefreshSearchResultBindingAsync,
                        SetFail));
                }

                OnPropertyChanged(nameof(IsSearching));
                OnPropertyChanged(nameof(IsFeedFiltersVisible));
        OnPropertyChanged(nameof(ShowDailyQuoteHeader));
        OnPropertyChanged(nameof(ShowForYouEmpty));
        OnPropertyChanged(nameof(ShowFavoritesEmpty));
        NotifyEmptyStateProperties();
                BumpFeedContentVersion();
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
        int index = DisplayItems.IndexOf(quoteItem);
        if (index < 0)
        {
            return Task.CompletedTask;
        }

        return UiThread.RunAsync(() => DisplayItems[index] = quoteItem);
    }

    private void SyncDisplayItemsFromFeed()
    {
        DisplayItems.ReplaceRange(QuotesObservableCollection);
        OnPropertyChanged(nameof(IsSearching));
        OnPropertyChanged(nameof(IsFeedFiltersVisible));
        OnPropertyChanged(nameof(ShowDailyQuoteHeader));
        OnPropertyChanged(nameof(ShowForYouEmpty));
        NotifyEmptyStateProperties();
    }

    private void NotifyEmptyStateProperties() =>
        Notify(
            nameof(EmptyTitleText),
            nameof(EmptyBodyText),
            nameof(EmptyActionText),
            nameof(EmptyActionCommand),
            nameof(EmptyIconName),
            nameof(ShowForYouEmpty),
            nameof(ShowFavoritesEmpty));

    private void BumpFeedContentVersion()
    {
        _feedContentVersion++;
        OnPropertyChanged(nameof(FeedContentVersion));
    }
}
