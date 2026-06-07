using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.TechniquePages;

public partial class TheoryPage : ContentPage
{
    public TheoryPage(ITheoryViewModelFactory theoryViewModelFactory, string content, TechniqueId? techniqueId = null)
    {
        InitializeComponent();
        BindingContext = theoryViewModelFactory.Create(Navigation, content, techniqueId);
    }
}
