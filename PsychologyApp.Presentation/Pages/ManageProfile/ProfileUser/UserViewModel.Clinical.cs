using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Entities.Journal;
using PsychologyApp.Presentation.Features.ManageProfile;
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

    private string _weekRangeSubtitle = string.Empty;
    public string WeekRangeSubtitle
    {
        get => _weekRangeSubtitle;
        private set => SetProperty(ref _weekRangeSubtitle, value);
    }

    private string _weekSummaryText = string.Empty;
    public string WeekSummaryText
    {
        get => _weekSummaryText;
        private set => SetProperty(ref _weekSummaryText, value);
    }

    private IReadOnlyList<JournalDayChip> _weekDays = [];
    public IReadOnlyList<JournalDayChip> WeekDays
    {
        get => _weekDays;
        private set => SetProperty(ref _weekDays, value);
    }

    private async Task RefreshClinicalScorecardAsync(CancellationToken cancellationToken)
    {
        try
        {
            Task<ClinicalScorecardDTO> scorecardTask =
                _clinicalCareService.BuildWeeklyScorecardAsync(cancellationToken);
            Task<IReadOnlyList<JournalDayChip>> weekTask =
                _profileWeekDaysLoader.LoadAsync(cancellationToken);

            await Task.WhenAll(scorecardTask, weekTask);

            ClinicalScorecardDTO scorecard = await scorecardTask;
            IReadOnlyList<JournalDayChip> weekDays = await weekTask;

            bool hasMetrics = scorecard.PracticeCount > 0 || scorecard.MoodEntriesCount > 0;
            string risk = scorecard.RiskLevel switch
            {
                RiskLevel.Red => AppStrings.ClinicalRiskRed,
                RiskLevel.Amber => AppStrings.ClinicalRiskAmber,
                _ => AppStrings.ClinicalRiskGreen
            };

            WeekRangeSubtitle = AppStrings.WeekRangeLabel(scorecard.WeekStart, scorecard.WeekEnd);
            WeekSummaryText = hasMetrics
                ? AppStrings.ClinicalScorecardSummary(
                    scorecard.PracticeCount,
                    scorecard.MoodEntriesCount,
                    risk)
                : string.Empty;
            WeekDays = weekDays;
            HasClinicalScorecardMetrics = hasMetrics;
            HasClinicalScorecard = true;
        }
        catch
        {
            HasClinicalScorecard = true;
            HasClinicalScorecardMetrics = false;
            WeekRangeSubtitle = string.Empty;
            WeekSummaryText = string.Empty;
            WeekDays = [];
        }
    }
}
