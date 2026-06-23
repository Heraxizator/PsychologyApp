using PsychologyApp.Presentation.Core.Common;

using PsychologyApp.Presentation.Core.Common;

namespace PsychologyApp.Presentation.Pages.Onboarding;

public partial class OnboardingViewModel
{
    private int _step;
    public int Step
    {
        get => _step;
        set
        {
            if (SetProperty(ref _step, value))
            {
                OnPropertyChanged(nameof(IsWelcomeStep));
                OnPropertyChanged(nameof(IsConcernStep));
                OnPropertyChanged(nameof(IsDisclaimerStep));
            }
        }
    }

    public bool IsWelcomeStep => Step == 0;
    public bool IsConcernStep => Step == 1;
    public bool IsDisclaimerStep => Step == 2;

    private string _selectedConcern = OnboardingConcernKeys.Default;
    public string SelectedConcern
    {
        get => _selectedConcern;
        set
        {
            if (SetProperty(ref _selectedConcern, value))
            {
                NotifyConcernSelection();
            }
        }
    }
}
