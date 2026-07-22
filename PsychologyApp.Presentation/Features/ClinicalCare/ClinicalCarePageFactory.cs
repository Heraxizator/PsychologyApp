using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.ClinicalCare.CrisisHub;
using PsychologyApp.Presentation.Pages.ClinicalCare.RiskCheck;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.ClinicalCare;

public interface IClinicalCarePageFactory
{
    CrisisHubPage CreateCrisisHubPage();
    RiskCheckPage CreateRiskCheckPage(string source, Func<RiskAssessmentDTO, Task>? onCompleted = null);
}

public interface ICrisisHubViewModelFactory
{
    CrisisHubViewModel Create(ContentPage page);
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

public sealed class ClinicalCarePageFactory(
    ICrisisHubViewModelFactory crisisHubViewModelFactory,
    IRiskCheckViewModelFactory riskCheckViewModelFactory) : IClinicalCarePageFactory
{
    public CrisisHubPage CreateCrisisHubPage() =>
        new(crisisHubViewModelFactory);

    public RiskCheckPage CreateRiskCheckPage(string source, Func<RiskAssessmentDTO, Task>? onCompleted = null) =>
        new(riskCheckViewModelFactory, source, onCompleted);
}
