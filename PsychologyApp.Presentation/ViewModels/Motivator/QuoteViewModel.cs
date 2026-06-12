using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MvvmHelpers;
using PsychologyApp.Application;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Common;
using PsychologyApp.Presentation.Models.Profile;
using System.Collections.ObjectModel;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Quotes;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.ViewModels.Motivator;

public enum QuoteFeedMode
{
    All,
    Favorites
}

public class QuoteViewModel : BaseViewModel
{
    private readonly SemaphoreSlim _initGate = new(1, 1);

    public ObservableRangeCollection<QuoteItem> QuotesObservableCollection { get; set; } = [];
    public ObservableCollection<FilterChipTabItem> FeedFilters { get; } = [];
    public ICommand LoadMoreQuotesCommand { get; private set; } = default!;
    public ICommand SelectFeedCommand { get; private set; } = default!;
    public ICommand ShowFavoritesCommand { get; private set; } = default!;

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

    private bool _showAllReadEmpty;
    public bool ShowAllReadEmpty
    {
        get => _showAllReadEmpty;
        private set => SetProperty(ref _showAllReadEmpty, value);
    }

    private readonly IQuotService _quotService;
    private readonly ILogger<QuoteViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private readonly IToastService _toastService;
    private readonly IQuotesChangeNotifier _quotesChangeNotifier;
    private readonly HashSet<string> _knownQuoteTexts = new(StringComparer.Ordinal);
    private QuoteFeedMode _feedMode = QuoteFeedMode.All;

    public QuoteViewModel(
        INavigation navigation,
        INavigationService navigationService,
        IQuotService quotService,
        ILogger<QuoteViewModel> logger,
        IOptions<AppSettings> settings,
        IToastService toastService,
        IQuotesChangeNotifier quotesChangeNotifier)
    {
        try
        {
            _quotService = quotService;
            _logger = logger;
            _settings = settings;
            _toastService = toastService;
            _quotesChangeNotifier = quotesChangeNotifier;
            BindNavigation(navigation, navigationService);
            Cancel = new Command(CancelProgress);
            LoadMoreQuotesCommand = new AsyncCommand(() => AddFreshQuotesAsync());
            SelectFeedCommand = new Command<string?>(key => _ = SelectFeedAsync(key));
            ShowFavoritesCommand = new AsyncCommand(() => SwitchFeedAsync(QuoteFeedMode.Favorites));
            Reload = new AsyncCommand(() => RunInitAsync(seedNewQuote: false));
            EnsureFeedFilters();

            RunInitAsync(seedNewQuote: true).FireAndForget();
        }
        catch (Exception e)
        {
            SetFail();
            _logger.LogError(e, "QuoteViewModel initialization failed.");
        }
    }

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

    public Task ReloadFromPullAsync() => RunInitAsync(seedNewQuote: false);

    private async Task SelectFeedAsync(string? key)
    {
        QuoteFeedMode mode = key == "favorites" ? QuoteFeedMode.Favorites : QuoteFeedMode.All;
        await SwitchFeedAsync(mode);
    }

    private void EnsureFeedFilters()
    {
        if (FeedFilters.Count == 0)
        {
            FeedFilters.Add(new FilterChipTabItem { Key = "all", Title = FeedAllLabel });
            FeedFilters.Add(new FilterChipTabItem { Key = "favorites", Title = FeedFavoritesLabel });
        }
        else
        {
            FeedFilters[0].Title = FeedAllLabel;
            FeedFilters[1].Title = FeedFavoritesLabel;
        }

        SyncFeedFilterSelection();
    }

    private void SyncFeedFilterSelection()
    {
        foreach (FilterChipTabItem filter in FeedFilters)
        {
            filter.IsSelected = _feedMode == QuoteFeedMode.Favorites
                ? filter.Key == "favorites"
                : filter.Key == "all";
        }
    }

    private async Task SwitchFeedAsync(QuoteFeedMode mode)
    {
        if (_feedMode == mode)
        {
            SyncFeedFilterSelection();
            return;
        }

        _feedMode = mode;
        SyncFeedFilterSelection();
        await RunInitAsync(seedNewQuote: false);
    }

    private async Task RunInitAsync(bool seedNewQuote)
    {
        await _initGate.WaitAsync();
        try
        {
            await InitAsync(seedNewQuote);
        }
        finally
        {
            _initGate.Release();
        }
    }

    private async Task InitAsync(bool seedNewQuote)
    {
        try
        {
            await AppReadiness.DatabaseReadyAsync;
            await UiThread.RunAsync(SetInit);

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            CancellationToken cancellationToken = timeoutSource.Token;

            if (seedNewQuote && _feedMode == QuoteFeedMode.All)
            {
                await LoadQuotesAsync(cancellationToken);
            }

            await FillCollAsync(20, cancellationToken);
            UpdateAllReadEmptyState();

            await UiThread.RunAsync(SetDone);
        }
        catch (Exception e)
        {
            await UiThread.RunAsync(SetFail);
            _logger.LogError(e, "QuoteViewModel init failed.");
        }
    }

    private void UpdateAllReadEmptyState() =>
        ShowAllReadEmpty = _feedMode == QuoteFeedMode.All
            && QuotesObservableCollection.Count == 0
            && IsDone;

    private async Task FillCollAsync(int count, CancellationToken cancellationToken)
    {
        await UiThread.RunAsync(() =>
        {
            QuotesObservableCollection.Clear();
            _knownQuoteTexts.Clear();
        });

        await AddItemsInCollAsync(count, cancellationToken);
    }

    private async Task AddItemsInCollAsync(int count, CancellationToken cancellationToken)
    {
        IEnumerable<QuotDTO> quotDTOs = _feedMode == QuoteFeedMode.Favorites
            ? await _quotService.GetFavouritesAsync(count, cancellationToken)
            : await _quotService.GetAllAsync(count, cancellationToken);

        await UiThread.RunAsync(() =>
        {
            foreach (QuotDTO quotDTO in quotDTOs)
            {
                if (string.IsNullOrEmpty(quotDTO.Text) || !_knownQuoteTexts.Add(quotDTO.Text))
                {
                    continue;
                }

                QuoteItem quoteItem = CreateQuoteItem(quotDTO);
                QuotesObservableCollection.Add(quoteItem);
            }

            UpdateAllReadEmptyState();
        });
    }

    private QuoteItem CreateQuoteItem(QuotDTO quotDTO)
    {
        QuoteItem quoteItem = new()
        {
            Id = quotDTO.QuotId,
            Text = quotDTO.Text!,
            Author = quotDTO.Title!,
            IsFavourite = quotDTO.IsFavourite,
            IsReaded = quotDTO.IsReaded,
            ShareCommand = CreateShareCommand(quotDTO.Text, quotDTO.Title),
        };

        quoteItem.LikeCommand = CreateLikeCommand(quoteItem);
        quoteItem.CopyCommand = CreateCopyCommand(quoteItem);
        return quoteItem;
    }

    public async Task AddFreshQuotesAsync(CancellationToken cancellationToken = default)
    {
        using CancellationTokenSource? timeoutSource = cancellationToken.CanBeCanceled
            ? null
            : OperationCancellation.CreateSmallTimeoutSource(_settings);
        CancellationToken effectiveToken = timeoutSource?.Token ?? cancellationToken;

        try
        {
            if (_feedMode == QuoteFeedMode.All)
            {
                await LoadQuotesAsync(effectiveToken);
            }

            await AddItemsInCollAsync(5, effectiveToken);
            UpdateAllReadEmptyState();
        }
        catch (Exception e)
        {
            await UiThread.RunAsync(SetFail);
            _logger.LogError(e, "Failed to add fresh quotes.");
        }
    }

    private async Task MarkAsFavouriteAsync(long quoteId, bool isFavourite, CancellationToken cancellationToken)
    {
        QuoteItem? quoteItem = QuotesObservableCollection.FirstOrDefault(x => x.Id == quoteId);

        if (quoteItem is null)
        {
            return;
        }

        int index = QuotesObservableCollection.IndexOf(quoteItem);
        bool previousValue = quoteItem.IsFavourite;

        try
        {
            quoteItem.IsFavourite = isFavourite;

            await UiThread.RunAsync(() =>
            {
                QuotesObservableCollection[index] = quoteItem;
            });

            await _quotService.MarkAsFavouriteAsync(quoteId, isFavourite, cancellationToken);
            _quotesChangeNotifier.NotifyFavoritesChanged();
            _toastService.ShortToast(isFavourite
                ? AppStrings.QuotesFavoriteAdded
                : AppStrings.QuotesFavoriteRemoved);
        }
        catch (Exception e)
        {
            quoteItem.IsFavourite = previousValue;

            await UiThread.RunAsync(() =>
            {
                QuotesObservableCollection[index] = quoteItem;
                SetFail();
            });

            _logger.LogError(e, "Failed to mark quote as favourite.");
        }
    }

    private Task LoadQuotesAsync(CancellationToken cancellationToken) =>
        _quotService.LoadSingleAsync(cancellationToken);

    private ICommand CreateLikeCommand(QuoteItem quoteItem) =>
        new AsyncCommand(async () =>
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(_settings);
            await MarkAsFavouriteAsync(quoteItem.Id, !quoteItem.IsFavourite, timeoutSource.Token);
        });

    private ICommand CreateShareCommand(string? text, string? title) =>
        new AsyncCommand(() => Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = QuoteShareFormatter.Format(text ?? string.Empty, title ?? AppStrings.UnknownAuthor),
            Title = AppStrings.QuoteShareTitle
        }));

    private ICommand CreateCopyCommand(QuoteItem quoteItem) =>
        new AsyncCommand(async () =>
        {
            await Clipboard.Default.SetTextAsync(
                QuoteShareFormatter.Format(quoteItem.Text, quoteItem.Author));
            _toastService.ShortToast(AppStrings.QuoteCopied);
        });
}
