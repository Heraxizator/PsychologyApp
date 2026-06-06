using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Practice.Messages;
using PsychologyApp.Presentation.ViewModels;
using PsychologyApp.Presentation.Services;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Practice.Constructor;

public class CreatedViewModel : BaseViewModel
{
    private readonly long _techniqueId;
    private readonly ITechniqueService _techniqueService;
    private readonly IDialogService _dialogService;
    private readonly ITechniqueMessenger _techniqueMessenger;
    private readonly ILogger<CreatedViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private readonly INavigationService _navigationService;

    public ICommand Remove { get; private set; } = default!;
    public ICommand Edit { get; private set; } = default!;

    public CreatedViewModel(
        INavigation navigation,
        long techniqueId,
        IDialogService dialogService,
        ITechniqueService techniqueService,
        ITechniqueMessenger techniqueMessenger,
        ILogger<CreatedViewModel> logger,
        IOptions<AppSettings> settings,
        INavigationService navigationService)
    {
        try
        {
            _techniqueId = techniqueId;
            _dialogService = dialogService;
            _techniqueService = techniqueService;
            _techniqueMessenger = techniqueMessenger;
            _logger = logger;
            _settings = settings;
            _navigationService = navigationService;

            ModuleName = "Практик";
            PageName = "Своя техника";

            BindNavigation(navigation, _navigationService);
            Remove = new AsyncCommand(() => ToRemoveAsync());
            Edit = new AsyncCommand(() => ToEditAsync());

            InitAsync().FireAndForget();
        }
        catch (Exception e)
        {
            SetFail();
            _logger.LogError(e, "CreatedViewModel initialization failed.");
        }
    }

    private Task ToEditAsync() =>
        _navigationService.GoToDesignerAsync(_techniqueId);

    private async Task ToRemoveAsync()
    {
        try
        {
            bool isConfirmed = await _dialogService.AskAsync(null, "Вы уверены, что хотите удалить свою технику", "Да", "Нет");

            if (!isConfirmed)
            {
                return;
            }

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(_settings);
            CancellationToken cancellationToken = timeoutSource.Token;
            TechniqueDTO techniqueDTO = await _techniqueService.GetTechniqueByIdAsync(_techniqueId, cancellationToken);
            await _techniqueService.DeleteTechniqueAsync(techniqueDTO, cancellationToken);

            _techniqueMessenger.Send(new TechniqueMessage
            {
                MessageType = TechniqueMessageType.Remove,
                Technique = techniqueDTO
            });

            await GoToRootAsync();
        }
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(SetFail);
            _logger.LogError(e, "Failed to remove custom technique.");
        }
    }

    private async Task InitAsync()
    {
        try
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(_settings);
            TechniqueDTO techniqueDTO = await _techniqueService.GetTechniqueByIdAsync(_techniqueId, timeoutSource.Token);
            string[] actions = techniqueDTO.Actions?.Split('\n') ?? [];

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                foreach (string action in actions)
                {
                    Algorithm.Add(action);
                }
            });
        }
        catch (Exception e)
        {
            await MainThread.InvokeOnMainThreadAsync(SetFail);
            _logger.LogError(e, "Failed to load custom technique.");
        }
    }
}
