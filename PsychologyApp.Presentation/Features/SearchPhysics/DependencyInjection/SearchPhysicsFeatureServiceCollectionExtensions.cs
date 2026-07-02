using PsychologyApp.Presentation.App.DependencyInjection;
using PsychologyApp.Presentation.Features.SearchPhysics;

namespace PsychologyApp.Presentation.Features.SearchPhysics.DependencyInjection;

public static class SearchPhysicsFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddSearchPhysicsFeature(this IServiceCollection services)
    {
        services.AddSingleton<PhysicsSearchCoordinator>();
        SharedPresentationServiceCollectionExtensions.AddTransientFactory<PhysicsSearchSession>(services);
        services.AddSingleton<IPhysicsSearchViewModelFactory, PhysicsSearchViewModelFactory>();
        services.AddSingleton<IStartPhysicsViewModelFactory, StartPhysicsViewModelFactory>();
        return services;
    }
}
