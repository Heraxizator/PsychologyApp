using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Services.ReasonSearch;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels.Physics;

namespace PsychologyApp.Presentation.Services.Factories;

public interface IPhysicsSearchViewModelFactory
{
    PhysicsSearchViewModel Create(INavigation navigation);
}

public sealed class PhysicsSearchViewModelFactory(
    IReasonSearchService reasonSearchService,
    ILogger<PhysicsSearchViewModel> logger,
    IOptions<AppSettings> settings,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : IPhysicsSearchViewModelFactory
{
    public PhysicsSearchViewModel Create(INavigation navigation) =>
        new(navigation, reasonSearchService, logger, settings, navigationServiceFactory(NavigationContext.From(navigation)));
}

public interface IStartPhysicsViewModelFactory
{
    StartPhysicsViewModel Create(INavigation navigation);
}

public sealed class StartPhysicsViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : IStartPhysicsViewModelFactory
{
    public StartPhysicsViewModel Create(INavigation navigation) =>
        new(navigation, navigationServiceFactory(NavigationContext.From(navigation)));
}
