using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.PracticeCompletion;

public partial class PracticeCompletionPage : ContentPage
{
    public PracticeCompletionPage(IPracticeCompletionViewModelFactory factory, int streakDays)
    {
        BindingContext = factory.Create(this, streakDays);
        InitializeComponent();
    }
}
