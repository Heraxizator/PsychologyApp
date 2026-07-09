using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

public interface ITheoryViewModelFactory
{
    TheoryViewModel Create(ContentPage page, string content, TechniqueId? techniqueId = null);

    Task<TheoryViewModel> CreateAsync(ContentPage page, string content, TechniqueId? techniqueId = null);
}

public sealed class TheoryViewModelFactory(
    TechniqueCatalogGateway techniqueCatalog,
    IOptions<AppSettings> settings,
    ILogger<TheoryViewModel> logger,
    Func<NavigationContext, INavigationService> navigationServiceFactory)
    : ViewModelFactoryBase, ITheoryViewModelFactory
{
    public TheoryViewModel Create(ContentPage page, string content, TechniqueId? techniqueId = null) =>
        new(
            ResolveNavigation(navigationServiceFactory, page),
            techniqueCatalog,
            content,
            techniqueId,
            settings,
            logger);

    public async Task<TheoryViewModel> CreateAsync(ContentPage page, string content, TechniqueId? techniqueId = null)
    {
        TheoryViewModel viewModel = Create(page, content, techniqueId);
        await viewModel.EnsureInitializedAsync();
        return viewModel;
    }
}
