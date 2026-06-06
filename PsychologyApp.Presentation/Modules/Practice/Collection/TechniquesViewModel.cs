using Microsoft.Extensions.Options;
using MvvmHelpers;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Practice.Messages;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Technique.Main;
using System.Windows.Input;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.Technique.Main;

public class TechniquesViewModel : BaseViewModel
{
    private readonly SemaphoreSlim _initGate = new(1, 1);
    private readonly ITechniqueService _techniqueService;
    private readonly IToastService _toastService;
    private readonly ITechniqueMessenger _techniqueMessenger;
    private readonly INavigationService _navigationService;
    private readonly IOptions<AppSettings> _settings;

    public ObservableRangeCollection<TechniqueItem> Techniques { get; private set; } = [];
    public ICommand ConstructorTapped { get; private set; } = default!;
    public ICommand OpenProfileCommand { get; private set; } = default!;

    public string PageTitle => AppStrings.PracticeHomeTitle;
    public string MyTechniquesLabel => AppStrings.PracticeMyTechniques;
    public string CreateButtonText => AppStrings.PracticeCreate;
    public string ProfileToolbarText => AppStrings.ProfileTitle;

    public TechniquesViewModel(
        INavigation navigation,
        ITechniqueService techniqueService,
        IToastService toastService,
        ITechniqueMessenger techniqueMessenger,
        Func<INavigation, INavigationService> navigationServiceFactory,
        IOptions<AppSettings> settings)
    {
        _techniqueService = techniqueService;
        _toastService = toastService;
        _techniqueMessenger = techniqueMessenger;
        _navigationService = navigationServiceFactory(navigation);
        _settings = settings;
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.PracticeTechniquesList;

        ConstructorTapped = new AsyncCommand(() => _navigationService.GoToDesignerAsync(-1));
        OpenProfileCommand = new AsyncCommand(() => _navigationService.GoToUserProfileAsync());
        _techniqueMessenger.Subscribe(this, message => OnTechniqueMessageAsync(message).FireAndForget());
        UserPreferences.Changed += OnPreferencesChanged;
        InitializeAsync().FireAndForget();
    }

    private void OnPreferencesChanged()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(MyTechniquesLabel));
        OnPropertyChanged(nameof(CreateButtonText));
        OnPropertyChanged(nameof(ProfileToolbarText));
        InitializeAsync().FireAndForget();
    }

    public void Unsubscribe() => _techniqueMessenger.Unsubscribe(this);

    private async Task OnTechniqueMessageAsync(TechniqueMessage message)
    {
        if (message.MessageType is not (TechniqueMessageType.Add or TechniqueMessageType.Remove or TechniqueMessageType.Change))
        {
            return;
        }

        await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await _initGate.WaitAsync();
        try
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            await InitAsync(timeoutSource.Token);
        }
        finally
        {
            _initGate.Release();
        }
    }

    public async Task InitAsync(CancellationToken cancellationToken)
    {
        try
        {
            Techniques.Clear();
            Techniques.AddRange(BuildStaticItems());
            IEnumerable<TechniqueDTO> dynamicSource = await _techniqueService.GetTechniquesListAsync(500, cancellationToken);
            Techniques.AddRange(dynamicSource.Select(ParseFromDb));
        }
        catch
        {
            _toastService.ShortToast(AppStrings.PracticeInitError);
        }
    }

    private IEnumerable<TechniqueItem> BuildStaticItems()
    {
        const string image = "method.png";
        return TechniqueListCatalog.GetBuiltIn().Select(entry => new TechniqueItem
        {
            Number = entry.Number,
            Date = entry.Date,
            Image = image,
            Title = entry.Title,
            Subtitle = entry.Subtitle,
            Theme = entry.Theme,
            Author = entry.Author,
            Active = true,
            TapCommand = new AsyncCommand(() => _navigationService.GoToTechniqueAsync(entry.TechniqueId))
        });
    }

    private TechniqueItem ParseFromDb(TechniqueDTO item) => new()
    {
        Id = item.TechniqueId,
        Number = AppStrings.PracticeCustomTechniqueNumber(item.TechniqueId),
        Date = item.Date,
        Image = item.Image,
        Title = item.Header,
        Subtitle = item.Describtion,
        Theme = item.Subject,
        Author = item.Author,
        Active = true,
        TapCommand = new AsyncCommand(() => _navigationService.GoToCreatedAsync(item.TechniqueId))
    };
}
