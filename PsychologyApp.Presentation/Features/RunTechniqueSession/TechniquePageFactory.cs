using PsychologyApp.Presentation.Pages.TechniqueCreated;
using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.Techniques;
using PsychologyApp.Presentation.Pages.TechniqueSession;
using PsychologyApp.Presentation.Pages.TechniqueDesigner;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public interface ITechniquePageFactory
{
    TechniquesPage CreateTechniquesPage();
    CreatedPage CreateCreatedPage(long techniqueId);
    DesignerPage CreateDesignerPage(long techniqueId);
    TechniqueSessionPage CreateTechniqueSessionPage(TechniqueId techniqueId, INavigation hostNavigation);
}

public sealed class TechniquePageFactory(
    IPageViewModelActivator pageViewModelActivator,
    ITechniquesViewModelFactory techniquesViewModelFactory,
    ICreatedViewModelFactory createdViewModelFactory,
    IDesignerViewModelFactory designerViewModelFactory,
    ITechniqueViewModelFactory techniqueViewModelFactory,
    IPageAnalyticsService pageAnalyticsService) : ITechniquePageFactory
{
    public TechniquesPage CreateTechniquesPage() =>
        new(pageViewModelActivator, techniquesViewModelFactory);

    public CreatedPage CreateCreatedPage(long techniqueId) =>
        new(pageViewModelActivator, createdViewModelFactory, techniqueId);

    public DesignerPage CreateDesignerPage(long techniqueId) =>
        new(pageViewModelActivator, designerViewModelFactory, techniqueId);

    public TechniqueSessionPage CreateTechniqueSessionPage(TechniqueId techniqueId, INavigation hostNavigation) =>
        new(techniqueViewModelFactory, pageAnalyticsService, techniqueId, hostNavigation);
}
