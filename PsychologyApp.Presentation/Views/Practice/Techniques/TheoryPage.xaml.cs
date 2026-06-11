using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Practice.Techniques;

public partial class TheoryPage : ContentPage
{
    public TheoryPage(ITheoryViewModelFactory theoryViewModelFactory, string content, TechniqueId? techniqueId = null)
    {
        InitializeComponent();
        BindingContext = theoryViewModelFactory.Create(Navigation, content, techniqueId);
    }
}
