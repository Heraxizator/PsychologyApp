using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesViewModel
{
    public string TodayReasonText { get; private set; } = string.Empty;
    public string TodayMoodDisplay { get; private set; } = string.Empty;
    public bool HasTodayMood => !string.IsNullOrWhiteSpace(TodayMoodDisplay);
    public string MoodHistorySummary { get; private set; } = string.Empty;
    public bool HasMoodHistorySummary => !string.IsNullOrWhiteSpace(MoodHistorySummary);

    private string _weeklyInsightText = string.Empty;
    public string WeeklyInsightText
    {
        get => _weeklyInsightText;
        private set
        {
            if (SetProperty(ref _weeklyInsightText, value))
            {
                OnPropertyChanged(nameof(HasWeeklyInsight));
            }
        }
    }

    public bool HasWeeklyInsight => !string.IsNullOrWhiteSpace(WeeklyInsightText);

    private int _selectedMoodLevel;
    public int SelectedMoodLevel
    {
        get => _selectedMoodLevel;
        private set => SetProperty(ref _selectedMoodLevel, value);
    }

    public string StreakDisplay => AppStrings.ProfileStreakCount(StreakDays);
    public bool HasStreak => StreakDays > 0;

    private int _atRiskStreakDays;
    public int AtRiskStreakDays
    {
        get => _atRiskStreakDays;
        private set
        {
            if (SetProperty(ref _atRiskStreakDays, value))
            {
                NotifyEngagementNudge();
            }
        }
    }

    private int _idleDays;
    public int IdleDays
    {
        get => _idleDays;
        private set
        {
            if (SetProperty(ref _idleDays, value))
            {
                NotifyEngagementNudge();
            }
        }
    }

    private string? _lastTechniqueName;
    public string? LastTechniqueName
    {
        get => _lastTechniqueName;
        private set
        {
            if (SetProperty(ref _lastTechniqueName, value))
            {
                NotifyEngagementNudge();
            }
        }
    }

    private bool _hasTodayDraft;
    public bool HasTodayDraft
    {
        get => _hasTodayDraft;
        private set
        {
            if (SetProperty(ref _hasTodayDraft, value))
            {
                OnPropertyChanged(nameof(TodayActionText));
            }
        }
    }

    public bool ShowStreakAtRiskBanner => AtRiskStreakDays >= 1;
    public bool ShowComebackBanner => !ShowStreakAtRiskBanner && IdleDays >= 3;
    public bool ShowEngagementNudge => ShowStreakAtRiskBanner || ShowComebackBanner;

    public string EngagementNudgeText =>
        ShowStreakAtRiskBanner
            ? AppStrings.StreakAtRiskBanner(AtRiskStreakDays)
            : !string.IsNullOrWhiteSpace(LastTechniqueName)
                ? AppStrings.ComebackBannerWithTechnique(LastTechniqueName)
                : AppStrings.ComebackBanner;

    public string TodayActionText =>
        HasTodayDraft ? AppStrings.TechniqueContinueBadge : AppStrings.TodayStartPractice;

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

    private string _therapyProgramBanner = string.Empty;
    public string TherapyProgramBanner
    {
        get => _therapyProgramBanner;
        private set
        {
            if (SetProperty(ref _therapyProgramBanner, value))
            {
                OnPropertyChanged(nameof(HasTherapyProgramBanner));
            }
        }
    }

    public bool HasTherapyProgramBanner => !string.IsNullOrWhiteSpace(TherapyProgramBanner);

    private string _clinicalRiskBanner = string.Empty;
    public string ClinicalRiskBanner
    {
        get => _clinicalRiskBanner;
        private set
        {
            if (SetProperty(ref _clinicalRiskBanner, value))
            {
                OnPropertyChanged(nameof(HasClinicalRiskBanner));
            }
        }
    }

    public bool HasClinicalRiskBanner => !string.IsNullOrWhiteSpace(ClinicalRiskBanner);

    private void NotifyEngagementNudge()
    {
        OnPropertyChanged(nameof(ShowStreakAtRiskBanner));
        OnPropertyChanged(nameof(ShowComebackBanner));
        OnPropertyChanged(nameof(ShowEngagementNudge));
        OnPropertyChanged(nameof(EngagementNudgeText));
    }

    private TechniqueId _todayTechniqueId = TechniqueId.Spin;

    private static string FormatProgramBanner(TherapyProgramStateDTO program, int completedInWeek = 0, int targetPractices = 0)
    {
        string name = program.ProgramType switch
        {
            TherapyProgramType.Anxiety => AppStrings.TherapyProgramAnxiety,
            TherapyProgramType.Mood => AppStrings.TherapyProgramMood,
            _ => AppStrings.TherapyProgramStress
        };
        string goal = AppStrings.TherapyProgramWeekGoal(program.CurrentWeek);
        return targetPractices > 0
            ? AppStrings.TherapyProgramBanner(name, program.CurrentWeek, goal, completedInWeek, targetPractices)
            : AppStrings.TherapyProgramBanner(name, program.CurrentWeek, goal);
    }

    private static string FormatRiskBanner(RiskLevel level) => level switch
    {
        RiskLevel.Red => AppStrings.ClinicalRedBanner,
        RiskLevel.Amber => AppStrings.ClinicalAmberBanner,
        _ => string.Empty
    };
}
