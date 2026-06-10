using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MvvmHelpers;
using PsychologyApp.Application;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Profile;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.ViewModels.Motivator;

public class QuoteViewModel : BaseViewModel
{
    public ObservableRangeCollection<QuoteItem> QuotesObservableCollection { get; set; } = [];
    public ICommand LoadMoreQuotesCommand { get; private set; } = default!;

    public string PageTitle => AppStrings.MotivatorTitle;
    public string QuotesSearchingText => AppStrings.QuotesSearching;
    public string QuotesLoadingText => AppStrings.QuotesLoading;
    public string QuotesEmptyTitle => AppStrings.QuotesEmptyTitle;
    public string QuotesEmptyBody => AppStrings.QuotesEmptyBody;
    public string LoadErrorText => AppStrings.LoadError;
    public string RetryText => AppStrings.RetryQuestion;

    private readonly IQuotService _quotService;
    private readonly ILogger<QuoteViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private readonly IToastService _toastService;
    private readonly HashSet<string> _knownQuoteTexts = new(StringComparer.Ordinal);

    public QuoteViewModel(
        INavigation navigation,
        IQuotService quotService,
        ILogger<QuoteViewModel> logger,
        IOptions<AppSettings> settings,
        IToastService toastService)
    {
        try
        {
            _quotService = quotService;
            _logger = logger;
            _settings = settings;
            _toastService = toastService;
            BindNavigation(navigation);
            Cancel = new Command(CancelProgress);
            LoadMoreQuotesCommand = new AsyncCommand(() => AddFreshQuotesAsync());
            Reload = new AsyncCommand(InitAsync);

            InitAsync().FireAndForget();
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
            nameof(QuotesSearchingText),
            nameof(QuotesLoadingText),
            nameof(QuotesEmptyTitle),
            nameof(QuotesEmptyBody),
            nameof(LoadErrorText),
            nameof(RetryText));
        InitAsync().FireAndForget();
    }

    private async Task InitAsync()
    {
        try
        {
            await AppReadiness.DatabaseReadyAsync;
            await MainThread.InvokeOnMainThreadAsync(SetInit);

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            CancellationToken cancellationToken = timeoutSource.Token;

            await LoadQuotesAsync(cancellationToken);
            await FillCollAsync(20, cancellationToken);

            await MainThread.InvokeOnMainThreadAsync(SetDone);
        }

        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(SetFail);
            _logger.LogError(e, "QuoteViewModel init failed.");
        }
    }

    private async Task FillCollAsync(int count, CancellationToken cancellationToken)
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            QuotesObservableCollection.Clear();
            _knownQuoteTexts.Clear();
        });

        await AddItemsInCollAsync(count, cancellationToken);
    }

    private async Task AddItemsInCollAsync(int count, CancellationToken cancellationToken)
    {
        IEnumerable<QuotDTO> quotDTOs = await _quotService.GetAllAsync(count, cancellationToken);

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            foreach (QuotDTO quotDTO in quotDTOs)
            {
                if (string.IsNullOrEmpty(quotDTO.Text) || !_knownQuoteTexts.Add(quotDTO.Text))
                {
                    continue;
                }

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
                QuotesObservableCollection.Add(quoteItem);
                if (quotDTO.QuotId > 0 && !quotDTO.IsReaded)
                {
                    _quotService.MarkAsReadedAsync(quotDTO.QuotId, cancellationToken).FireAndForget();
                }
            }
        });
    }

    public async Task AddFreshQuotesAsync(CancellationToken cancellationToken = default)
    {
        using CancellationTokenSource? timeoutSource = cancellationToken.CanBeCanceled
            ? null
            : OperationCancellation.CreateSmallTimeoutSource(_settings);
        CancellationToken effectiveToken = timeoutSource?.Token ?? cancellationToken;

        try
        {
            await LoadQuotesAsync(effectiveToken);
            await AddItemsInCollAsync(1, effectiveToken);
        }

        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(SetFail);
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

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                QuotesObservableCollection[index] = quoteItem;
            });

            await _quotService.MarkAsFavouriteAsync(quoteId, isFavourite, cancellationToken);
        }

        catch (Exception e)
        {
            quoteItem.IsFavourite = previousValue;

            await MainThread.InvokeOnMainThreadAsync(() =>
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
            await Clipboard.Default.SetTextAsync($"{quoteItem.Text} ({quoteItem.Author})");
            _toastService.ShortToast(AppStrings.QuoteCopied);
        });
}
