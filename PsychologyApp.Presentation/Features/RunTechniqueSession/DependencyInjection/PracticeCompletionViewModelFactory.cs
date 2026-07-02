using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.PracticeCompletion;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

public interface IPracticeCompletionViewModelFactory
{
    PracticeCompletionViewModel Create(ContentPage page, int streakDays);
}

public sealed class PracticeCompletionViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IPracticeCompletionViewModelFactory
{
    public PracticeCompletionViewModel Create(ContentPage page, int streakDays) =>
        new(ResolveNavigation(navigationServiceFactory, page), streakDays);
}
