namespace PsychologyApp.Presentation.Services.Factories;

public abstract class ViewModelFactoryBase
{
    protected static INavigationService ResolveNavigation(
        Func<NavigationContext, INavigationService> navigationServiceFactory,
        INavigation navigation) =>
        navigationServiceFactory(NavigationContext.From(navigation));
}
