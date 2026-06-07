using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
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
    private readonly IUserProgressService _userProgressService;
    private readonly ILogger<UserViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;

    public ICommand OpenOptionsCommand { get; private set; } = default!;
    public ICommand ReloadQuotesCommand { get; private set; } = default!;

    public ObservableCollection<TechniqueItem> Techniques { get; private set; } = [];
    public ObservableCollection<QuoteItem> Quotes { get; private set; } = [];

    public string PageTitle => AppStrings.ProfileTitle;
    public string OptionsLabel => AppStrings.OptionsTitle;
    public string UserLabel => AppStrings.ProfileUserLabel;
    public string StandardUserLabel => AppStrings.ProfileStandardUser;
    public string TechniquesCompletedLabel => AppStrings.ProfileTechniquesCompleted;
    public string TestsCompletedLabel => AppStrings.ProfileTestsCompleted;
    public string StreakLabel => AppStrings.ProfileStreakDays;
    public string RecommendedLabel => AppStrings.ProfileRecommended;
    public string BestQuotesLabel => AppStrings.ProfileBestQuotes;
    public string QuotesEmptyText => AppStrings.ProfileQuotesEmpty;
    public string LoadErrorText => AppStrings.LoadError;
    public string RetryText => AppStrings.RetryQuestion;

    public UserViewModel(
        INavigation navigation,
        IQuotService quotService,
        IUserProgressService userProgressService,
        ILogger<UserViewModel> logger,
        IOptions<AppSettings> settings,
        INavigationService navigationService)
    {
        try
        {
            _quotService = quotService;
            _userProgressService = userProgressService;
            _logger = logger;
            _settings = settings;
            ModuleName = AppStrings.ShellTabPractice;
            PageName = AppStrings.ProfileTitle;

            BindNavigation(navigation, navigationService);
            Cancel = new Command(CancelProgress);

            OpenOptionsCommand = new AsyncCommand(() => navigationService.GoToOptionsAsync());
            ReloadQuotesCommand = new AsyncCommand(InitAsync);
            UserPreferences.Changed += OnPreferencesChanged;

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
                InitTechniques();
                Quotes.Clear();
            });

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            CancellationToken cancellationToken = timeoutSource.Token;

            TechniquesCompletedCount = (await _userProgressService.CountTechniqueCompletionsAsync(cancellationToken)).ToString();
            TestsCompletedCount = (await _userProgressService.CountTestResultsAsync(cancellationToken)).ToString();
            StreakCount = AppStrings.ProfileStreakCount(await _userProgressService.GetStreakDaysAsync(cancellationToken));
            await InitQuotesAsync(cancellationToken);

            await MainThread.InvokeOnMainThreadAsync(SetDone);
        }
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(SetFail);
            _logger.LogError(e, "UserViewModel init failed.");
        }
    }

    private void OnPreferencesChanged()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(OptionsLabel));
        OnPropertyChanged(nameof(UserLabel));
        OnPropertyChanged(nameof(StandardUserLabel));
        OnPropertyChanged(nameof(TechniquesCompletedLabel));
        OnPropertyChanged(nameof(TestsCompletedLabel));
        OnPropertyChanged(nameof(StreakLabel));
        OnPropertyChanged(nameof(RecommendedLabel));
        OnPropertyChanged(nameof(BestQuotesLabel));
        OnPropertyChanged(nameof(QuotesEmptyText));
        OnPropertyChanged(nameof(LoadErrorText));
        OnPropertyChanged(nameof(RetryText));
        InitTechniques();
    }

    private void InitTechniques()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Techniques.Clear();
            TechniqueDefinition spin = TechniqueCatalog.Get(TechniqueId.Spin);
            Techniques.Add(new TechniqueItem
            {
                Title = spin.PageName,
                Subtitle = spin.ModuleName
            });
        });
    }

    private async Task InitQuotesAsync(CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<QuotDTO> quotDTOs = await _quotService.GetFavouritesAsync(5, cancellationToken);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Quotes.Clear();
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

    private string _techniques_completed_count = "0";
    public string TechniquesCompletedCount
    {
        get => _techniques_completed_count;
        set => SetProperty(ref _techniques_completed_count, value);
    }

    private string _tests_completed_count = "0";
    public string TestsCompletedCount
    {
        get => _tests_completed_count;
        set => SetProperty(ref _tests_completed_count, value);
    }

    private string _streak_count = "0";
    public string StreakCount
    {
        get => _streak_count;
        set => SetProperty(ref _streak_count, value);
    }
}
