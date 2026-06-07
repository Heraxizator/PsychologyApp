using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Services.ReasonService;
using PsychologyApp.Presentation.ViewModels.Physics;

namespace PsychologyApp.Presentation.Services.Factories;

public interface IPhysicsSearchViewModelFactory
{
    PhysicsSearchViewModel Create(INavigation navigation);
}

public sealed class PhysicsSearchViewModelFactory(
    IReasonService reasonService,
    ILogger<PhysicsSearchViewModel> logger,
    IOptions<AppSettings> settings,
    Func<INavigation, INavigationService> navigationServiceFactory) : IPhysicsSearchViewModelFactory
{
    public PhysicsSearchViewModel Create(INavigation navigation) =>
        new(navigation, reasonService, logger, settings, navigationServiceFactory(navigation));
}

public interface IStartPhysicsViewModelFactory
{
    StartPhysicsViewModel Create(INavigation navigation);
}

public sealed class StartPhysicsViewModelFactory(Func<INavigation, INavigationService> navigationServiceFactory) : IStartPhysicsViewModelFactory
{
    public StartPhysicsViewModel Create(INavigation navigation) =>
        new(navigation, navigationServiceFactory(navigation));
}
