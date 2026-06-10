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
    private readonly INavigationService _navigationService;
    private readonly ILogger<UserViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private CancellationTokenSource? _quotesLoadCts;
    private bool _quotesLoadedOnce;
    private int _initGeneration;

    public ICommand OpenOptionsCommand { get; private set; } = default!;
    public ICommand OpenSettingsCommand { get; private set; } = default!;
    public ICommand OpenDonateCommand { get; private set; } = default!;
    public ICommand ReloadQuotesCommand { get; private set; } = default!;
    public ICommand CancelQuotesCommand { get; private set; } = default!;
    public ICommand OpenTestsListCommand { get; private set; } = default!;

    public ObservableCollection<TechniqueItem> Techniques { get; private set; } = [];
    public ObservableCollection<QuoteItem> Quotes { get; private set; } = [];

    public string PageTitle => AppStrings.ProfileTitle;
    public string OptionsLabel => AppStrings.OptionsTitle;
    public string SettingsCardTitle => AppStrings.OptionsSettingsTitle;
    public string SettingsCardSubtitle => AppStrings.ProfileSettingsCardSubtitle;
    public string DonateTitle => AppStrings.OptionsDonateTitle;
    public string DonateSubtitle => AppStrings.OptionsDonateSubtitle;
    public string UserLabel => AppStrings.ProfileUserLabel;
    public string StandardUserLabel => AppStrings.ProfileStandardUser;
    public string TechniquesCompletedLabel => AppStrings.ProfileTechniquesCompleted;
    public string TestsCompletedLabel => AppStrings.ProfileTestsCompleted;
    public string StreakLabel => AppStrings.ProfileStreakDays;
    public string LastPracticeDisplay { get; private set; } = string.Empty;
    public bool HasLastPractice => !string.IsNullOrWhiteSpace(LastPracticeDisplay);
    public string RecommendedLabel => AppStrings.ProfileRecommended;
    public string BestQuotesLabel => AppStrings.ProfileBestQuotes;
    public string QuotesEmptyText => AppStrings.ProfileQuotesEmpty;
    public string QuotesSearchingText => AppStrings.QuotesSearching;
    public string QuotesLoadingText => AppStrings.QuotesLoading;
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
            _navigationService = navigationService;
            _logger = logger;
            _settings = settings;
            ModuleName = AppStrings.ShellTabPractice;
            PageName = AppStrings.ProfileTitle;

            BindNavigation(navigation, navigationService);

            OpenOptionsCommand = new AsyncCommand(() => navigationService.GoToOptionsAsync());
            OpenSettingsCommand = new AsyncCommand(() => navigationService.GoToSettingsAsync());
            OpenDonateCommand = new AsyncCommand(() => navigationService.GoToDonateAsync());
            ReloadQuotesCommand = new AsyncCommand(() => ReloadQuotesAsync());
            CancelQuotesCommand = new Command(CancelQuotesLoading);
            OpenTestsListCommand = new AsyncCommand(() => _navigationService.GoToTestsListAsync());

            InitTechniques();
            InitAsync().FireAndForget();
        }
        catch (Exception e)
        {
            SetQuotesFailed();
            _logger.LogError(e, "UserViewModel initialization failed.");
        }
    }

    public Task InitAsync(bool forceQuotesReload = false) =>
        RefreshAsync(forceQuotesReload);

    public Task RefreshAsync(bool forceQuotesReload = false)
    {
        int generation = Interlocked.Increment(ref _initGeneration);
        return RefreshCoreAsync(generation, forceQuotesReload);
    }

    private async Task RefreshCoreAsync(int generation, bool forceQuotesReload)
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(InitTechniques);

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            CancellationToken cancellationToken = timeoutSource.Token;

            TechniquesCompletedCount = (await _userProgressService.CountTechniqueCompletionsAsync(cancellationToken)).ToString();
            TestsCompletedCount = (await _userProgressService.CountTestResultsAsync(cancellationToken)).ToString();
            StreakCount = AppStrings.ProfileStreakCount(await _userProgressService.GetStreakDaysAsync(cancellationToken));
            DateTime? lastPractice = await _userProgressService.GetLastTechniqueCompletionDateAsync(cancellationToken);
            LastPracticeDisplay = lastPractice is null
                ? string.Empty
                : AppStrings.ProfileLastPractice(lastPractice.Value.ToLocalTime().ToString("d"));
            OnPropertyChanged(nameof(LastPracticeDisplay));
            OnPropertyChanged(nameof(HasLastPractice));

            if (generation != Volatile.Read(ref _initGeneration))
            {
                return;
            }

            if (UserQuotesRefreshPolicy.ShouldReload(_quotesLoadedOnce, forceQuotesReload))
            {
                await LoadQuotesAsync(generation, cancellationToken);
            }
        }
        catch (Exception e)
        {
            if (generation == Volatile.Read(ref _initGeneration))
            {
                await MainThread.InvokeOnMainThreadAsync(SetQuotesFailed);
            }

            _logger.LogError(e, "UserViewModel refresh failed.");
        }
    }

    private Task ReloadQuotesAsync() => RefreshAsync(forceQuotesReload: true);

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(OptionsLabel),
            nameof(SettingsCardTitle),
            nameof(SettingsCardSubtitle),
            nameof(DonateTitle),
            nameof(DonateSubtitle),
            nameof(UserLabel),
            nameof(StandardUserLabel),
            nameof(TechniquesCompletedLabel),
            nameof(TestsCompletedLabel),
            nameof(StreakLabel),
            nameof(LastPracticeDisplay),
            nameof(HasLastPractice),
            nameof(RecommendedLabel),
            nameof(BestQuotesLabel),
            nameof(QuotesEmptyText),
            nameof(QuotesSearchingText),
            nameof(QuotesLoadingText),
            nameof(LoadErrorText),
            nameof(RetryText));
        InitTechniques();
        RefreshAsync(forceQuotesReload: false).FireAndForget();
    }

    private async Task LoadQuotesAsync(int generation, CancellationToken outerToken)
    {
        _quotesLoadCts?.Cancel();
        _quotesLoadCts?.Dispose();
        _quotesLoadCts = CancellationTokenSource.CreateLinkedTokenSource(outerToken);

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            IsQuotesFailed = false;
            IsQuotesReady = false;
            IsQuotesLoading = true;
        });

        try
        {
            IEnumerable<QuotDTO> quotDTOs = await _quotService.GetFavouritesAsync(5, _quotesLoadCts.Token);

            if (generation != Volatile.Read(ref _initGeneration))
            {
                return;
            }

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

                SetQuotesReady();
            });
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            if (generation == Volatile.Read(ref _initGeneration))
            {
                await MainThread.InvokeOnMainThreadAsync(SetQuotesFailed);
            }

            _logger.LogError(e, "Failed to load profile quotes.");
        }
    }

    private void CancelQuotesLoading()
    {
        _quotesLoadCts?.Cancel();

        if (IsQuotesLoading)
        {
            if (_quotesLoadedOnce)
            {
                SetQuotesReady();
            }
            else
            {
                IsQuotesLoading = false;
                IsQuotesReady = true;
            }
        }
    }

    private void SetQuotesReady()
    {
        IsQuotesFailed = false;
        IsQuotesLoading = false;
        IsQuotesReady = true;
        _quotesLoadedOnce = true;
    }

    private void SetQuotesFailed()
    {
        IsQuotesLoading = false;
        IsQuotesReady = false;
        IsQuotesFailed = true;
    }

    private void InitTechniques()
    {
        Techniques.Clear();

        string concern = UserPreferences.Load().OnboardingConcern;
        TechniqueId recommendedId = OnboardingRecommendation.ResolveTechnique(concern);
        TechniqueId[] featuredIds =
        [
            recommendedId,
            TechniqueId.Spin,
            TechniqueId.Paper,
            TechniqueId.Polarity
        ];

        const string image = "method.png";
        HashSet<TechniqueId> added = [];

        foreach (TechniqueId techniqueId in featuredIds)
        {
            if (!added.Add(techniqueId))
            {
                continue;
            }

            TechniqueDefinition definition = TechniqueCatalog.Get(techniqueId);
            Techniques.Add(new TechniqueItem
            {
                Image = image,
                Title = definition.PageName,
                Subtitle = definition.ListSubtitle,
                Theme = definition.Theme,
                Active = true,
                TapCommand = NavigationService is null
                    ? null
                    : new AsyncCommand(() => NavigationService.GoToTechniqueAsync(techniqueId))
            });
        }
    }

    private bool _isQuotesLoading;
    public bool IsQuotesLoading
    {
        get => _isQuotesLoading;
        private set => SetProperty(ref _isQuotesLoading, value);
    }

    private bool _isQuotesReady;
    public bool IsQuotesReady
    {
        get => _isQuotesReady;
        private set => SetProperty(ref _isQuotesReady, value);
    }

    private bool _isQuotesFailed;
    public bool IsQuotesFailed
    {
        get => _isQuotesFailed;
        private set => SetProperty(ref _isQuotesFailed, value);
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
