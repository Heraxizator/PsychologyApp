using Microsoft.Extensions.Options;
using MvvmHelpers;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services.Practice;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Models.Practice;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BaseViewModel = PsychologyApp.Presentation.ViewModels.BaseViewModel;

namespace PsychologyApp.Presentation.ViewModels.Practice;

public class TechniquesViewModel : BaseViewModel
{
    private readonly SemaphoreSlim _initGate = new(1, 1);
    private readonly ITechniqueService _techniqueService;
    private readonly IToastService _toastService;
    private readonly ITechniqueMessenger _techniqueMessenger;
    private readonly INavigationService _navigationService;
    private readonly IUserProgressService _userProgressService;
    private readonly IOptions<AppSettings> _settings;

    public ObservableCollection<TechniqueGroup> TechniqueGroups { get; private set; } = [];
    public ObservableCollection<TechniqueItem> CatalogTechniques { get; private set; } = [];

    private bool _isTechniquesGrouped;
    private object _techniquesItemsSource = new ObservableCollection<TechniqueItem>();

    public bool IsTechniquesGrouped
    {
        get => _isTechniquesGrouped;
        private set => SetProperty(ref _isTechniquesGrouped, value);
    }

    public object TechniquesItemsSource
    {
        get => _techniquesItemsSource;
        private set => SetProperty(ref _techniquesItemsSource, value);
    }
    public ICommand ConstructorTapped { get; private set; } = default!;
    public ICommand OpenProfileCommand { get; private set; } = default!;
    public ICommand StartTodayPracticeCommand { get; private set; } = default!;
    public ICommand RecordMoodCommand { get; private set; } = default!;

    public string PageTitle => AppStrings.PracticeHomeTitle;
    public string MyTechniquesLabel => AppStrings.PracticeMyTechniques;
    public string PracticeCatalogLabel => AppStrings.PracticeCatalog;
    public string CreateButtonText => AppStrings.PracticeCreate;
    public string ProfileToolbarText => AppStrings.ProfileTitle;
    public string TodayForYouLabel => AppStrings.TodayForYou;
    public string TodayStartPracticeText => AppStrings.TodayStartPractice;
    public string TodayMoodQuestion => AppStrings.TodayMoodQuestion;
    public string TodayReasonText { get; private set; } = string.Empty;
    public string TodayMoodDisplay { get; private set; } = string.Empty;
    public bool HasTodayMood => !string.IsNullOrWhiteSpace(TodayMoodDisplay);
    public string MoodHistorySummary { get; private set; } = string.Empty;
    public bool HasMoodHistorySummary => !string.IsNullOrWhiteSpace(MoodHistorySummary);

    private int _selectedMoodLevel;
    public int SelectedMoodLevel
    {
        get => _selectedMoodLevel;
        private set => SetProperty(ref _selectedMoodLevel, value);
    }
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
        INavigationService navigationService,
        IUserProgressService userProgressService,
        IOptions<AppSettings> settings)
    {
        _techniqueService = techniqueService;
        _toastService = toastService;
        _techniqueMessenger = techniqueMessenger;
        _navigationService = navigationService;
        _userProgressService = userProgressService;
        _settings = settings;
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.PracticeTechniquesList;

        ConstructorTapped = new AsyncCommand(() => _navigationService.GoToDesignerAsync(-1));
        OpenProfileCommand = new AsyncCommand(() => _navigationService.GoToUserProfileAsync());
        StartTodayPracticeCommand = new AsyncCommand(() => _navigationService.GoToTechniqueAsync(_todayTechniqueId));
        RecordMoodCommand = new Command<object?>(parameter =>
        {
            int level = parameter switch
            {
                int value => value,
                string text when int.TryParse(text, out int parsed) => parsed,
                _ => 0
            };

            if (level is >= 1 and <= 5)
            {
                RecordMoodAsync(level).FireAndForget();
            }
        });

        Cancel = new Command(CancelProgress);
        Reload = new AsyncCommand(() => InitializeAsync(showLoadingOverlay: true));

        SubscribeToTechniqueChanges();
        SetInit();
        InitializeAsync(showLoadingOverlay: true).FireAndForget();
    }

    public void SubscribeToTechniqueChanges() =>
        _techniqueMessenger.Subscribe(this, message => OnTechniqueMessageAsync(message).FireAndForget());

    public Task RefreshOnAppearAsync() =>
        InitializeAsync(showLoadingOverlay: false);

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(MyTechniquesLabel),
            nameof(PracticeCatalogLabel),
            nameof(CreateButtonText),
            nameof(ProfileToolbarText),
            nameof(TodayForYouLabel),
            nameof(TodayReasonText),
            nameof(TodayMoodQuestion),
            nameof(TodayMoodDisplay),
            nameof(HasTodayMood),
            nameof(MoodHistorySummary),
            nameof(HasMoodHistorySummary),
            nameof(StreakDisplay),
            nameof(PracticeEmptyTitle),
            nameof(PracticeEmptyBody),
            nameof(LoadingText),
            nameof(FailedText),
            nameof(RetryText));
        UpdateTodayRecommendation();
    }

    private async Task OnTechniqueMessageAsync(TechniqueMessage message)
    {
        if (message.MessageType is not (TechniqueMessageType.Add or TechniqueMessageType.Remove or TechniqueMessageType.Change))
        {
            return;
        }

        await InitializeAsync(showLoadingOverlay: false);
    }

    private async Task InitializeAsync(bool showLoadingOverlay)
    {
        await _initGate.WaitAsync();
        try
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            await InitAsync(timeoutSource.Token, showLoadingOverlay);
        }
        finally
        {
            _initGate.Release();
        }
    }

    public async Task InitAsync(CancellationToken cancellationToken, bool showLoadingOverlay = true)
    {
        try
        {
            if (showLoadingOverlay)
            {
                await MainThread.InvokeOnMainThreadAsync(SetInit);
            }

            await AppReadiness.DatabaseReadyAsync.WaitAsync(cancellationToken);

            StreakDays = await _userProgressService.GetStreakDaysAsync(cancellationToken);
            await RefreshTodayMoodAsync(cancellationToken);
            await RefreshMoodHistoryAsync(cancellationToken);
            List<TechniqueItem> staticItems = (await BuildStaticItemsAsync(cancellationToken)).ToList();
            List<TechniqueItem> customItems = (await _techniqueService.GetTechniquesListAsync(500, cancellationToken))
                .Select(ParseFromDb)
                .ToList();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                UpdateTodayRecommendation();
                ApplyTodayTechniqueDateFromList(staticItems);

                if (customItems.Count > 0)
                {
                    IsTechniquesGrouped = true;
                    CatalogTechniques = [];
                    TechniqueGroups =
                    [
                        new TechniqueGroup(string.Empty, staticItems),
                        new TechniqueGroup(MyTechniquesLabel, customItems)
                    ];
                    TechniquesItemsSource = TechniqueGroups;
                }
                else
                {
                    IsTechniquesGrouped = false;
                    TechniqueGroups = [];
                    CatalogTechniques = new ObservableCollection<TechniqueItem>(staticItems);
                    TechniquesItemsSource = CatalogTechniques;
                }

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
        string durationText = AppStrings.TechniqueDuration(definition.ListDurationMinutes);
        TodayReasonText = AppStrings.TodayRecommendationReason(concern);
        TodayTechniqueItem = new TechniqueItem
        {
            Number = definition.ListNumber,
            Date = HasStreak ? StreakDisplay : definition.ListDate,
            IconName = definition.ListIcon,
            DurationText = durationText,
            MetaText = AppStrings.TechniqueMetaLine(durationText, definition.Theme),
            Title = definition.ListTitle,
            Subtitle = definition.ListSubtitle,
            Theme = definition.Theme,
            Author = definition.Author,
            Active = true,
            TapCommand = new AsyncCommand(() => _navigationService.GoToTechniqueAsync(_todayTechniqueId))
        };
        OnPropertyChanged(nameof(TodayReasonText));
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
        List<TechniqueItem> items = [];

        foreach (TechniqueListEntry entry in TechniqueListCatalog.GetBuiltIn())
        {
            DateTime? lastPractice = await _userProgressService.GetLastPracticeDateAsync(entry.TechniqueId.ToString(), cancellationToken);
            string? draft = await _userProgressService.GetSessionDraftAsync(entry.TechniqueId.ToString(), cancellationToken);
            string durationText = AppStrings.TechniqueDuration(entry.DurationMinutes);
            string theme = !string.IsNullOrWhiteSpace(draft) ? AppStrings.TechniqueContinueBadge : entry.Theme;

            items.Add(new TechniqueItem
            {
                Number = entry.Number,
                Date = lastPractice is null
                    ? AppStrings.TechniqueNotTriedYet
                    : AppStrings.TechniqueLastPractice(lastPractice.Value.ToLocalTime().ToString("d")),
                IconName = entry.Icon,
                DurationText = durationText,
                MetaText = AppStrings.TechniqueMetaLine(durationText, theme),
                Title = entry.Title,
                Subtitle = entry.Subtitle,
                Theme = theme,
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
        IconName = "Build",
        Title = item.Header,
        Subtitle = item.Describtion,
        Theme = item.Subject,
        MetaText = item.Subject ?? string.Empty,
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
            SelectedMoodLevel = 0;
        }
        else
        {
            MoodEntryDTO latest = moods[0];
            DateTime local = latest.RecordedAt.ToLocalTime();
            if (local.Date == DateTime.Today)
            {
                TodayMoodDisplay = AppStrings.TodayMoodLine(latest.MoodLevel, 5);
                SelectedMoodLevel = latest.MoodLevel;
            }
            else
            {
                TodayMoodDisplay = string.Empty;
                SelectedMoodLevel = 0;
            }
        }

        OnPropertyChanged(nameof(TodayMoodDisplay));
        OnPropertyChanged(nameof(HasTodayMood));
    }

    private async Task RefreshMoodHistoryAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MoodEntryDTO> moods = await _userProgressService.GetRecentMoodsAsync(3, cancellationToken);

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            IEnumerable<MoodEntryDTO> summarySource = moods.Count > 0 && moods[0].RecordedAt.ToLocalTime().Date == DateTime.Today
                ? moods.Skip(1)
                : moods;

            string[] entries = summarySource
                .Take(2)
                .Select(mood => AppStrings.MoodHistoryEntry(mood.RecordedAt.ToLocalTime().ToString("d"), mood.MoodLevel, 5))
                .ToArray();

            MoodHistorySummary = entries.Length == 0 ? string.Empty : string.Join(" · ", entries);
            OnPropertyChanged(nameof(MoodHistorySummary));
            OnPropertyChanged(nameof(HasMoodHistorySummary));
        });
    }

    private async Task RecordMoodAsync(int moodLevel)
    {
        await _userProgressService.RecordMoodAsync(moodLevel);
        SelectedMoodLevel = moodLevel;
        StreakDays = await _userProgressService.GetStreakDaysAsync();
        await RefreshTodayMoodAsync();
        await RefreshMoodHistoryAsync();
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
