namespace PsychologyApp.Presentation.App.DependencyInjection;

/// <summary>
/// Shared helpers for feature slice DI registration.
/// </summary>
public static class FeatureRegistrationBuilder
{
    public static IServiceCollection AddFeatureSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddSingleton<TService, TImplementation>();
        return services;
    }

    public static IServiceCollection AddFeatureSingleton<TImplementation>(this IServiceCollection services)
        where TImplementation : class
    {
        services.AddSingleton<TImplementation>();
        return services;
    }

    public static IServiceCollection AddFeatureViewModelFactory<TFactory, TImplementation>(this IServiceCollection services)
        where TFactory : class
        where TImplementation : class, TFactory
    {
        services.AddSingleton<TFactory, TImplementation>();
        return services;
    }
}
