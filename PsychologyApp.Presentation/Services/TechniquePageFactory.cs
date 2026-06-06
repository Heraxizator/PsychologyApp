using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.Views;
using PsychologyApp.Presentation.Views.TechniquePages;
using PsychologyApp.Presentation.Views.TechniquePages.ConstructorPages;

namespace PsychologyApp.Presentation.Services;

public interface ITechniquePageFactory
{
    TechniquesPage CreateTechniquesPage();
    CreatedPage CreateCreatedPage(long techniqueId);
    DesignerPage CreateDesignerPage(long techniqueId);
    TechniqueSessionPage CreateTechniqueSessionPage(TechniqueId techniqueId);
}

public sealed class TechniquePageFactory(
    IPageViewModelActivator pageViewModelActivator,
    ITechniquesViewModelFactory techniquesViewModelFactory,
    ICreatedViewModelFactory createdViewModelFactory,
    IDesignerViewModelFactory designerViewModelFactory,
    ITechniqueViewModelFactory techniqueViewModelFactory,
    IPageAnalyticsService pageAnalyticsService,
    Func<INavigation, INavigationService> navigationServiceFactory) : ITechniquePageFactory
{
    public TechniquesPage CreateTechniquesPage() =>
        new(pageViewModelActivator, techniquesViewModelFactory);

    public CreatedPage CreateCreatedPage(long techniqueId) =>
        new(pageViewModelActivator, createdViewModelFactory, techniqueId);

    public DesignerPage CreateDesignerPage(long techniqueId) =>
        new(pageViewModelActivator, designerViewModelFactory, techniqueId);

    public TechniqueSessionPage CreateTechniqueSessionPage(TechniqueId techniqueId) =>
        new(techniqueViewModelFactory, pageAnalyticsService, techniqueId);
}
