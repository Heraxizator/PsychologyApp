using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Exposure;
using PsychologyApp.Application.Body;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.ClinicalCare.CrisisHub;
using PsychologyApp.Presentation.Pages.ClinicalCare.RiskCheck;
using PsychologyApp.Presentation.Pages.ClinicalCare.SafetyPlan;
using PsychologyApp.Presentation.Pages.ClinicalCare.Exposure;
using PsychologyApp.Presentation.Pages.ClinicalCare.BodyCheck;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.ClinicalCare;

public interface IClinicalCarePageFactory
{
    CrisisHubPage CreateCrisisHubPage();
    RiskCheckPage CreateRiskCheckPage(string source, Func<RiskAssessmentDTO, Task>? onCompleted = null);
    SafetyPlanPage CreateSafetyPlanPage();
    ExposurePage CreateExposurePage();
    BodyCheckPage CreateBodyCheckPage();
}

public interface ICrisisHubViewModelFactory
{
    CrisisHubViewModel Create(ContentPage page);
}

public interface ISafetyPlanViewModelFactory
{
    SafetyPlanViewModel Create(ContentPage page);
}

public interface IExposureViewModelFactory
{
    ExposureViewModel Create(ContentPage page);
}

public interface IBodyCheckViewModelFactory
{
    BodyCheckViewModel Create(ContentPage page);
}

public interface IRiskCheckViewModelFactory
{
    RiskCheckViewModel Create(ContentPage page, string source, Func<RiskAssessmentDTO, Task>? onCompleted = null);
}

public sealed class CrisisHubViewModelFactory(
    IClinicalCareService clinicalCareService,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, ICrisisHubViewModelFactory
{
    public CrisisHubViewModel Create(ContentPage page) =>
        new(ResolveNavigation(navigationServiceFactory, page), clinicalCareService);
}

public sealed class RiskCheckViewModelFactory(
    IClinicalCareService clinicalCareService,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IRiskCheckViewModelFactory
{
    public RiskCheckViewModel Create(
        ContentPage page,
        string source,
        Func<RiskAssessmentDTO, Task>? onCompleted = null) =>
        new(ResolveNavigation(navigationServiceFactory, page), clinicalCareService, source, onCompleted);
}

public sealed class SafetyPlanViewModelFactory(
    IClinicalCareService clinicalCareService,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, ISafetyPlanViewModelFactory
{
    public SafetyPlanViewModel Create(ContentPage page) =>
        new(ResolveNavigation(navigationServiceFactory, page), clinicalCareService);
}

public sealed class ExposureViewModelFactory(
    IExposureService exposureService,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IExposureViewModelFactory
{
    public ExposureViewModel Create(ContentPage page) =>
        new(ResolveNavigation(navigationServiceFactory, page), exposureService);
}

public sealed class BodyCheckViewModelFactory(
    IBodyService bodyService,
    IDialogService dialogs,
    IDatabaseReadySignal databaseReady,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IBodyCheckViewModelFactory
{
    public BodyCheckViewModel Create(ContentPage page) =>
        new(ResolveNavigation(navigationServiceFactory, page), bodyService, dialogs, databaseReady);
}

public sealed class ClinicalCarePageFactory(
    ICrisisHubViewModelFactory crisisHubViewModelFactory,
    IRiskCheckViewModelFactory riskCheckViewModelFactory,
    ISafetyPlanViewModelFactory safetyPlanViewModelFactory,
    IExposureViewModelFactory exposureViewModelFactory,
    IBodyCheckViewModelFactory bodyCheckViewModelFactory) : IClinicalCarePageFactory
{
    public CrisisHubPage CreateCrisisHubPage() =>
        new(crisisHubViewModelFactory);

    public RiskCheckPage CreateRiskCheckPage(string source, Func<RiskAssessmentDTO, Task>? onCompleted = null) =>
        new(riskCheckViewModelFactory, source, onCompleted);

    public SafetyPlanPage CreateSafetyPlanPage() =>
        new(safetyPlanViewModelFactory);

    public ExposurePage CreateExposurePage() =>
        new(exposureViewModelFactory);

    public BodyCheckPage CreateBodyCheckPage() =>
        new(bodyCheckViewModelFactory);
}
