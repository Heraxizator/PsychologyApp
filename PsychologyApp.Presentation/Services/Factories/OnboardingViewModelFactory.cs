using PsychologyApp.Presentation.Modules.Onboarding;

namespace PsychologyApp.Presentation.Services.Factories;

public interface IOnboardingViewModelFactory
{
    OnboardingViewModel Create(INavigation navigation, Func<PsychologyApp.Presentation.Modules.Practice.Techniques.TechniqueId?, Task> onCompleted);
}

public sealed class OnboardingViewModelFactory(Func<INavigation, INavigationService> navigationServiceFactory) : IOnboardingViewModelFactory
{
    public OnboardingViewModel Create(INavigation navigation, Func<PsychologyApp.Presentation.Modules.Practice.Techniques.TechniqueId?, Task> onCompleted) =>
        new(navigation, navigationServiceFactory(navigation), onCompleted);
}
