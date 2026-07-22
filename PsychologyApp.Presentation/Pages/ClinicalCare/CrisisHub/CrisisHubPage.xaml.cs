using PsychologyApp.Presentation.Features.ClinicalCare;

namespace PsychologyApp.Presentation.Pages.ClinicalCare.CrisisHub;

public partial class CrisisHubPage : ContentPage
{
    public CrisisHubPage(ICrisisHubViewModelFactory viewModelFactory)
    {
        InitializeComponent();
        BindingContext = viewModelFactory.Create(this);
    }
}
