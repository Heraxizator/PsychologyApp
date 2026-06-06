using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Application.Services.Statistic;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Profile;
using PsychologyApp.Presentation.Technique;
using PsychologyApp.Presentation.ViewModels;
using PsychologyApp.Presentation.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Profile;

public class UserViewModel : BaseViewModel
{
    private readonly IQuotService _quotService;
    private readonly IStatisticService _statisticService;
    private readonly ILogger<UserViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;

    public ICommand OpenOptionsCommand { get; private set; } = default!;
    public ICommand ReloadQuotesCommand { get; private set; } = default!;

    public ObservableCollection<TechniqueItem> Techniques { get; private set; } = [];
    public ObservableCollection<QuoteItem> Quotes { get; private set; } = [];

    public UserViewModel(
        INavigation navigation,
        IQuotService quotService,
        IStatisticService statisticService,
        ILogger<UserViewModel> logger,
        IOptions<AppSettings> settings,
        INavigationService navigationService)
    {
        try
        {
            _quotService = quotService;
            _statisticService = statisticService;
            _logger = logger;
            _settings = settings;
            ModuleName = "Практик";
            PageName = "Профиль";

            BindNavigation(navigation);

            OpenOptionsCommand = new AsyncCommand(() => navigationService.GoToOptionsAsync());
            ReloadQuotesCommand = new AsyncCommand(InitAsync);

            InitAsync().FireAndForget();
        }
        catch (Exception e)
        {
            SetFail();
            _logger.LogError(e, "UserViewModel initialization failed.");
        }
    }

    public async Task InitAsync()
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetInit();
                InitQuotes();
                InitTechniques();
            });

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            CancellationToken cancellationToken = timeoutSource.Token;

            await SetCompletedTechniquesCountAsync(cancellationToken);
            await GetQuotesAsync(cancellationToken);
            await InitQuotesAsync(cancellationToken);

            await MainThread.InvokeOnMainThreadAsync(SetDone);
        }
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(SetFail);
            _logger.LogError(e, "UserViewModel init failed.");
        }
    }

    private void InitTechniques()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Techniques.Add(new TechniqueItem
            {
                Title = "BSFF",
                Subtitle = "Методика депрограммирования подсознания"
            });
        });
    }

    private void InitQuotes()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Quotes.Add(new QuoteItem
            {
                Text = "Лето, урожай, война.",
                Author = "Латинская пословица"
            });
        });
    }

    private async Task InitQuotesAsync(CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<QuotDTO> quotDTOs = await _quotService.GetAllAsync(2, cancellationToken);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                foreach (QuotDTO quotDTO in quotDTOs)
                {
                    if (string.IsNullOrEmpty(quotDTO.Text) || string.IsNullOrEmpty(quotDTO.Title))
                    {
                        continue;
                    }

                    Quotes.Add(new QuoteItem
                    {
                        Text = quotDTO.Text,
                        Author = quotDTO.Title
                    });
                }
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load profile quotes.");
        }
    }

    private Task GetQuotesAsync(CancellationToken cancellationToken) =>
        _quotService.LoadSingleAsync(cancellationToken);

    private async Task SetCompletedTechniquesCountAsync(CancellationToken cancellationToken)
    {
        try
        {
            TechniquesCompletedCount = (await _statisticService.CountPageCompletedAsync(cancellationToken)).ToString();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to count completed techniques.");
        }
    }

    private string _techniques_completed_count = "0";
    public string TechniquesCompletedCount
    {
        get => _techniques_completed_count;
        set => SetProperty(ref _techniques_completed_count, value);
    }
}
