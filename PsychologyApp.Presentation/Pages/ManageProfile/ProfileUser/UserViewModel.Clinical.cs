using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

public partial class UserViewModel
{
    private bool _hasClinicalScorecard;
    public bool HasClinicalScorecard
    {
        get => _hasClinicalScorecard;
        private set => SetProperty(ref _hasClinicalScorecard, value);
    }

    private bool _hasClinicalScorecardMetrics;
    public bool HasClinicalScorecardMetrics
    {
        get => _hasClinicalScorecardMetrics;
        private set
        {
            if (SetProperty(ref _hasClinicalScorecardMetrics, value))
            {
                OnPropertyChanged(nameof(ShowClinicalScorecardEmpty));
            }
        }
    }

    public bool ShowClinicalScorecardEmpty => HasClinicalScorecard && !HasClinicalScorecardMetrics;

    public string ClinicalScorecardTitle => AppStrings.ClinicalScorecardTitle;
    public string ClinicalScorecardEmpty => AppStrings.ClinicalScorecardEmpty;
    public string WeekPracticesLabel => AppStrings.WeekPracticesLabel;
    public string WeekAvgMoodLabel => AppStrings.WeekAvgMoodLabel;
    public string WeekRiskLabel => AppStrings.WeekRiskLabel;

    private string _weekRangeSubtitle = string.Empty;
    public string WeekRangeSubtitle
    {
        get => _weekRangeSubtitle;
        private set => SetProperty(ref _weekRangeSubtitle, value);
    }

    private string _weekPracticeCount = AppStrings.MetricEmptyValue;
    public string WeekPracticeCount
    {
        get => _weekPracticeCount;
        private set => SetProperty(ref _weekPracticeCount, value);
    }

    private string _weekAverageMoodDisplay = AppStrings.MetricEmptyValue;
    public string WeekAverageMoodDisplay
    {
        get => _weekAverageMoodDisplay;
        private set => SetProperty(ref _weekAverageMoodDisplay, value);
    }

    private string _weekRiskLabel = string.Empty;
    public string WeekRiskDisplay
    {
        get => _weekRiskLabel;
        private set => SetProperty(ref _weekRiskLabel, value);
    }

    private async Task RefreshClinicalScorecardAsync(CancellationToken cancellationToken)
    {
        try
        {
            ClinicalScorecardDTO scorecard = await _clinicalCareService.BuildWeeklyScorecardAsync(cancellationToken);
            bool hasMetrics = scorecard.PracticeCount > 0 || scorecard.MoodEntriesCount > 0;
            string risk = scorecard.RiskLevel switch
            {
                RiskLevel.Red => AppStrings.ClinicalRiskRed,
                RiskLevel.Amber => AppStrings.ClinicalRiskAmber,
                _ => AppStrings.ClinicalRiskGreen
            };

            WeekRangeSubtitle = AppStrings.WeekRangeLabel(scorecard.WeekStart, scorecard.WeekEnd);
            WeekPracticeCount = hasMetrics ? scorecard.PracticeCount.ToString() : AppStrings.MetricEmptyValue;
            WeekAverageMoodDisplay = scorecard.MoodEntriesCount > 0
                ? AppStrings.FormatAverageMood(scorecard.AverageMoodLevel)
                : AppStrings.MetricEmptyValue;
            WeekRiskDisplay = hasMetrics ? risk : AppStrings.MetricEmptyValue;
            HasClinicalScorecardMetrics = hasMetrics;
            HasClinicalScorecard = true;
        }
        catch
        {
            HasClinicalScorecard = true;
            HasClinicalScorecardMetrics = false;
            WeekRangeSubtitle = string.Empty;
            WeekPracticeCount = AppStrings.MetricEmptyValue;
            WeekAverageMoodDisplay = AppStrings.MetricEmptyValue;
            WeekRiskDisplay = AppStrings.MetricEmptyValue;
        }
    }
}
