using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.ProfileUser;

public partial class UserViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IQuotesChangeNotifier _quotesChangeNotifier;
    private readonly ProfileStatsLoader _profileStatsLoader;
    private readonly ProfileQuotesLoader _profileQuotesLoader;
    private readonly ProfilePracticeHistoryLoader _practiceHistoryLoader;
    private readonly ProfileFeaturedTechniquesBuilder _featuredTechniquesBuilder;
    private readonly QuoteItemCommandsFactory _quoteCommandsFactory;
    private readonly UserProfileRefreshCoordinator _profileRefreshCoordinator;
    private readonly LanguageContentReloader _languageContentReloader;
    private readonly ILogger<UserViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private int _initGeneration;
    private bool _initialized;
    private string? _feedLanguage;

    public bool HasInitialized => _initialized;

    public async Task EnsureInitializedAsync()
    {
        if (_initialized)
        {
            return;
        }

        await RefreshAsync(forceQuotesReload: false);
        _initialized = true;
        _feedLanguage = UserPreferences.GetPersistedLanguage();
    }

    public UserViewModel(
        ILogger<UserViewModel> logger,
        IOptions<AppSettings> settings,
        INavigationService navigationService,
        IQuotesChangeNotifier quotesChangeNotifier,
        ProfileStatsLoader profileStatsLoader,
        ProfileQuotesLoader profileQuotesLoader,
        ProfilePracticeHistoryLoader practiceHistoryLoader,
        ProfileFeaturedTechniquesBuilder featuredTechniquesBuilder,
        QuoteItemCommandsFactory quoteCommandsFactory,
        UserProfileRefreshCoordinator profileRefreshCoordinator,
        LanguageContentReloader languageContentReloader)
    {
        try
        {
            _navigationService = navigationService;
            _quotesChangeNotifier = quotesChangeNotifier;
            _profileStatsLoader = profileStatsLoader;
            _profileQuotesLoader = profileQuotesLoader;
            _practiceHistoryLoader = practiceHistoryLoader;
            _featuredTechniquesBuilder = featuredTechniquesBuilder;
            _quoteCommandsFactory = quoteCommandsFactory;
            _profileRefreshCoordinator = profileRefreshCoordinator;
            _languageContentReloader = languageContentReloader;
            _logger = logger;
            _settings = settings;
            _quotesChangeNotifier.FavoritesChanged += OnFavoritesChanged;
            ModuleName = AppStrings.ShellTabPractice;
            PageName = AppStrings.ProfileTitle;

            BindNavigation(navigationService);
            WireCommands(navigationService);

            InitTechniques();
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
}
