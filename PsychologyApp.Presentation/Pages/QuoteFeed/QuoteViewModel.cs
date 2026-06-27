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

namespace PsychologyApp.Presentation.Pages.QuoteFeed;

public partial class QuoteViewModel : BaseViewModel
{
    public ObservableRangeCollection<QuoteItem> QuotesObservableCollection { get; set; } = [];
    public ObservableCollection<FilterChipTabItem> FeedFilters { get; } = [];
    public ICommand LoadMoreQuotesCommand { get; private set; } = default!;
    public ICommand SelectFeedCommand { get; private set; } = default!;
    public ICommand ShowFavoritesCommand { get; private set; } = default!;

    private readonly IQuotService _quotService;
    private readonly ILogger<QuoteViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private readonly IDatabaseReadySignal _databaseReadySignal;
    private readonly QuoteFeedCoordinator _feedCoordinator;
    private readonly QuoteItemCommandsFactory _quoteCommandsFactory;
    private readonly QuoteFeedLoader _quoteFeedLoader;
    private readonly LanguageContentReloader _languageContentReloader;
    private string? _feedLanguage;

    public QuoteViewModel(
        INavigationService navigationService,
        IQuotService quotService,
        ILogger<QuoteViewModel> logger,
        IOptions<AppSettings> settings,
        QuoteFeedCoordinator feedCoordinator,
        QuoteItemCommandsFactory quoteCommandsFactory,
        QuoteFeedLoader quoteFeedLoader,
        IDatabaseReadySignal databaseReadySignal,
        LanguageContentReloader languageContentReloader)
    {
        try
        {
            _quotService = quotService;
            _logger = logger;
            _settings = settings;
            _databaseReadySignal = databaseReadySignal;
            _feedCoordinator = feedCoordinator;
            _quoteCommandsFactory = quoteCommandsFactory;
            _quoteFeedLoader = quoteFeedLoader;
            _languageContentReloader = languageContentReloader;
            BindNavigation(navigationService);
            Cancel = new Command(CancelProgress);
            LoadMoreQuotesCommand = new AsyncCommand(() => AddFreshQuotesAsync());
            SelectFeedCommand = new Command<string?>(key => SelectFeedAsync(key).FireAndForget());
            ShowFavoritesCommand = new AsyncCommand(() => SwitchFeedAsync(QuoteFeedMode.Favorites));
            Reload = new AsyncCommand(() => RunInitAsync(seedNewQuote: false));
            EnsureFeedFilters();
        }
        catch (Exception e)
        {
            SetFail();
            _logger.LogError(e, "QuoteViewModel initialization failed.");
        }
    }
}
