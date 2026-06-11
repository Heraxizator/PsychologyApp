namespace PsychologyApp.Presentation.Services;

public sealed record NavigationContext(INavigation Navigation, INavigationService? NavigationService = null)
{
    public static NavigationContext From(INavigation navigation) => new(navigation);

    public INavigationService Resolve(Func<NavigationContext, INavigationService> factory) =>
        NavigationService ?? factory(this);
}
