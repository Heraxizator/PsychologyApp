using PsychologyApp.Presentation.Pages.PracticeCompletion;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.App.Providers;

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
