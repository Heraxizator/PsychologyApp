namespace PsychologyApp.Presentation.Shared.Services.Dialogs;

public sealed class MauiPageHost : IPageHost
{
    public Page? GetActivePage() => ResolveActivePage();

    internal static Page? ResolveActivePage()
    {
        if (Shell.Current?.CurrentPage is Page shellPage)
        {
            Page? resolved = ResolveFromNavigation(shellPage);
            if (resolved is not null)
            {
                return resolved;
            }
        }

        Window? window = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault();
        if (window?.Page is Page windowPage)
        {
            return ResolveFromNavigation(windowPage) ?? windowPage;
        }

        return null;
    }

    private static Page? ResolveFromNavigation(Page page)
    {
        if (page is Shell shell && shell.CurrentPage is Page currentShellPage)
        {
            return ResolveFromNavigation(currentShellPage) ?? currentShellPage;
        }

        INavigation? navigation = page.Navigation;
        if (navigation?.ModalStack is { Count: > 0 } modals)
        {
            return modals[^1];
        }

        if (navigation?.NavigationStack is { Count: > 0 } stack)
        {
            return stack[^1];
        }

        return page;
    }
}
