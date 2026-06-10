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
using PsychologyApp.Presentation.Technique;
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
    public string TodayStartPracticeText => AppStrings.TodayStartPractice;
    public string TodayMoodQuestion => AppStrings.TodayMoodQuestion;
    public string TodayMoodDisplay { get; private set; } = string.Empty;
    public bool HasTodayMood => !string.IsNullOrWhiteSpace(TodayMoodDisplay);
    public string StreakDisplay => AppStrings.ProfileStreakCount(StreakDays);
    public bool HasStreak => StreakDays > 0;
    public string PracticeEmptyTitle => AppStrings.PracticeEmptyTitle;
    public string PracticeEmptyBody => AppStrings.PracticeEmptyBody;
    public string LoadingText => AppStrings.PracticeLoadingText;
    public string FailedText => AppStrings.LoadFailed;
    public string RetryText => AppStrings.RetryQuestion;

    private TechniqueItem? _todayTechniqueItem;
    public TechniqueItem? TodayTechniqueItem
    {
        get => _todayTechniqueItem;
        private set => SetProperty(ref _todayTechniqueItem, value);
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
                UpdateTodayRecommendation();
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

        Cancel = new Command(CancelProgress);
        Reload = new AsyncCommand(InitializeAsync);

        _techniqueMessenger.Subscribe(this, message => OnTechniqueMessageAsync(message).FireAndForget());
        SetInit();
        InitializeAsync().FireAndForget();
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(MyTechniquesLabel),
            nameof(CreateButtonText),
            nameof(ProfileToolbarText),
            nameof(TodayForYouLabel),
            nameof(TodayStartPracticeText),
            nameof(TodayMoodQuestion),
            nameof(TodayMoodDisplay),
            nameof(HasTodayMood),
            nameof(StreakDisplay),
            nameof(PracticeEmptyTitle),
            nameof(PracticeEmptyBody),
            nameof(LoadingText),
            nameof(FailedText),
            nameof(RetryText));
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
            await MainThread.InvokeOnMainThreadAsync(SetInit);

            await AppReadiness.DatabaseReadyAsync.WaitAsync(cancellationToken);

            StreakDays = await _userProgressService.GetStreakDaysAsync(cancellationToken);
            await RefreshTodayMoodAsync(cancellationToken);
            IEnumerable<TechniqueItem> staticItems = await BuildStaticItemsAsync(cancellationToken);
            IEnumerable<TechniqueDTO> dynamicSource = await _techniqueService.GetTechniquesListAsync(500, cancellationToken);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                UpdateTodayRecommendation();
                ApplyTodayTechniqueDateFromList(staticItems);
                Techniques.Clear();
                Techniques.AddRange(staticItems);
                Techniques.AddRange(dynamicSource.Select(ParseFromDb));
                SetDone();
            });
        }
        catch (OperationCanceledException)
        {
            await MainThread.InvokeOnMainThreadAsync(CancelProgress);
        }
        catch
        {
            await MainThread.InvokeOnMainThreadAsync(SetFail);
            _toastService.ShortToast(AppStrings.PracticeInitError);
        }
    }

    private void UpdateTodayRecommendation()
    {
        string concern = UserPreferences.Load().OnboardingConcern;
        _todayTechniqueId = OnboardingRecommendation.ResolveTechnique(concern);
        TechniqueDefinition definition = TechniqueCatalog.Get(_todayTechniqueId);
        TodayTechniqueItem = new TechniqueItem
        {
            Number = definition.ListNumber,
            Date = HasStreak ? StreakDisplay : definition.ListDate,
            Image = "method.png",
            Title = definition.ListTitle,
            Subtitle = definition.ListSubtitle,
            Theme = definition.Theme,
            Author = definition.Author,
            Active = true,
            TapCommand = new AsyncCommand(() => _navigationService.GoToTechniqueAsync(_todayTechniqueId))
        };
    }

    private void ApplyTodayTechniqueDateFromList(IEnumerable<TechniqueItem> staticItems)
    {
        if (TodayTechniqueItem is null || HasStreak)
        {
            return;
        }

        TechniqueListEntry entry = TechniqueListCatalog.GetBuiltIn().First(e => e.TechniqueId == _todayTechniqueId);
        TechniqueItem? match = staticItems.FirstOrDefault(item => item.Number == entry.Number);
        if (match is not null)
        {
            TodayTechniqueItem.Date = match.Date;
            OnPropertyChanged(nameof(TodayTechniqueItem));
        }
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

    private async Task RefreshTodayMoodAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MoodEntryDTO> moods = await _userProgressService.GetRecentMoodsAsync(1, cancellationToken);
        if (moods.Count == 0)
        {
            TodayMoodDisplay = string.Empty;
        }
        else
        {
            MoodEntryDTO latest = moods[0];
            DateTime local = latest.RecordedAt.ToLocalTime();
            TodayMoodDisplay = local.Date == DateTime.Today
                ? AppStrings.TodayMoodLine(latest.MoodLevel, 5)
                : string.Empty;
        }

        OnPropertyChanged(nameof(TodayMoodDisplay));
        OnPropertyChanged(nameof(HasTodayMood));
    }

    private async Task RecordMoodAsync(int moodLevel)
    {
        await _userProgressService.RecordMoodAsync(moodLevel);
        StreakDays = await _userProgressService.GetStreakDaysAsync();
        await RefreshTodayMoodAsync();
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
