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
            ModuleName = "Психосоматик";
            PageName = "Поисковик";

            BindNavigation(navigation);
            Reload = new AsyncCommand(ReloadAsync);
            Cancel = new Command(SetFail);
            SearchCommand = new Command(() => ExecuteSearch(SearchText));

            InitAsync().FireAndForget();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "PhysicsSearchViewModel initialization failed.");
            SetFail();
        }
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
            IEnumerable<ReasonDTO> reasonDTOs = await _reasonService.GetReasonsAsync(0, 10000, timeoutSource.Token);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ReasonsList.AddRange(reasonDTOs);
                ResultsObservableCollection.AddRange(reasonDTOs.Take(50));
            });

            await MainThread.InvokeOnMainThreadAsync(SetDone);
        }
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(SetFail);
            _logger.LogError(e, "Physics search init failed.");
        }
    }

    public void ExecuteSearch(string? input) => DebouncedSearchAsync(input).FireAndForget();

    private async Task DebouncedSearchAsync(string? input)
    {
        _searchDebounceCts?.Cancel();
        _searchDebounceCts?.Dispose();
        _searchDebounceCts = new CancellationTokenSource();
        CancellationToken token = _searchDebounceCts.Token;

        try
        {
            await Task.Delay(SearchDebounceMs, token);
            await ExecuteSearchAsync(input);
        }
        catch (OperationCanceledException)
        {
        }
    }

    public async Task ExecuteSearchAsync(string? input)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            SetInit();

            string text = input.ToLower();
            IEnumerable<ReasonDTO> source = await Task.Run(() =>
                ReasonsList.Where(x => x.Title?.Length >= text.Length && x.Title.ToLower().Contains(text)), CancellationToken.None);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ResultsObservableCollection.Clear();
                ResultsObservableCollection.AddRange(source);
            });

            SetDone();
        }
        catch (Exception e)
        {
            SetFail();
            _logger.LogError(e, "Physics search failed.");
        }
    }

    private string? _search_text;
    public string? SearchText
    {
        get => _search_text;
        set
        {
            if (_search_text == value)
            {
                return;
            }

            _search_text = value;
            ExecuteSearch(_search_text ?? string.Empty);
            OnPropertyChanged(nameof(SearchText));
        }
    }
}
