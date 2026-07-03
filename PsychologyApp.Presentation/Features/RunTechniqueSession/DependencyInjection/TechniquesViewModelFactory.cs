using PsychologyApp.Presentation.App.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Technique;
using PsychologyApp.Application.UserProgress;
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
    IUserProgressService userProgressService,
    TechniqueListBuilder techniqueListBuilder,
    IDatabaseReadySignal databaseReadySignal,
        PracticeDashboardLoader dashboardLoader,
        TechniquesDashboardPresenter dashboardPresenter,
        TodayRecommendationResolver todayRecommendationResolver,
    TechniquesListInitializer listInitializer,
    IOptions<AppSettings> settings,
    ILogger<TechniquesViewModel> logger) : ViewModelFactoryBase, ITechniquesViewModelFactory
{
    public TechniquesViewModel Create(ContentPage page) =>
        new(
            techniqueService,
            toastService,
            techniqueMessenger,
            ResolveNavigation(navigationServiceFactory, page),
            userProgressService,
            techniqueListBuilder,
            databaseReadySignal,
            dashboardLoader,
            dashboardPresenter,
            todayRecommendationResolver,
            listInitializer,
            settings,
            logger);
}
