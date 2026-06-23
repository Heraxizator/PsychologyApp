using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Pages.TechniqueTheory;

namespace PsychologyApp.Presentation.App.Routes;

public interface IPracticeTheoryNavigator
{
    TheoryPage CreateTheoryPage(string content, TechniqueId? techniqueId = null);
}
