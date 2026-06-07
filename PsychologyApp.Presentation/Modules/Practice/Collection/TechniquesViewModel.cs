using Microsoft.Extensions.Options;
using MvvmHelpers;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Application.Services.UserProgress;
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
    private readonly IUserProgressService _userProgressService;
    private readonly IOptions<AppSettings> _settings;

    public ObservableRangeCollection<TechniqueItem> Techniques { get; private set; } = [];
    public ICommand ConstructorTapped { get; private set; } = default!;
    public ICommand OpenProfileCommand { get; private set; } = default!;
    public ICommand StartTodayPracticeCommand { get; private set; } = default!;
    public ICommand RecordMood1Command { get; private set; } = default!;
    public ICommand RecordMood2Command { get; private set; } = default!;
    public ICommand RecordMood3Command { get; private set; } = default!;
    public ICommand RecordMood4Command { get; private set; } = default!;
    public ICommand RecordMood5Command { get; private set; } = default!;

    public string PageTitle => AppStrings.PracticeHomeTitle;
    public string MyTechniquesLabel => AppStrings.PracticeMyTechniques;
    public string CreateButtonText => AppStrings.PracticeCreate;
    public string ProfileToolbarText => AppStrings.ProfileTitle;
    public string TodayForYouLabel => AppStrings.TodayForYou;
    public string TodayRecommendedLabel => AppStrings.TodayRecommended;
    public string TodayStartPracticeText => AppStrings.TodayStartPractice;
    public string TodayMoodQuestion => AppStrings.TodayMoodQuestion;
    public string StreakDisplay => AppStrings.ProfileStreakCount(StreakDays);
    public bool HasStreak => StreakDays > 0;
    public string PracticeEmptyTitle => AppStrings.PracticeEmptyTitle;
    public string PracticeEmptyBody => AppStrings.PracticeEmptyBody;

    private string _todayTechniqueTitle = string.Empty;
    public string TodayTechniqueTitle
    {
        get => _todayTechniqueTitle;
        set => SetProperty(ref _todayTechniqueTitle, value);
    }

    private int _streakDays;
    public int StreakDays
    {
        get => _streakDays;
        set
        {
            if (SetProperty(ref _streakDays, value))
            {
                OnPropertyChanged(nameof(StreakDisplay));
                OnPropertyChanged(nameof(HasStreak));
            }
        }
    }

    private TechniqueId _todayTechniqueId = TechniqueId.Spin;

    public TechniquesViewModel(
        INavigation navigation,
        ITechniqueService techniqueService,
        IToastService toastService,
        ITechniqueMessenger techniqueMessenger,
        Func<INavigation, INavigationService> navigationServiceFactory,
        IUserProgressService userProgressService,
        IOptions<AppSettings> settings)
    {
        _techniqueService = techniqueService;
        _toastService = toastService;
        _techniqueMessenger = techniqueMessenger;
        _navigationService = navigationServiceFactory(navigation);
        _userProgressService = userProgressService;
        _settings = settings;
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.PracticeTechniquesList;

        ConstructorTapped = new AsyncCommand(() => _navigationService.GoToDesignerAsync(-1));
        OpenProfileCommand = new AsyncCommand(() => _navigationService.GoToUserProfileAsync());
        StartTodayPracticeCommand = new AsyncCommand(() => _navigationService.GoToTechniqueAsync(_todayTechniqueId));
        RecordMood1Command = new AsyncCommand(() => RecordMoodAsync(1));
        RecordMood2Command = new AsyncCommand(() => RecordMoodAsync(2));
        RecordMood3Command = new AsyncCommand(() => RecordMoodAsync(3));
        RecordMood4Command = new AsyncCommand(() => RecordMoodAsync(4));
        RecordMood5Command = new AsyncCommand(() => RecordMoodAsync(5));

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
        OnPropertyChanged(nameof(TodayForYouLabel));
        OnPropertyChanged(nameof(TodayRecommendedLabel));
        OnPropertyChanged(nameof(TodayStartPracticeText));
        OnPropertyChanged(nameof(TodayMoodQuestion));
        OnPropertyChanged(nameof(StreakDisplay));
        OnPropertyChanged(nameof(PracticeEmptyTitle));
        OnPropertyChanged(nameof(PracticeEmptyBody));
        UpdateTodayRecommendation();
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
            await AppReadiness.DatabaseReadyAsync.WaitAsync(cancellationToken);

            StreakDays = await _userProgressService.GetStreakDaysAsync(cancellationToken);
            UpdateTodayRecommendation();

            Techniques.Clear();
            Techniques.AddRange(await BuildStaticItemsAsync(cancellationToken));
            IEnumerable<TechniqueDTO> dynamicSource = await _techniqueService.GetTechniquesListAsync(500, cancellationToken);
            Techniques.AddRange(dynamicSource.Select(ParseFromDb));
        }
        catch
        {
            _toastService.ShortToast(AppStrings.PracticeInitError);
        }
    }

    private void UpdateTodayRecommendation()
    {
        string concern = UserPreferences.Load().OnboardingConcern;
        _todayTechniqueId = OnboardingRecommendation.ResolveTechnique(concern);
        TodayTechniqueTitle = TechniqueCatalog.Get(_todayTechniqueId).PageName;
    }

    private async Task<IEnumerable<TechniqueItem>> BuildStaticItemsAsync(CancellationToken cancellationToken)
    {
        const string image = "method.png";
        List<TechniqueItem> items = [];

        foreach (TechniqueListEntry entry in TechniqueListCatalog.GetBuiltIn())
        {
            DateTime? lastPractice = await _userProgressService.GetLastPracticeDateAsync(entry.TechniqueId.ToString(), cancellationToken);
            string? draft = await _userProgressService.GetSessionDraftAsync(entry.TechniqueId.ToString(), cancellationToken);

            items.Add(new TechniqueItem
            {
                Number = entry.Number,
                Date = lastPractice is null
                    ? entry.Date
                    : AppStrings.TechniqueLastPractice(lastPractice.Value.ToLocalTime().ToString("d")),
                Image = image,
                Title = entry.Title,
                Subtitle = entry.Subtitle,
                Theme = !string.IsNullOrWhiteSpace(draft) ? AppStrings.TechniqueContinueBadge : entry.Theme,
                Author = entry.Author,
                Active = true,
                TapCommand = new AsyncCommand(() => _navigationService.GoToTechniqueAsync(entry.TechniqueId))
            });
        }

        return items;
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

    private async Task RecordMoodAsync(int moodLevel)
    {
        await _userProgressService.RecordMoodAsync(moodLevel);
        StreakDays = await _userProgressService.GetStreakDaysAsync();
        _toastService.ShortToast(AppStrings.TodayMoodSaved);
    }

    public async Task TryOpenPendingTechniqueAsync()
    {
        if (UserPreferences.ConsumePendingTechnique() is not TechniqueId techniqueId)
        {
            return;
        }

        await _navigationService.GoToTechniqueAsync(techniqueId);
    }
}
