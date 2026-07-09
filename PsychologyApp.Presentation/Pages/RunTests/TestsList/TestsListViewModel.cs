using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MvvmHelpers;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.RunTests.TestsList;

public partial class TestsListViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IDatabaseReadySignal _databaseReadySignal;
    private readonly TestsListLoader _testsListLoader;
    private readonly IOptions<AppSettings> _settings;
    private readonly ILogger<TestsListViewModel> _logger;
    private readonly SemaphoreSlim _initGate = new(1, 1);
    private bool _initialized;
    private CancellationTokenSource? _initCts;

    public bool HasInitialized => _initialized;

    public ICommand OpenProfileCommand { get; }

    public ObservableRangeCollection<TestItem> TestItemCollection { get; } = [];

    public TestsListViewModel(
        INavigationService navigationService,
        IDatabaseReadySignal databaseReadySignal,
        TestsListLoader testsListLoader,
        IOptions<AppSettings> settings,
        ILogger<TestsListViewModel> logger)
    {
        _navigationService = navigationService;
        _databaseReadySignal = databaseReadySignal;
        _testsListLoader = testsListLoader;
        _settings = settings;
        _logger = logger;
        BindNavigation(navigationService);
        OpenProfileCommand = new AsyncCommand(() => _navigationService.GoToUserProfileAsync());
        Cancel = new Command(CancelInit);
        Reload = new AsyncCommand(ReloadAsync);
    }

    public Task EnsureInitializedAsync()
    {
        if (_initialized && !IsFail)
        {
            return Task.CompletedTask;
        }

        return InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await _initGate.WaitAsync();
        try
        {
            if (_initialized && !IsFail)
            {
                return;
            }

            _initCts?.Cancel();
            _initCts?.Dispose();
            _initCts = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            await InitAsync(_initCts.Token);
            _initialized = !IsFail;
        }
        finally
        {
            _initGate.Release();
        }
    }

    private async Task ReloadAsync()
    {
        _initialized = false;
        await EnsureInitializedAsync();
    }

    private void CancelInit()
    {
        _initCts?.Cancel();
        CancelProgress();
    }
}
