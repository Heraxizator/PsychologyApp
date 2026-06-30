using PsychologyApp.Application.Recommendations;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Pages.Onboarding;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.App.Providers;

public interface IOnboardingViewModelFactory
{
    OnboardingViewModel Create(ContentPage page, Func<TechniqueId?, Task> onCompleted);
}

public sealed class OnboardingViewModelFactory(
    IUserPreferencesStore userPreferencesStore,
    TechniqueCatalogGateway techniqueCatalog,
    ITechniqueRecommendationService techniqueRecommendationService,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IOnboardingViewModelFactory
{
    public OnboardingViewModel Create(ContentPage page, Func<TechniqueId?, Task> onCompleted) =>
        new(
            ResolveNavigation(navigationServiceFactory, page),
            userPreferencesStore,
            techniqueCatalog,
            techniqueRecommendationService,
            onCompleted);
}
