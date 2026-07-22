using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

public partial class UserViewModel
{
    private string _clinicalScorecardSummary = string.Empty;
    public string ClinicalScorecardSummary
    {
        get => _clinicalScorecardSummary;
        private set
        {
            if (SetProperty(ref _clinicalScorecardSummary, value))
            {
                OnPropertyChanged(nameof(HasClinicalScorecard));
            }
        }
    }

    public bool HasClinicalScorecard => !string.IsNullOrWhiteSpace(ClinicalScorecardSummary);
    public string ClinicalScorecardTitle => AppStrings.ClinicalScorecardTitle;

    private async Task RefreshClinicalScorecardAsync(CancellationToken cancellationToken)
    {
        try
        {
            ClinicalScorecardDTO scorecard = await _clinicalCareService.BuildWeeklyScorecardAsync(cancellationToken);
            string risk = scorecard.RiskLevel switch
            {
                RiskLevel.Red => AppStrings.ClinicalRiskRed,
                RiskLevel.Amber => AppStrings.ClinicalRiskAmber,
                _ => AppStrings.ClinicalRiskGreen
            };
            ClinicalScorecardSummary = scorecard.PracticeCount == 0 && scorecard.MoodEntriesCount == 0
                ? AppStrings.ClinicalScorecardEmpty
                : AppStrings.ClinicalScorecardSummary(scorecard.PracticeCount, scorecard.MoodEntriesCount, risk);
        }
        catch
        {
            ClinicalScorecardSummary = string.Empty;
        }
    }
}
