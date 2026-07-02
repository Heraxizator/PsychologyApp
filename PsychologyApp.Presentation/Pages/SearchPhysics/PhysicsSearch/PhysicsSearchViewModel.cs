using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MvvmHelpers;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Reason;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Physics;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.SearchPhysics;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.SearchPhysics.PhysicsSearch;

public partial class PhysicsSearchViewModel : BaseViewModel
{
    private readonly IReasonSearchService _reasonSearchService;
    private readonly PhysicsSearchCoordinator _searchCoordinator;
    private readonly PhysicsSearchSession _searchSession;
    private readonly ILogger<PhysicsSearchViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private readonly INavigationService _navigationService;
    private readonly IDatabaseReadySignal _databaseReadySignal;
    private readonly SemaphoreSlim _initGate = new(1, 1);
    private readonly PhysicsSearchUiBindings _searchUi = new();
    private CancellationTokenSource? _searchDebounceCts;
    private CancellationTokenSource? _initCts;

    public ObservableRangeCollection<PhysicsReasonItem> ResultsObservableCollection { get; private set; } = [];

    public ICommand SearchCommand { get; private set; } = default!;
    public ICommand LoadMoreSearchResultsCommand { get; private set; } = default!;

    public PhysicsSearchViewModel(
        IReasonSearchService reasonSearchService,
        PhysicsSearchCoordinator searchCoordinator,
        PhysicsSearchSession searchSession,
        ILogger<PhysicsSearchViewModel> logger,
        IOptions<AppSettings> settings,
        INavigationService navigationService,
        IDatabaseReadySignal databaseReadySignal)
    {
        try
        {
            _reasonSearchService = reasonSearchService;
            _searchCoordinator = searchCoordinator;
            _searchSession = searchSession;
            _logger = logger;
            _settings = settings;
            _navigationService = navigationService;
            _databaseReadySignal = databaseReadySignal;
            ModuleName = AppStrings.PhysicsTitle;
            PageName = AppStrings.PhysicsSearchPage;

            BindNavigation(navigationService);
            Reload = new AsyncCommand(ReloadAsync);
            Cancel = new Command(CancelInit);
            SearchCommand = new Command(() => ExecuteSearch(SearchText));
            LoadMoreSearchResultsCommand = new Command(LoadMoreSearchResults);
            ResultsObservableCollection.CollectionChanged += (_, _) => UpdateSearchUiState();
            PropertyChanged += OnSelfPropertyChanged;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PhysicsSearchViewModel initialization failed.");
            SetFail();
        }
    }
}
