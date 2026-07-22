using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Features.ClinicalCare;

namespace PsychologyApp.Presentation.Pages.ClinicalCare.RiskCheck;

public partial class RiskCheckPage : ContentPage
{
    public RiskCheckPage(
        IRiskCheckViewModelFactory viewModelFactory,
        string source,
        Func<RiskAssessmentDTO, Task>? onCompleted = null)
    {
        InitializeComponent();
        BindingContext = viewModelFactory.Create(this, source, onCompleted);
    }
}
