using PsychologyApp.Presentation.Features.ClinicalCare;

namespace PsychologyApp.Presentation.Pages.ClinicalCare.SafetyPlan;

public partial class SafetyPlanPage : ContentPage
{
    public SafetyPlanPage(ISafetyPlanViewModelFactory viewModelFactory)
    {
        InitializeComponent();
        BindingContext = viewModelFactory.Create(this);
    }
}
