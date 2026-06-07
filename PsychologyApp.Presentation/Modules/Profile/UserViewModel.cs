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
    public ICommand OpenSettingsCommand { get; private set; } = default!;
    public ICommand OpenDonateCommand { get; private set; } = default!;
    public ICommand ReloadQuotesCommand { get; private set; } = default!;

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
            OpenSettingsCommand = new AsyncCommand(() => navigationService.GoToSettingsAsync());
            OpenDonateCommand = new AsyncCommand(() => navigationService.GoToDonateAsync());
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
            nameof(RecommendedLabel),
            nameof(BestQuotesLabel),
            nameof(QuotesEmptyText),
            nameof(LoadErrorText),
            nameof(RetryText));
        InitTechniques();
        InitAsync().FireAndForget();
    }

    private void InitTechniques()
    {
        MainThread.BeginInvokeOnMainThread(() =>
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
