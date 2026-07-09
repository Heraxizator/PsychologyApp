using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Technique;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueCreated;

public partial class CreatedViewModel : BaseViewModel
{
    private readonly long _techniqueId;
    private readonly ITechniqueService _techniqueService;
    private readonly IDialogService _dialogService;
    private readonly ITechniqueMessenger _techniqueMessenger;
    private readonly ILogger<CreatedViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private readonly INavigationService _navigationService;
    private readonly IUserProgressService _userProgressService;
    private readonly CustomTechniqueSessionOperations _sessionOperations;
    private readonly TechniqueSessionCompletionService _sessionCompletionService;
    private readonly DateTime _sessionStartedAt = DateTime.UtcNow;
    private readonly SemaphoreSlim _initGate = new(1, 1);
    private bool _initialized;
    private CancellationTokenSource? _initCts;

    public bool HasInitialized => _initialized;

    public CreatedViewModel(
        long techniqueId,
        IDialogService dialogService,
        ITechniqueService techniqueService,
        ITechniqueMessenger techniqueMessenger,
        ILogger<CreatedViewModel> logger,
        IOptions<AppSettings> settings,
        INavigationService navigationService,
        IUserProgressService userProgressService,
        CustomTechniqueSessionOperations sessionOperations,
        TechniqueSessionCompletionService sessionCompletionService)
    {
        _techniqueId = techniqueId;
        _dialogService = dialogService;
        _techniqueService = techniqueService;
        _techniqueMessenger = techniqueMessenger;
        _logger = logger;
        _settings = settings;
        _navigationService = navigationService;
        _userProgressService = userProgressService;
        _sessionOperations = sessionOperations;
        _sessionCompletionService = sessionCompletionService;

        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.PracticeCustomTechnique;

        BindNavigation(_navigationService);
        WireCommands();
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
            _initCts = OperationCancellation.CreateSmallTimeoutSource(_settings);
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
