using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MvvmHelpers;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.ReasonSearch;
using PsychologyApp.Application.Somatic;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Physics;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Physics;

public class PhysicsSearchViewModel : BaseViewModel
{
    private const int SearchDebounceMs = 300;
    private const int SearchResultsPageSize = 20;

    private readonly IReasonSearchService _reasonSearchService;
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
    public string NoResultsHint => AppStrings.PhysicsNoResultsHint;
    public string NoResultsSubhint => AppStrings.PhysicsNoResultsSubhint;
    public string LoadingText => AppStrings.PhysicsLoadingText;
    public string SearchFilteringText => AppStrings.PhysicsSearchFilteringText;
    public string FailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;
    public string SolutionHeader => AppStrings.PhysicsSolutionHeader;
    public string RecommendedPracticesLabel => AppStrings.PhysicsRecommendedPractices;
    public string TryPracticeLabel => AppStrings.PhysicsTryPractice;

    public ICommand SearchCommand { get; private set; } = default!;
    public ICommand LoadMoreSearchResultsCommand { get; private set; } = default!;

    private List<PhysicsReasonItem> _searchMatches = [];
    private int _loadedSearchCount;

    private bool _isSearching;

    public bool IsSearching
    {
        get => _isSearching;
        private set
        {
            if (_isSearching != value)
            {
                _isSearching = value;
                OnPropertyChanged(nameof(IsSearching));
                UpdateSearchUiState();
            }
        }
    }

    public bool IsSearchEmptyPromptVisible => _isSearchEmptyPromptVisible;
    public bool IsSearchFilteringVisible => _isSearchFilteringVisible;
    public bool IsSearchResultsListVisible => _isSearchResultsListVisible;
    public bool IsSearchNoResultsVisible => _isSearchNoResultsVisible;

    public PhysicsSearchViewModel(
        INavigation navigation,
        IReasonSearchService reasonSearchService,
        ILogger<PhysicsSearchViewModel> logger,
        IOptions<AppSettings> settings,
        INavigationService navigationService)
    {
        try
        {
            _reasonSearchService = reasonSearchService;
            _logger = logger;
            _settings = settings;
            _navigationService = navigationService;
            ModuleName = AppStrings.PhysicsTitle;
            PageName = AppStrings.PhysicsSearchPage;

            BindNavigation(navigation, navigationService);
            Reload = new AsyncCommand(ReloadAsync);
            Cancel = new Command(CancelInit);
            SearchCommand = new Command(() => ExecuteSearch(SearchText));
            LoadMoreSearchResultsCommand = new Command(LoadMoreSearchResults);
            ResultsObservableCollection.CollectionChanged += (_, _) => UpdateSearchUiState();
            PropertyChanged += OnSelfPropertyChanged;

            InitAsync().FireAndForget();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PhysicsSearchViewModel initialization failed.");
            SetFail();
        }
    }

    private void OnSelfPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(IsDone) or nameof(IsInit) or nameof(IsFail))
        {
            UpdateSearchUiState();
        }
    }

    private void CancelInit()
    {
        _initCts?.Cancel();
        CancelProgress();
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(SearchToolbarText),
            nameof(ProblemLabel),
            nameof(IllnessPlaceholder),
            nameof(EmptySearchHint),
            nameof(EmptySearchSubhint),
            nameof(NoResultsHint),
            nameof(NoResultsSubhint),
            nameof(LoadingText),
            nameof(SearchFilteringText),
            nameof(FailedText),
            nameof(RetryText),
            nameof(SolutionHeader),
            nameof(RecommendedPracticesLabel),
            nameof(TryPracticeLabel));
        ReloadAsync().FireAndForget();
    }

    private void CancelPendingSearch()
    {
        _searchDebounceCts?.Cancel();
        _searchDebounceCts?.Dispose();
        _searchDebounceCts = null;
    }

    private async Task ReloadAsync()
    {
        await _initGate.WaitAsync();
        try
        {
            CancelPendingSearch();
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
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                SetDone();
                UpdateSearchUiState();
            });
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
        CancelPendingSearch();
        _initCts?.Cancel();
        _initCts?.Dispose();
        _initCts = OperationCancellation.CreateLargeTimeoutSource(_settings);
        CancellationToken cancellationToken = _initCts.Token;

        await MainThread.InvokeOnMainThreadAsync(SetInit);

        await AppReadiness.DatabaseReadyAsync.WaitAsync(cancellationToken);

        ReasonsList = (await _reasonSearchService.LoadReasonsAsync(cancellationToken)).ToList();

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            SetDone();
            UpdateSearchUiState();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                IsSearching = true;
                RunSearchAsync(SearchText, cancellationToken).FireAndForget();
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
                UpdateSearchUiState();
                DebouncedSearch(value);
            }
        }
    }

    private void DebouncedSearch(string searchText)
    {
        _searchDebounceCts?.Cancel();
        _searchDebounceCts?.Dispose();
        _searchDebounceCts = new CancellationTokenSource();
        CancellationToken token = _searchDebounceCts.Token;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            MainThread.BeginInvokeOnMainThread(ClearSearchResults);
            return;
        }

        MainThread.BeginInvokeOnMainThread(() => IsSearching = true);

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(SearchDebounceMs, token);
                await RunSearchAsync(searchText, token);
            }
            catch (TaskCanceledException)
            {
            }
        }, token);
    }

    private void UpdateSearchUiState()
    {
        bool hasText = !string.IsNullOrWhiteSpace(SearchText);
        bool hasResults = ResultsObservableCollection.Count > 0;

        SetSearchUiFlag(ref _isSearchEmptyPromptVisible, IsDone && !hasText && !IsSearching, nameof(IsSearchEmptyPromptVisible));
        SetSearchUiFlag(ref _isSearchFilteringVisible, IsDone && hasText && IsSearching, nameof(IsSearchFilteringVisible));
        SetSearchUiFlag(ref _isSearchResultsListVisible, IsDone && hasText && !IsSearching && hasResults, nameof(IsSearchResultsListVisible));
        SetSearchUiFlag(ref _isSearchNoResultsVisible, IsDone && hasText && !IsSearching && !hasResults, nameof(IsSearchNoResultsVisible));
    }

    private bool _isSearchEmptyPromptVisible;
    private bool _isSearchFilteringVisible;
    private bool _isSearchResultsListVisible;
    private bool _isSearchNoResultsVisible;

    private void SetSearchUiFlag(ref bool field, bool value, string propertyName)
    {
        if (field == value)
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
    }

    private void ExecuteSearch(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            ClearSearchResults();
            return;
        }

        IsSearching = true;
        RunSearchAsync(searchText, CancellationToken.None).FireAndForget();
    }

    private void ClearSearchResults()
    {
        _searchMatches = [];
        _loadedSearchCount = 0;
        ResultsObservableCollection.Clear();
        IsSearching = false;
        UpdateSearchUiState();
    }

    private async Task RunSearchAsync(string searchText, CancellationToken cancellationToken)
    {
        IReadOnlyList<TechniqueId> techniqueIds = SomaticTechniqueRecommendation.RecommendForQuery(searchText);
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

        List<PhysicsReasonItem> matches = await Task.Run(() =>
            _reasonSearchService.Search(ReasonsList, searchText)
                .Select(pair => CreateItem(pair.Reason, suggestions, searchText))
                .ToList(), cancellationToken);

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (cancellationToken.IsCancellationRequested || !string.Equals(_searchText, searchText, StringComparison.Ordinal))
            {
                return;
            }

            _searchMatches = matches;
            _loadedSearchCount = 0;
            ResultsObservableCollection.ReplaceRange(_searchMatches.Take(SearchResultsPageSize).ToList());
            _loadedSearchCount = ResultsObservableCollection.Count;
            IsSearching = false;
            UpdateSearchUiState();
        });
    }

    private void LoadMoreSearchResults()
    {
        if (_loadedSearchCount >= _searchMatches.Count)
        {
            return;
        }

        List<PhysicsReasonItem> nextPage = _searchMatches
            .Skip(_loadedSearchCount)
            .Take(SearchResultsPageSize)
            .ToList();

        if (nextPage.Count == 0)
        {
            return;
        }

        ResultsObservableCollection.AddRange(nextPage);
        _loadedSearchCount += nextPage.Count;
    }

    private PhysicsReasonItem CreateItem(ReasonDTO dto, IReadOnlyList<PhysicsTechniqueSuggestion> suggestions, string searchText)
    {
        PhysicsReasonItem item = PhysicsReasonItem.FromDto(dto, suggestions, searchText);
        item.ToggleExpandCommand = new Command(() => item.IsExpanded = !item.IsExpanded);
        return item;
    }
}
