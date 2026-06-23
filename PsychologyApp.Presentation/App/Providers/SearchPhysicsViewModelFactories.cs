using PsychologyApp.Presentation.Pages.StartPhysics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Reason;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.SearchPhysics;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Pages.PhysicsSearch;

namespace PsychologyApp.Presentation.App.Providers;

public interface IPhysicsSearchViewModelFactory
{
    PhysicsSearchViewModel Create(INavigation navigation);
}

public sealed class PhysicsSearchViewModelFactory(
    IReasonSearchService reasonSearchService,
    PhysicsSearchCoordinator searchCoordinator,
    Func<PhysicsSearchSession> searchSessionFactory,
    ILogger<PhysicsSearchViewModel> logger,
    IOptions<AppSettings> settings,
    IDatabaseReadySignal databaseReadySignal,
    Func<NavigationContext, INavigationService> navigationServiceFactory)
    : ViewModelFactoryBase, IPhysicsSearchViewModelFactory
{
    public PhysicsSearchViewModel Create(INavigation navigation) =>
        new(
            reasonSearchService,
            searchCoordinator,
            searchSessionFactory(),
            logger,
            settings,
            ResolveNavigation(navigationServiceFactory, navigation),
            databaseReadySignal);
}

public interface IStartPhysicsViewModelFactory
{
    StartPhysicsViewModel Create(INavigation navigation);
}

public sealed class StartPhysicsViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IStartPhysicsViewModelFactory
{
    public StartPhysicsViewModel Create(INavigation navigation) =>
        new(ResolveNavigation(navigationServiceFactory, navigation));
}
