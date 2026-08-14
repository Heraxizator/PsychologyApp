using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesViewModel
{
    public string TodayReasonText { get; private set; } = string.Empty;

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
            : !string.IsNullOrWhiteSpace(TodayTechniqueItem?.Title)
                ? AppStrings.ComebackBannerWithTechnique(TodayTechniqueItem.Title)
                : AppStrings.ComebackBanner;

    public string TodayActionText =>
        HasTodayDraft ? AppStrings.TechniqueContinueBadge : AppStrings.TodayStartPractice;

    private TechniqueItem? _todayTechniqueItem;
    public TechniqueItem? TodayTechniqueItem
    {
        get => _todayTechniqueItem;
        private set
        {
            if (SetProperty(ref _todayTechniqueItem, value))
            {
                NotifyEngagementNudge();
            }
        }
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
}
