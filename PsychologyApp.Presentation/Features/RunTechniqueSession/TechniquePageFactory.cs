using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueCreated;
using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDesigner;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.PracticeCompletion;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public interface ITechniquePageFactory
{
    TechniquesPage CreateTechniquesPage();
    CreatedPage CreateCreatedPage(long techniqueId);
    DesignerPage CreateDesignerPage(long techniqueId);
    TechniqueSessionPage CreateTechniqueSessionPage(TechniqueId techniqueId, INavigation hostNavigation);
    PracticeCompletionPage CreatePracticeCompletionPage(int streakDays);
}

public sealed class TechniquePageFactory(
    IPageViewModelActivator pageViewModelActivator,
    ITechniquesViewModelFactory techniquesViewModelFactory,
    ICreatedViewModelFactory createdViewModelFactory,
    IDesignerViewModelFactory designerViewModelFactory,
    ITechniqueViewModelFactory techniqueViewModelFactory,
    IPracticeCompletionViewModelFactory practiceCompletionViewModelFactory,
    IPageAnalyticsService pageAnalyticsService,
    TechniqueCatalogGateway techniqueCatalog) : ITechniquePageFactory
{
    public TechniquesPage CreateTechniquesPage() =>
        new(pageViewModelActivator, techniquesViewModelFactory);

    public CreatedPage CreateCreatedPage(long techniqueId) =>
        new(pageViewModelActivator, createdViewModelFactory, techniqueId);

    public DesignerPage CreateDesignerPage(long techniqueId) =>
        new(pageViewModelActivator, designerViewModelFactory, techniqueId);

    public TechniqueSessionPage CreateTechniqueSessionPage(TechniqueId techniqueId, INavigation hostNavigation) =>
        new(techniqueViewModelFactory, pageAnalyticsService, techniqueCatalog, techniqueId, hostNavigation);

    public PracticeCompletionPage CreatePracticeCompletionPage(int streakDays) =>
        new(practiceCompletionViewModelFactory, streakDays);
}
