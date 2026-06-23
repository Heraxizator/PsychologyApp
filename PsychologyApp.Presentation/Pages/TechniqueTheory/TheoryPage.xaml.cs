using PsychologyApp.Presentation.App.Providers;

namespace PsychologyApp.Presentation.Pages.TechniqueTheory;

public partial class TheoryPage : ContentPage
{
    public TheoryPage(ITheoryViewModelFactory theoryViewModelFactory, string content, TechniqueId? techniqueId = null)
    {
        InitializeComponent();
        BindingContext = theoryViewModelFactory.Create(Navigation, content, techniqueId);
    }
}
