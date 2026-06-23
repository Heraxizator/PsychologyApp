using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.TechniqueTheory;

namespace PsychologyApp.Presentation.App.Providers;

public interface ITheoryViewModelFactory
{
    TheoryViewModel Create(INavigation navigation, string content, TechniqueId? techniqueId = null);
}

public sealed class TheoryViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory)
    : ViewModelFactoryBase, ITheoryViewModelFactory
{
    public TheoryViewModel Create(INavigation navigation, string content, TechniqueId? techniqueId = null) =>
        new(ResolveNavigation(navigationServiceFactory, navigation), content, techniqueId);
}
