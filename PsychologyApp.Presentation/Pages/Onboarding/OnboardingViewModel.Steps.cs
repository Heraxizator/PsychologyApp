using PsychologyApp.Presentation.Core.Common;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingViewModel
{
    public const int StepCount = 4;

    private int _step;
    public int Step
    {
        get => _step;
        set
        {
            if (SetProperty(ref _step, value))
            {
                NotifyStepVisibility();
            }
        }
    }

    public bool IsWelcomeStep => Step == 0;
    public bool IsOverviewStep => Step == 1;
    public bool IsConcernStep => Step == 2;
    public bool IsFinishStep => Step == 3;
    public bool IsBackVisible => Step > 0;
    public bool IsNextFooterVisible => Step < 2;
    public bool IsFinishFooterVisible => Step == 3;

    private string _selectedConcern = string.Empty;
    public string SelectedConcern
    {
        get => _selectedConcern;
        set
        {
            if (SetProperty(ref _selectedConcern, value))
            {
                NotifyConcernSelection();
                NotifyRecommendation();
            }
        }
    }

    private void NotifyStepVisibility()
    {
        Notify(
            nameof(IsWelcomeStep),
            nameof(IsOverviewStep),
            nameof(IsConcernStep),
            nameof(IsFinishStep),
            nameof(IsBackVisible),
            nameof(IsNextFooterVisible),
            nameof(IsFinishFooterVisible),
            nameof(StepLabel));
    }
}
