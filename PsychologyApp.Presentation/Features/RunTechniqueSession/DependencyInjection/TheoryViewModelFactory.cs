using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

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
