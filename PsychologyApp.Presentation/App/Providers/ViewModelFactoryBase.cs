using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.App.Providers;

public abstract class ViewModelFactoryBase
{
    protected static INavigationService ResolveNavigation(
        Func<NavigationContext, INavigationService> navigationServiceFactory,
        ContentPage page) =>
        navigationServiceFactory(NavigationContext.From(page));
}
