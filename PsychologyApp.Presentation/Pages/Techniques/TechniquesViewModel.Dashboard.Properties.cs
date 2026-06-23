using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Pages.Techniques;

public partial class TechniquesViewModel
{
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
}
