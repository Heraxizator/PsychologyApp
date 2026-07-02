using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;
using PsychologyApp.Presentation.Shared.Lib.Navigation;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.App.Routes;

public sealed class TheoryNavigationAdapter(IPracticeTheoryNavigator theoryNavigator) : INavigateToTheory
{
    public Task NavigateToTheoryAsync(string content, string? techniqueId = null)
    {
        TechniqueId? id = techniqueId is null ? null : Enum.Parse<TechniqueId>(techniqueId);
        TheoryPage page = theoryNavigator.CreateTheoryPage(content, id);
        return Shell.Current.Navigation.PushAsync(page);
    }
}
