using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.TechniqueTheory;

namespace PsychologyApp.Presentation.App.Providers;

public interface ITheoryViewModelFactory
{
    TheoryViewModel Create(ContentPage page, string content, TechniqueId? techniqueId = null);
}

public sealed class TheoryViewModelFactory(
    TechniqueCatalogGateway techniqueCatalog,
    Func<NavigationContext, INavigationService> navigationServiceFactory)
    : ViewModelFactoryBase, ITheoryViewModelFactory
{
    public TheoryViewModel Create(ContentPage page, string content, TechniqueId? techniqueId = null) =>
        new(ResolveNavigation(navigationServiceFactory, page), techniqueCatalog, content, techniqueId);
}
