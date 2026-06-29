namespace PsychologyApp.Presentation.Shared.Navigation;

internal static class ShellNavigationResolver
{
    internal static ShellContent? ResolveShellContent(BaseShellItem? item) => item switch
    {
        ShellContent shellContent => shellContent,
        ShellSection section => ResolveShellContent(section.CurrentItem),
        ShellItem shellItem => ResolveShellContent(shellItem.CurrentItem),
        _ => null
    };

    internal static INavigation? TryGetActiveShellTabNavigation() =>
        TryGetActiveTabStackNavigation();

    internal static INavigation? TryGetActiveTabStackNavigation()
    {
        if (Shell.Current?.CurrentItem is not TabBar tabBar)
        {
            return null;
        }

        if (ResolveShellContent(tabBar.CurrentItem)?.Content is not Page rootPage)
        {
            return null;
        }

        INavigation navigation = rootPage.Navigation;
        return ResolveTopStackNavigation(navigation);
    }

    internal static INavigation? ResolveTopStackNavigation(INavigation? navigation)
    {
        if (navigation is null)
        {
            return null;
        }

        if (navigation.ModalStack.Count > 0)
        {
            Page topModal = navigation.ModalStack[^1];
            return topModal.Navigation ?? navigation;
        }

        if (navigation.NavigationStack.Count > 0)
        {
            Page topPage = navigation.NavigationStack[^1];
            return topPage.Navigation ?? navigation;
        }

        return navigation;
    }
}
