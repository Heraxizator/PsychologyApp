using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MvvmHelpers;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.ReasonService;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Physics;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Physics;

public class PhysicsSearchViewModel : BaseViewModel
{
    private const int SearchDebounceMs = 300;

    private readonly IReasonService _reasonService;
    private readonly ILogger<PhysicsSearchViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private readonly INavigationService _navigationService;
    private readonly SemaphoreSlim _initGate = new(1, 1);
    private CancellationTokenSource? _searchDebounceCts;
    private CancellationTokenSource? _initCts;

    public List<ReasonDTO> ReasonsList { get; private set; } = [];
    public ObservableRangeCollection<PhysicsReasonItem> ResultsObservableCollection { get; private set; } = [];

    public string PageTitle => AppStrings.PhysicsSearchTitle;
    public string SearchToolbarText => AppStrings.PhysicsSearchToolbar;
    public string ProblemLabel => AppStrings.PhysicsProblemLabel;
    public string IllnessPlaceholder => AppStrings.PhysicsIllnessPlaceholder;
    public string EmptySearchHint => AppStrings.PhysicsEmptySearchHint;
    public string EmptySearchSubhint => AppStrings.PhysicsEmptySearchSubhint;
    public string LoadingText => AppStrings.PhysicsLoadingText;
    public string FailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;
    public string SolutionHeader => AppStrings.PhysicsSolutionHeader;
    public string RecommendedPracticesLabel => AppStrings.PhysicsRecommendedPractices;
    public string TryPracticeLabel => AppStrings.PhysicsTryPractice;

    public ICommand SearchCommand { get; private set; } = default!;

    public PhysicsSearchViewModel(
        INavigation navigation,
        IReasonService reasonService,
        ILogger<PhysicsSearchViewModel> logger,
        IOptions<AppSettings> settings,
        INavigationService navigationService)
    {
        try
        {
            _reasonService = reasonService;
            _logger = logger;
            _settings = settings;
            _navigationService = navigationService;
            ModuleName = AppStrings.PhysicsTitle;
            PageName = AppStrings.PhysicsSearchPage;

            BindNavigation(navigation, navigationService);
            Reload = new AsyncCommand(ReloadAsync);
            Cancel = new Command(CancelInit);
            SearchCommand = new Command(() => ExecuteSearch(SearchText));
            UserPreferences.Changed += OnPreferencesChanged;

            InitAsync().FireAndForget();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PhysicsSearchViewModel initialization failed.");
            SetFail();
        }
    }

    private void CancelInit()
    {
        _initCts?.Cancel();
        CancelProgress();
    }

    private void OnPreferencesChanged()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(SearchToolbarText));
        OnPropertyChanged(nameof(ProblemLabel));
        OnPropertyChanged(nameof(IllnessPlaceholder));
        OnPropertyChanged(nameof(EmptySearchHint));
        OnPropertyChanged(nameof(EmptySearchSubhint));
        OnPropertyChanged(nameof(LoadingText));
        OnPropertyChanged(nameof(FailedText));
        OnPropertyChanged(nameof(RetryText));
        OnPropertyChanged(nameof(SolutionHeader));
        OnPropertyChanged(nameof(RecommendedPracticesLabel));
        OnPropertyChanged(nameof(TryPracticeLabel));
        ReloadAsync().FireAndForget();
    }

    private async Task ReloadAsync()
    {
        await _initGate.WaitAsync();
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ReasonsList.Clear();
                ResultsObservableCollection.Clear();
            });

            await InitCoreAsync();
        }
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(SetFail);
            _logger.LogError(e, "Physics search reload failed.");
        }
        finally
        {
            _initGate.Release();
        }
    }

    private async Task InitAsync()
    {
        await _initGate.WaitAsync();
        try
        {
            await InitCoreAsync();
        }
        catch (OperationCanceledException)
        {
            await MainThread.InvokeOnMainThreadAsync(SetDone);
        }
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(SetFail);
            _logger.LogError(e, "Physics search init failed.");
        }
        finally
        {
            _initGate.Release();
        }
    }

    private async Task InitCoreAsync()
    {
        _initCts?.Cancel();
        _initCts?.Dispose();
        _initCts = OperationCancellation.CreateLargeTimeoutSource(_settings);
        CancellationToken cancellationToken = _initCts.Token;

        await MainThread.InvokeOnMainThreadAsync(SetInit);

        IEnumerable<ReasonDTO> reasons = await _reasonService.GetReasonsAsync(0, 10_000, cancellationToken);
        ReasonsList = reasons.ToList();

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            SetDone();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                ExecuteSearch(SearchText);
            }
        });
    }

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                DebouncedSearch(value);
            }
        }
    }

    private void DebouncedSearch(string searchText)
    {
        _searchDebounceCts?.Cancel();
        _searchDebounceCts = new CancellationTokenSource();
        CancellationToken token = _searchDebounceCts.Token;

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(SearchDebounceMs, token);
                await MainThread.InvokeOnMainThreadAsync(() => ExecuteSearch(searchText));
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    private void ExecuteSearch(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            ResultsObservableCollection.Clear();
            return;
        }

        IReadOnlyList<TechniqueId> techniqueIds = SomaticTechniqueMap.RecommendForQuery(searchText);
        List<PhysicsTechniqueSuggestion> suggestions = techniqueIds
            .Select(id =>
            {
                TechniqueDefinition definition = TechniqueCatalog.Get(id);
                return new PhysicsTechniqueSuggestion
                {
                    Title = definition.PageName,
                    OpenCommand = new AsyncCommand(() => _navigationService.GoToTechniqueAsync(id))
                };
            })
            .ToList();

        List<PhysicsReasonItem> filtered = ReasonsList
            .Where(MatchesSearch)
            .Select(dto => CreateItem(dto, suggestions))
            .ToList();

        ResultsObservableCollection.ReplaceRange(filtered);

        bool MatchesSearch(ReasonDTO reason) =>
            reason.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) is true
            || reason.Subtitle?.Contains(searchText, StringComparison.OrdinalIgnoreCase) is true
            || reason.Solution?.Contains(searchText, StringComparison.OrdinalIgnoreCase) is true;
    }

    private PhysicsReasonItem CreateItem(ReasonDTO dto, IReadOnlyList<PhysicsTechniqueSuggestion> suggestions)
    {
        PhysicsReasonItem item = PhysicsReasonItem.FromDto(dto, suggestions);
        item.ToggleExpandCommand = new Command(() =>
        {
            item.IsExpanded = !item.IsExpanded;
            int index = ResultsObservableCollection.IndexOf(item);
            if (index >= 0)
            {
                ResultsObservableCollection[index] = item;
            }
        });
        return item;
    }
}
