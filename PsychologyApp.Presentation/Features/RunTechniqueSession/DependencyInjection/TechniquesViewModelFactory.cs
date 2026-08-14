using PsychologyApp.Presentation.App.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Technique;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

public interface ITechniquesViewModelFactory
{
    TechniquesViewModel Create(ContentPage page);
}

public sealed class TechniquesViewModelFactory(
    ITechniqueService techniqueService,
    IToastService toastService,
    ITechniqueMessenger techniqueMessenger,
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    TechniqueListBuilder techniqueListBuilder,
    IDatabaseReadySignal databaseReadySignal,
    PracticeDashboardLoader dashboardLoader,
    TodayRecommendationResolver todayRecommendationResolver,
    TechniquesListInitializer listInitializer,
    PracticeClinicalDashboardEnricher clinicalDashboardEnricher,
    IClinicalCareService clinicalCareService,
    IOptions<AppSettings> settings,
    ILogger<TechniquesViewModel> logger) : ViewModelFactoryBase, ITechniquesViewModelFactory
{
    public TechniquesViewModel Create(ContentPage page) =>
        new(
            techniqueService,
            toastService,
            techniqueMessenger,
            ResolveNavigation(navigationServiceFactory, page),
            techniqueListBuilder,
            databaseReadySignal,
            dashboardLoader,
            todayRecommendationResolver,
            listInitializer,
            clinicalDashboardEnricher,
            clinicalCareService,
            settings,
            logger);
}
