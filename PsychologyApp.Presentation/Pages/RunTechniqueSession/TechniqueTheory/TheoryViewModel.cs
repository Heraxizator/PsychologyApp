using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;

public partial class TheoryViewModel : BaseViewModel
{
    private readonly TechniqueId? _techniqueId;
    private readonly string? _legacyContent;
    private readonly TechniqueCatalogGateway _techniqueCatalog;
    private readonly IOptions<AppSettings>? _settings;
    private readonly ILogger<TheoryViewModel>? _logger;
    private readonly SemaphoreSlim _initGate = new(1, 1);
    private bool _initialized;
    private CancellationTokenSource? _initCts;

    public bool HasInitialized => _initialized;

    public TheoryViewModel() { }

    public TheoryViewModel(
        INavigationService navigationService,
        TechniqueCatalogGateway techniqueCatalog,
        string content,
        TechniqueId? techniqueId = null,
        IOptions<AppSettings>? settings = null,
        ILogger<TheoryViewModel>? logger = null)
    {
        _techniqueId = techniqueId;
        _legacyContent = techniqueId is null ? content : null;
        _techniqueCatalog = techniqueCatalog;
        _settings = settings;
        _logger = logger;
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.TechniqueTheory;

        BindNavigation(navigationService);
        Cancel = new Command(CancelInit);
        Reload = new AsyncCommand(ReloadAsync);

        if (techniqueId is null)
        {
            ApplyContent(content, null);
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

        return InitializeCoreAsync();
    }

    public Task InitializeAsync() => EnsureInitializedAsync();

    private async Task InitializeCoreAsync()
    {
        if (_techniqueId is null)
        {
            return;
        }

        await _initGate.WaitAsync();
        try
        {
            if (_initialized && !IsFail)
            {
                return;
            }

            _initCts?.Cancel();
            _initCts?.Dispose();
            _initCts = _settings is null
                ? new CancellationTokenSource(TimeSpan.FromSeconds(10))
                : OperationCancellation.CreateMiddleTimeoutSource(_settings);
            await LoadTechniqueContentAsync(_techniqueId.Value, _initCts.Token);
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

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(BackText),
            nameof(TechniqueSubtitle),
            nameof(HasTechniqueSubtitle),
            nameof(LoadingText),
            nameof(FailedText),
            nameof(RetryText));

        if (_techniqueId is TechniqueId)
        {
            ReloadAsync().FireAndForget();
            return;
        }

        if (!string.IsNullOrWhiteSpace(_legacyContent))
        {
            ApplyContent(_legacyContent, null);
        }
    }

    private async Task LoadTechniqueContentAsync(TechniqueId techniqueId, CancellationToken cancellationToken)
    {
        try
        {
            await UiThread.RunAsync(SetInit);
            TechniqueDefinition definition = await _techniqueCatalog.GetAsync(techniqueId, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            await UiThread.RunAsync(() =>
            {
                ApplyContent(definition);
                SetDone();
            });
        }
        catch (OperationCanceledException)
        {
            await UiThread.RunAsync(CancelProgress);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to load technique theory content.");
            await UiThread.RunAsync(SetFail);
        }
    }
}
