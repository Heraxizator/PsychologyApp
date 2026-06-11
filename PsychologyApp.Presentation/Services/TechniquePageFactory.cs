using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.Views.Practice;
using PsychologyApp.Presentation.Views.Practice.Techniques;
using PsychologyApp.Presentation.Views.Practice.Constructor;

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
    IPageAnalyticsService pageAnalyticsService) : ITechniquePageFactory
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
