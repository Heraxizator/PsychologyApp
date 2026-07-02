using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.Onboarding;
using PsychologyApp.Presentation.Pages.Onboarding;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Features.Onboarding.DependencyInjection;

public static class OnboardingFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddOnboardingFeature(this IServiceCollection services)
    {
        services.AddSingleton<OnboardingRecommendationResolver>();
        services.AddSingleton<IOnboardingViewModelFactory, OnboardingViewModelFactory>();
        return services;
    }
}

public interface IOnboardingViewModelFactory
{
    OnboardingViewModel Create(ContentPage page, Func<TechniqueId?, Task> onCompleted);
}

public sealed class OnboardingViewModelFactory(
    IUserPreferencesStore userPreferencesStore,
    OnboardingRecommendationResolver onboardingRecommendationResolver,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IOnboardingViewModelFactory
{
    public OnboardingViewModel Create(ContentPage page, Func<TechniqueId?, Task> onCompleted) =>
        new(
            ResolveNavigation(navigationServiceFactory, page),
            userPreferencesStore,
            onboardingRecommendationResolver,
            onCompleted);
}
