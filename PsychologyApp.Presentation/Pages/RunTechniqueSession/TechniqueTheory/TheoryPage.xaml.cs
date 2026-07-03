using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;

public partial class TheoryPage : ContentPage
{
    public TheoryPage(ITheoryViewModelFactory theoryViewModelFactory, string content, TechniqueId? techniqueId = null)
    {
        InitializeComponent();
        BindingContext = theoryViewModelFactory.Create(this, content, techniqueId);
    }
}
