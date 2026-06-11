using PsychologyApp.Presentation.ViewModels.Onboarding;
using PsychologyApp.Presentation.Services;

namespace PsychologyApp.Presentation.Services.Factories;

public interface IOnboardingViewModelFactory
{
    OnboardingViewModel Create(INavigation navigation, Func<TechniqueId?, Task> onCompleted);
}

public sealed class OnboardingViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : IOnboardingViewModelFactory
{
    public OnboardingViewModel Create(INavigation navigation, Func<TechniqueId?, Task> onCompleted) =>
        new(navigation, navigationServiceFactory(NavigationContext.From(navigation)), onCompleted);
}
