using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Technique;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDesigner;

public partial class DesignerViewModel : BaseViewModel
{
    private readonly long _techniqueId;
    private readonly ITechniqueService _techniqueService;
    private readonly ITechniqueMessenger _techniqueMessenger;
    private readonly DesignerTechniqueOperations _techniqueOperations;
    private readonly IToastService _toastService;
    private readonly ILogger<DesignerViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private readonly INavigationService _navigationService;
    private readonly SemaphoreSlim _initGate = new(1, 1);
    private bool _initialized;
    private bool _isSaving;
    private CancellationTokenSource? _initCts;

    public bool HasInitialized => _initialized;

    public ICommand ExecuteTechnique { get; private set; } = default!;

    public bool IsSaving
    {
        get => _isSaving;
        private set
        {
            if (SetProperty(ref _isSaving, value))
            {
                OnPropertyChanged(nameof(SaveButtonText));
                (ExecuteTechnique as AsyncCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public DesignerViewModel(
        long techniqueId,
        ITechniqueService techniqueService,
        ITechniqueMessenger techniqueMessenger,
        DesignerTechniqueOperations techniqueOperations,
        IToastService toastService,
        ILogger<DesignerViewModel> logger,
        IOptions<AppSettings> settings,
        INavigationService navigationService)
    {
        _techniqueService = techniqueService;
        _techniqueMessenger = techniqueMessenger;
        _techniqueOperations = techniqueOperations;
        _toastService = toastService;
        _logger = logger;
        _settings = settings;
        _navigationService = navigationService;
        _techniqueId = techniqueId;

        Path = "method.png";

        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.PracticeConstructor;

        BindNavigation(_navigationService);

        ExecuteTechnique = new AsyncCommand(ExecuteOperationAsync, () => !IsSaving && !IsFail && (_techniqueId <= 0 || IsDone));
        Cancel = new Command(CancelInit);
        Reload = new AsyncCommand(ReloadAsync);

        if (_techniqueId <= 0)
        {
            SetDone();
            _initialized = true;
        }
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
