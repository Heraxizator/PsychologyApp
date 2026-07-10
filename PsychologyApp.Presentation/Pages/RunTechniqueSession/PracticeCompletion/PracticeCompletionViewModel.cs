using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
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

    public PracticeCompletionViewModel(
        INavigationService navigationService,
        IUserProgressService userProgressService,
        int streakDays)
    {
        _navigationService = navigationService;
        _userProgressService = userProgressService;
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
        GoHomeCommand = new AsyncCommand(() => _navigationService.GoToRootAsync());
        LoadBeforeMoodAsync().FireAndForget();
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
            nameof(HasMoodDelta),
            nameof(MoodDeltaText));
    }

    private void NotifyMoodDelta()
    {
        OnPropertyChanged(nameof(HasMoodDelta));
        OnPropertyChanged(nameof(MoodDeltaText));
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

    private async Task RecordMoodAsync(int moodLevel)
    {
        SelectedMoodLevel = moodLevel;
        string? note = string.IsNullOrWhiteSpace(ReflectionNote) ? null : ReflectionNote.Trim();
        await _userProgressService.RecordMoodAsync(moodLevel, note);
    }

    private async Task MorePracticeAsync()
    {
        await _navigationService.GoBackAsync();
        await _navigationService.GoBackAsync();
    }
}
