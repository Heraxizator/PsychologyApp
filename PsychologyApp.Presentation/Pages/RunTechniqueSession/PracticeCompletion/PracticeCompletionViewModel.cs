using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
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
    }

    public int StreakDays { get; }

    public string TitleText => AppStrings.PracticeCompletedTitle;
    public string BodyText => AppStrings.PracticeCompletedBody(StreakDays);
    public string StreakValueText => StreakDays > 0 ? AppStrings.ProfileStreakCount(StreakDays) : string.Empty;
    public string StreakLabelText => AppStrings.ProfileStreakDays;
    public bool HasStreak => StreakDays > 0;
    public string MorePracticeText => AppStrings.PracticeMoreButton;
    public string GoHomeText => AppStrings.PracticeGoHomeButton;
    public string ReflectionQuestion => AppStrings.PracticeReflectionQuestion;
    public string ReflectionNotePlaceholder => AppStrings.PracticeReflectionNotePlaceholder;

    private int _selectedMoodLevel;
    public int SelectedMoodLevel
    {
        get => _selectedMoodLevel;
        private set => SetProperty(ref _selectedMoodLevel, value);
    }

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
            nameof(TitleText),
            nameof(BodyText),
            nameof(StreakValueText),
            nameof(StreakLabelText),
            nameof(HasStreak),
            nameof(MorePracticeText),
            nameof(GoHomeText),
            nameof(ReflectionQuestion),
            nameof(ReflectionNotePlaceholder));
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
