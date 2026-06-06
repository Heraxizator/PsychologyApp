using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MvvmHelpers;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.ReasonService;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Physics;

public class PhysicsSearchViewModel : BaseViewModel
{
    private const int SearchDebounceMs = 300;

    private readonly IReasonService _reasonService;
    private readonly ILogger<PhysicsSearchViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private CancellationTokenSource? _searchDebounceCts;

    public List<ReasonDTO> ReasonsList { get; private set; } = [];
    public ObservableRangeCollection<ReasonDTO> ResultsObservableCollection { get; private set; } = [];

    public string PageTitle => AppStrings.PhysicsSearchTitle;
    public string SearchToolbarText => AppStrings.PhysicsSearchToolbar;
    public string ProblemLabel => AppStrings.PhysicsProblemLabel;
    public string IllnessPlaceholder => AppStrings.PhysicsIllnessPlaceholder;
    public string EmptySearchHint => AppStrings.PhysicsEmptySearchHint;
    public string EmptySearchSubhint => AppStrings.PhysicsEmptySearchSubhint;
    public string LoadingText => AppStrings.PhysicsLoadingText;
    public string FailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;

    public ICommand SearchCommand { get; private set; } = default!;

    public PhysicsSearchViewModel(
        INavigation navigation,
        IReasonService reasonService,
        ILogger<PhysicsSearchViewModel> logger,
        IOptions<AppSettings> settings)
    {
        try
        {
            _reasonService = reasonService;
            _logger = logger;
            _settings = settings;
            ModuleName = AppStrings.PhysicsTitle;
            PageName = AppStrings.PhysicsSearchPage;

            BindNavigation(navigation);
            Reload = new AsyncCommand(ReloadAsync);
            Cancel = new Command(CancelProgress);
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
        ReloadAsync().FireAndForget();
    }

    private async Task ReloadAsync()
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ReasonsList.Clear();
                ResultsObservableCollection.Clear();
            });

            await InitAsync();
        }
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(SetFail);
            _logger.LogError(e, "Physics search reload failed.");
        }
    }

    private async Task InitAsync()
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(SetInit);

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateLargeTimeoutSource(_settings);
            CancellationToken cancellationToken = timeoutSource.Token;

            IEnumerable<ReasonDTO> reasons = await _reasonService.GetReasonsAsync(0, 10_000, cancellationToken);
            ReasonsList = reasons.ToList();

            await MainThread.InvokeOnMainThreadAsync(SetDone);
        }
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(SetFail);
            _logger.LogError(e, "Physics search init failed.");
        }
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

        List<ReasonDTO> filtered = ReasonsList
            .Where(reason => reason.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .ToList();

        ResultsObservableCollection.ReplaceRange(filtered);
    }
}
