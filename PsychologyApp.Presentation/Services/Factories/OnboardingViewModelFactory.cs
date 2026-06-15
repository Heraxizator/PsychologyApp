using PsychologyApp.Presentation.ViewModels.Onboarding;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Preferences;

namespace PsychologyApp.Presentation.Services.Factories;

public interface IOnboardingViewModelFactory
{
    OnboardingViewModel Create(INavigation navigation, Func<TechniqueId?, Task> onCompleted);
}

public sealed class OnboardingViewModelFactory(
    IUserPreferencesStore userPreferencesStore,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IOnboardingViewModelFactory
{
    public OnboardingViewModel Create(INavigation navigation, Func<TechniqueId?, Task> onCompleted) =>
        new(
            ResolveNavigation(navigationServiceFactory, navigation),
            userPreferencesStore,
            onCompleted);
}
