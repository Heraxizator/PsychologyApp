using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.App.Routes;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed class PracticeTheoryNavigator(ITheoryViewModelFactory theoryViewModelFactory) : IPracticeTheoryNavigator
{
    public TheoryPage CreateTheoryPage(string content, TechniqueId? techniqueId = null) =>
        new(theoryViewModelFactory, content, techniqueId);
}
