using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Practice.Messages;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Practice.Constructor;

public class DesignerViewModel : BaseViewModel
{
    private readonly long _techniqueId;
    private readonly ITechniqueService _techniqueService;
    private readonly ITechniqueMessenger _techniqueMessenger;
    private readonly ILogger<DesignerViewModel> _logger;
    private readonly IOptions<AppSettings> _settings;
    private readonly INavigationService _navigationService;

    public ICommand ExecuteTechnique { get; private set; } = default!;

    public string PageTitle => AppStrings.TechniqueTitle;
    public string DesignTitle => AppStrings.PracticeDesignTitle;
    public string BackText => AppStrings.Back;
    public string DescriptionSection => AppStrings.TestsDescriptionHeader;
    public string NameFieldLabel => AppStrings.NameLabel;
    public string DescriptionFieldLabel => AppStrings.TestsDescriptionHeader;
    public string ThemeFieldLabel => AppStrings.ThemeLabel;
    public string AuthorFieldLabel => AppStrings.AuthorLabel;
    public string AlgorithmSection => AppStrings.TechniqueAlgorithm;
    public string ActionsFieldLabel => AppStrings.ActionsListLabel;
    public string SaveButtonText => AppStrings.Save;
    public string NamePlaceholder => AppStrings.DesignerNamePlaceholder;
    public string DescriptionPlaceholder => AppStrings.DesignerDescriptionPlaceholder;
    public string ThemePlaceholder => AppStrings.DesignerThemePlaceholder;
    public string AuthorPlaceholder => AppStrings.DesignerAuthorPlaceholder;

    public DesignerViewModel(
        INavigation navigation,
        long techniqueId,
        ITechniqueService techniqueService,
        ITechniqueMessenger techniqueMessenger,
        ILogger<DesignerViewModel> logger,
        IOptions<AppSettings> settings,
        INavigationService navigationService)
    {
        try
        {
            _techniqueService = techniqueService;
            _techniqueMessenger = techniqueMessenger;
            _logger = logger;
            _settings = settings;
            _navigationService = navigationService;
            _techniqueId = techniqueId;

            Path = "method.png";

            ModuleName = AppStrings.ShellTabPractice;
            PageName = AppStrings.PracticeConstructor;

            BindNavigation(navigation, _navigationService);

            ExecuteTechnique = new AsyncCommand(ExecuteOperationAsync);
            UserPreferences.Changed += OnPreferencesChanged;

            InitAsync().FireAndForget();
        }

        catch (Exception e)
        {
            _logger.LogError(e, "DesignerViewModel initialization failed.");
        }
    }

    private void OnPreferencesChanged()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(DesignTitle));
        OnPropertyChanged(nameof(BackText));
        OnPropertyChanged(nameof(DescriptionSection));
        OnPropertyChanged(nameof(NameFieldLabel));
        OnPropertyChanged(nameof(DescriptionFieldLabel));
        OnPropertyChanged(nameof(ThemeFieldLabel));
        OnPropertyChanged(nameof(AuthorFieldLabel));
        OnPropertyChanged(nameof(AlgorithmSection));
        OnPropertyChanged(nameof(ActionsFieldLabel));
        OnPropertyChanged(nameof(SaveButtonText));
        OnPropertyChanged(nameof(NamePlaceholder));
        OnPropertyChanged(nameof(DescriptionPlaceholder));
        OnPropertyChanged(nameof(ThemePlaceholder));
        OnPropertyChanged(nameof(AuthorPlaceholder));
    }

    private async Task ExecuteOperationAsync()
    {
        if (_techniqueId <= 0)
        {
            await ToAddTechniqueAsync();
        }
        else
        {
            await ToChangeTechniqueAsync();
        }
    }

    private async Task FillAsync(CancellationToken cancellationToken)
    {
        try
        {
            TechniqueDTO techniqueDTO = await _techniqueService.GetTechniqueByIdAsync(_techniqueId, cancellationToken);

            if (techniqueDTO.TechniqueId <= 0)
            {
                return;
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Name = techniqueDTO.Header;
                Description = techniqueDTO.Describtion;
                Theme = techniqueDTO.Subject;
                Author = techniqueDTO.Author;
                Actions = techniqueDTO.Actions;
                Path = techniqueDTO.Image;
            });
        }

        catch (Exception e)
        {
            _logger.LogError(e, "Failed to load technique for designer.");
        }
    }

    private async Task InitAsync()
    {
        using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(_settings);
        await FillAsync(timeoutSource.Token);
    }

    private async Task ToChangeTechniqueAsync()
    {
        try
        {
            TechniqueDTO item = new()
            {
                TechniqueId = _techniqueId,
                Header = Name,
                Describtion = Description,
                Subject = Theme,
                Author = Author,
                Actions = Actions,
                Image = Path,
                Number = Guid.NewGuid().ToString(),
                Date = GetTechniqueDate()
            };

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(_settings);
            await _techniqueService.UpdateTechniqueAsync(item, timeoutSource.Token);

            _techniqueMessenger.Send(new TechniqueMessage
            {
                MessageType = TechniqueMessageType.Change,
                Technique = item
            });

            await GoToRootAsync();
        }

        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update technique.");
        }
    }

    private async Task ToAddTechniqueAsync()
    {
        try
        {
            TechniqueDTO technique = new()
            {
                TechniqueId = -1,
                Number = Guid.NewGuid().ToString(),
                Header = Name,
                Describtion = Description,
                Subject = Theme,
                Image = Path,
                Author = Author,
                Actions = Actions,
                Date = GetTechniqueDate()
            };

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(_settings);
            await _techniqueService.AddNewTechniqueAsync(technique, timeoutSource.Token);

            _techniqueMessenger.Send(new TechniqueMessage
            {
                MessageType = TechniqueMessageType.Add,
                Technique = technique
            });

            await GoBackAsync();
        }

        catch (Exception e)
        {
            _logger.LogError(e, "Failed to add technique.");
        }
    }

    private static string GetTechniqueDate() =>
        DateTime.Now.ToString().Split(' ').First();

    #region Public Properties

    private string? _name_string { get; set; }
    public string? Name
    {
        get => _name_string;
        set
        {
            if (_name_string != value)
            {
                _name_string = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    private string? _describtion_string { get; set; }
    public string? Description
    {
        get => _describtion_string;
        set
        {
            if (_describtion_string != value)
            {
                _describtion_string = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }

    private string? _theme_string { get; set; }
    public string? Theme
    {
        get => _theme_string;
        set
        {
            if (_theme_string != value)
            {
                _theme_string = value;
                OnPropertyChanged(nameof(Theme));
            }
        }
    }

    private string? _author_string { get; set; }
    public string? Author
    {
        get => _author_string;
        set
        {
            if (_author_string != value)
            {
                _author_string = value;
                OnPropertyChanged(nameof(Author));
            }
        }
    }

    private string? _algorithm_string { get; set; }
    public string? Actions
    {
        get => _algorithm_string;
        set
        {
            if (_algorithm_string != value)
            {
                _algorithm_string = value;
                OnPropertyChanged(nameof(Actions));
            }
        }
    }

    private string? _path_string { get; set; }
    public string? Path
    {
        get => _path_string;
        set
        {
            if (_path_string != value)
            {
                _path_string = value;
                OnPropertyChanged(nameof(Path));
            }
        }
    }

    #endregion
}
