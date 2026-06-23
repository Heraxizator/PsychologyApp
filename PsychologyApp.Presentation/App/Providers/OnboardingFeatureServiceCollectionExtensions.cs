using PsychologyApp.Presentation.App.Providers;

namespace PsychologyApp.Presentation.App.Providers;

public static class OnboardingFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddOnboardingFeature(this IServiceCollection services)
    {
        services.AddSingleton<IOnboardingViewModelFactory, OnboardingViewModelFactory>();
        return services;
    }
}
