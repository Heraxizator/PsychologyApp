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
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using PsychologyApp.Presentation.Features.ManageQuotes.Index;

namespace PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;

public partial class QuoteViewModel : BaseViewModel
{
    private readonly QuoteFeedState _feedState = new();
    private readonly QuoteSearchController _searchController;

    public ObservableRangeCollection<QuoteItem> DisplayItems => _feedState.DisplayItems;
    public ObservableCollection<FilterChipTabItem> FeedFilters { get; } = [];
    public ObservableCollection<FilterChipTabItem> ThemeFilters { get; } = [];
    public ICommand LoadMoreQuotesCommand { get; private set; } = default!;
    public ICommand SelectFeedCommand { get; private set; } = default!;
    public ICommand SelectThemeCommand { get; private set; } = default!;
    public ICommand ShowFavoritesCommand { get; private set; } = default!;
    public ICommand ShowAgainCommand { get; private set; } = default!;
    public ICommand OpenProfileCommand { get; private set; } = default!;
    public ICommand CancelSearchCommand { get; private set; } = default!;

    private readonly IQuotService _quotService;
    private readonly ILogger<QuoteViewModel> _logger;
    private readonly IToastService _toastService;
    private readonly IOptions<AppSettings> _settings;
    private readonly IDatabaseReadySignal _databaseReadySignal;
    private readonly QuoteFeedCoordinator _feedCoordinator;
    private readonly QuoteItemCommandsFactory _quoteCommandsFactory;
    private readonly LanguageContentReloader _languageContentReloader;
    private string? _feedLanguage;

    public QuoteViewModel(
        INavigationService navigationService,
        IQuotService quotService,
        IQuoteSearchService quoteSearchService,
        ILogger<QuoteViewModel> logger,
        IToastService toastService,
        IOptions<AppSettings> settings,
        QuoteFeedCoordinator feedCoordinator,
        QuoteItemCommandsFactory quoteCommandsFactory,
        IDatabaseReadySignal databaseReadySignal,
        LanguageContentReloader languageContentReloader)
    {
        try
        {
            _quotService = quotService;
            _logger = logger;
            _toastService = toastService;
            _settings = settings;
            _databaseReadySignal = databaseReadySignal;
            _feedCoordinator = feedCoordinator;
            _quoteCommandsFactory = quoteCommandsFactory;
            _languageContentReloader = languageContentReloader;
            _searchController = new QuoteSearchController(
                quoteSearchService,
                quoteCommandsFactory,
                _feedState,
                logger,
                NotifySearchRelatedProperties,
                () => ShowAllReadEmpty = false,
                SetFail,
                () => _toastService.ShortToast(AppStrings.QuotesSearchError));
            BindNavigation(navigationService);
            OpenProfileCommand = new AsyncCommand(() => navigationService.GoToUserProfileAsync());
            Cancel = new Command(CancelInit);
            CancelSearchCommand = new Command(() => _searchController.CancelPendingSearch());
            LoadMoreQuotesCommand = new AsyncCommand(() => AddFreshQuotesAsync());
            SelectFeedCommand = new Command<string?>(key => SelectFeedAsync(key).FireAndForget());
            SelectThemeCommand = new Command<string?>(key => SelectThemeAsync(key).FireAndForget());
            ShowFavoritesCommand = new AsyncCommand(() => SwitchFeedAsync(QuoteFeedMode.Favorites));
            ShowAgainCommand = new AsyncCommand(ResetReadStateAsync);
            Reload = new AsyncCommand(() => RunInitAsync(seedNewQuote: false));
            EnsureFeedFilters();
            EnsureThemeFilters();
        }
        catch (Exception e)
        {
            SetFail();
            _logger.LogError(e, "QuoteViewModel initialization failed.");
        }
    }

    private void NotifySearchRelatedProperties() =>
        Notify(
            nameof(SearchQuery),
            nameof(IsSearching),
            nameof(IsSearchFilteringVisible),
            nameof(IsQuoteListVisible),
            nameof(IsFeedFiltersVisible),
            nameof(IsThemeFiltersVisible),
            nameof(ShowDailyQuoteHeader),
            nameof(ShowForYouEmpty),
            nameof(ShowFavoritesEmpty),
            nameof(EmptyTitleText),
            nameof(EmptyBodyText),
            nameof(EmptyActionText),
            nameof(EmptyActionCommand),
            nameof(EmptyIconName));

    private void NotifyEmptyStateProperties() =>
        Notify(
            nameof(EmptyTitleText),
            nameof(EmptyBodyText),
            nameof(EmptyActionText),
            nameof(EmptyActionCommand),
            nameof(EmptyIconName),
            nameof(ShowForYouEmpty),
            nameof(ShowFavoritesEmpty));
}
