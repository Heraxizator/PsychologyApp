using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.UI.Components;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.PracticeCompletion;

public sealed class PracticeCompletionViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IUserProgressService _userProgressService;
    private readonly NextPracticeResolver _nextPracticeResolver;
    private TechniqueId? _nextTechniqueId;
    private long? _sessionResultId;
    private int? _preIntensity;

    public PracticeCompletionViewModel(
        INavigationService navigationService,
        IUserProgressService userProgressService,
        NextPracticeResolver nextPracticeResolver,
        int streakDays,
        string? completedItemKey = null,
        long? sessionResultId = null)
    {
        _navigationService = navigationService;
        _userProgressService = userProgressService;
        _nextPracticeResolver = nextPracticeResolver;
        _sessionResultId = sessionResultId;
        StreakDays = streakDays;
        RecordMoodCommand = new Command<object?>(parameter =>
        {
            if (parameter is int moodLevel)
            {
                RecordMoodAsync(moodLevel).FireAndForget();
            }
            else if (parameter is string text && int.TryParse(text, out int parsed))
            {
                RecordMoodAsync(parsed).FireAndForget();
            }
        });
        MorePracticeCommand = new AsyncCommand(MorePracticeAsync);
        NextPracticeCommand = new AsyncCommand(StartNextPracticeAsync);
        GoHomeCommand = new AsyncCommand(() => _navigationService.GoToRootAsync());
        LoadBeforeMoodAsync().FireAndForget();
        LoadSessionOutcomeAsync().FireAndForget();
        LoadNextPracticeAsync(completedItemKey).FireAndForget();
    }

    public int StreakDays { get; }

    public bool IsMilestone => AppStrings.IsStreakMilestone(StreakDays);
    public string CelebrationIconName => StreakDays switch
    {
        3 => MaterialIconNames.Whatshot,
        7 => MaterialIconNames.AutoAwesome,
        14 => MaterialIconNames.EmojiEvents,
        30 => MaterialIconNames.WorkspacePremium,
        _ => MaterialIconNames.CheckCircle
    };
    public string TitleText => IsMilestone
        ? AppStrings.PracticeMilestoneTitle(StreakDays)
        : AppStrings.PracticeCompletedTitle;
    public string BodyText => IsMilestone
        ? AppStrings.PracticeMilestoneBody(StreakDays)
        : AppStrings.PracticeCompletedBody(StreakDays);
    public string StreakValueText => StreakDays > 0 ? AppStrings.ProfileStreakCount(StreakDays) : string.Empty;
    public string StreakLabelText => AppStrings.ProfileStreakDays;
    public bool HasStreak => StreakDays > 0;
    public string MorePracticeText => AppStrings.PracticeMoreButton;
    public string GoHomeText => AppStrings.PracticeGoHomeButton;
    public string ReflectionQuestion => AppStrings.PracticeReflectionQuestion;
    public string ReflectionNotePlaceholder => AppStrings.PracticeReflectionNotePlaceholder;
    public string PostSudsLabel => AppStrings.PracticePostSudsLabel;
    public string SudsSectionTitle => AppStrings.PracticeSudsSectionTitle;
    public string MoodSectionTitle => AppStrings.PracticeReflectionSectionTitle;

    private string _postIntensityText = string.Empty;
    public string PostIntensityText
    {
        get => _postIntensityText;
        set
        {
            if (SetProperty(ref _postIntensityText, value)
                && int.TryParse(value, out int parsed)
                && parsed is >= 0 and <= 10)
            {
                SavePostIntensityAsync(parsed).FireAndForget();
                NotifySudsDelta();
            }
        }
    }

    public bool HasSudsDelta =>
        _preIntensity is >= 0 and <= 10
        && int.TryParse(_postIntensityText, out int post)
        && post is >= 0 and <= 10;

    public string SudsDeltaText => HasSudsDelta && int.TryParse(_postIntensityText, out int post) && _preIntensity is int pre
        ? AppStrings.PracticeSudsDelta(pre, post)
        : string.Empty;

    private bool _hasNextPractice;
    public bool HasNextPractice
    {
        get => _hasNextPractice;
        private set => SetProperty(ref _hasNextPractice, value);
    }

    private string _nextPracticeCaption = string.Empty;
    public string NextPracticeCaption
    {
        get => _nextPracticeCaption;
        private set => SetProperty(ref _nextPracticeCaption, value);
    }

    private string _nextPracticeTitle = string.Empty;
    public string NextPracticeTitle
    {
        get => _nextPracticeTitle;
        private set => SetProperty(ref _nextPracticeTitle, value);
    }

    private string _nextPracticeSubtitle = string.Empty;
    public string NextPracticeSubtitle
    {
        get => _nextPracticeSubtitle;
        private set => SetProperty(ref _nextPracticeSubtitle, value);
    }

    private string _nextPracticeReason = string.Empty;
    public string NextPracticeReason
    {
        get => _nextPracticeReason;
        private set => SetProperty(ref _nextPracticeReason, value);
    }

    private string _nextPracticeIcon = string.Empty;
    public string NextPracticeIcon
    {
        get => _nextPracticeIcon;
        private set => SetProperty(ref _nextPracticeIcon, value);
    }

    private string _nextPracticeActionText = string.Empty;
    public string NextPracticeActionText
    {
        get => _nextPracticeActionText;
        private set => SetProperty(ref _nextPracticeActionText, value);
    }

    private int _beforeMoodLevel;
    public int BeforeMoodLevel
    {
        get => _beforeMoodLevel;
        private set
        {
            if (SetProperty(ref _beforeMoodLevel, value))
            {
                NotifyMoodDelta();
            }
        }
    }

    private int _selectedMoodLevel;
    public int SelectedMoodLevel
    {
        get => _selectedMoodLevel;
        private set
        {
            if (SetProperty(ref _selectedMoodLevel, value))
            {
                NotifyMoodDelta();
            }
        }
    }

    public bool HasMoodDelta =>
        BeforeMoodLevel >= 1 && SelectedMoodLevel >= 1 && BeforeMoodLevel != SelectedMoodLevel;

    public string MoodDeltaText => HasMoodDelta
        ? AppStrings.PracticeMoodDelta(BeforeMoodLevel, SelectedMoodLevel)
        : string.Empty;

    private string _reflectionNote = string.Empty;
    public string ReflectionNote
    {
        get => _reflectionNote;
        set => SetProperty(ref _reflectionNote, value);
    }

    public ICommand RecordMoodCommand { get; }
    public ICommand MorePracticeCommand { get; }
    public ICommand NextPracticeCommand { get; }
    public ICommand GoHomeCommand { get; }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(IsMilestone),
            nameof(CelebrationIconName),
            nameof(TitleText),
            nameof(BodyText),
            nameof(StreakValueText),
            nameof(StreakLabelText),
            nameof(HasStreak),
            nameof(MorePracticeText),
            nameof(GoHomeText),
            nameof(ReflectionQuestion),
            nameof(ReflectionNotePlaceholder),
            nameof(PostSudsLabel),
            nameof(SudsSectionTitle),
            nameof(MoodSectionTitle),
            nameof(HasSudsDelta),
            nameof(SudsDeltaText),
            nameof(HasMoodDelta),
            nameof(MoodDeltaText),
            nameof(NextPracticeCaption),
            nameof(NextPracticeReason),
            nameof(NextPracticeActionText));
    }

    private void NotifyMoodDelta()
    {
        OnPropertyChanged(nameof(HasMoodDelta));
        OnPropertyChanged(nameof(MoodDeltaText));
    }

    private void NotifySudsDelta()
    {
        OnPropertyChanged(nameof(HasSudsDelta));
        OnPropertyChanged(nameof(SudsDeltaText));
    }

    private async Task LoadSessionOutcomeAsync()
    {
        if (_sessionResultId is null)
        {
            return;
        }

        try
        {
            SessionResultDTO? result = await _userProgressService.GetSessionResultAsync(_sessionResultId.Value);
            if (result?.PreIntensity is >= 0 and <= 10)
            {
                _preIntensity = result.PreIntensity.Value;
                NotifySudsDelta();
            }

            if (result?.PostIntensity is >= 0 and <= 10)
            {
                _postIntensityText = result.PostIntensity.Value.ToString();
                OnPropertyChanged(nameof(PostIntensityText));
                NotifySudsDelta();
            }
        }
        catch
        {
            // Pre-SUDS is optional on the completion screen.
        }
    }

    private async Task SavePostIntensityAsync(int postIntensity)
    {
        if (_sessionResultId is null)
        {
            return;
        }

        try
        {
            await _userProgressService.UpdateSessionResultPostIntensityAsync(_sessionResultId.Value, postIntensity);
        }
        catch
        {
            // Post-SUDS is optional; completion still works without persistence.
        }
    }

    private async Task LoadBeforeMoodAsync()
    {
        try
        {
            IReadOnlyList<MoodEntryDTO> recent = await _userProgressService.GetRecentMoodsAsync(3);
            MoodEntryDTO? today = recent.FirstOrDefault(entry => entry.RecordedAt.ToLocalTime().Date == DateTime.Today);
            if (today is not null && today.MoodLevel >= 1)
            {
                BeforeMoodLevel = today.MoodLevel;
            }
        }
        catch
        {
            // Pre-mood is optional; completion still works without delta.
        }
    }

    private async Task LoadNextPracticeAsync(string? completedItemKey)
    {
        if (string.IsNullOrWhiteSpace(completedItemKey)
            || completedItemKey.StartsWith("custom_", StringComparison.OrdinalIgnoreCase)
            || !Enum.TryParse(completedItemKey, out TechniqueId completedTechniqueId))
        {
            return;
        }

        try
        {
            NextPracticeResult? result = await _nextPracticeResolver.ResolveAsync(completedTechniqueId);
            if (result is null)
            {
                return;
            }

            _nextTechniqueId = result.TechniqueId;
            NextPracticeCaption = result.Caption;
            NextPracticeTitle = result.Title;
            NextPracticeSubtitle = result.Subtitle;
            NextPracticeReason = result.ReasonText;
            NextPracticeIcon = result.IconName;
            NextPracticeActionText = result.ActionText;
            HasNextPractice = true;
        }
        catch
        {
            // Next practice is optional; completion still works without it.
        }
    }

    private async Task RecordMoodAsync(int moodLevel)
    {
        SelectedMoodLevel = moodLevel;
        string? note = string.IsNullOrWhiteSpace(ReflectionNote) ? null : ReflectionNote.Trim();
        await _userProgressService.RecordMoodAsync(moodLevel, note);
    }

    private async Task MorePracticeAsync()
    {
        if (HasNextPractice && _nextTechniqueId is not null)
        {
            await StartNextPracticeAsync();
            return;
        }

        await _navigationService.GoBackAsync();
        await _navigationService.GoBackAsync();
    }

    private async Task StartNextPracticeAsync()
    {
        if (!HasNextPractice || _nextTechniqueId is null)
        {
            return;
        }

        await _navigationService.GoBackAsync();
        await _navigationService.GoBackAsync();
        await _navigationService.GoToTechniqueAsync(_nextTechniqueId.Value);
    }
}
