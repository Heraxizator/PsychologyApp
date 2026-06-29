namespace PsychologyApp.Presentation.Shared.Navigation;

public sealed record NavigationContext(INavigation? Navigation, ContentPage? OwnerPage = null, INavigationService? NavigationService = null)
{
    public static NavigationContext From(ContentPage page) => new(page.Navigation, page);

    public static NavigationContext From(INavigation navigation) => new(navigation);

    public INavigationService Resolve(Func<NavigationContext, INavigationService> factory) =>
        NavigationService ?? factory(this);
}
