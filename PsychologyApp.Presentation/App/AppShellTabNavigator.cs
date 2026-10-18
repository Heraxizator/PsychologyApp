using PsychologyApp.Presentation.App.Routes;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Lib.Navigation;

namespace PsychologyApp.Presentation.App;

public sealed class AppShellTabNavigator(Func<AppShell> appShellFactory, Func<INavigationService> navigationServiceFactory) : IShellTabNavigator
{
    private AppShell Shell => appShellFactory();

    public void OpenPracticeTabAndPendingTechnique()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AppShell appShell = Shell;
            appShell.ShowTab(appShell.PracticeShellTab);
            appShell.OpenPendingTechniqueIfNeeded();
        });
    }

    public void OpenPracticeTabAndPendingJournal()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AppShell appShell = Shell;
            appShell.ShowTab(appShell.PracticeShellTab);
            appShell.OpenPendingJournalIfNeeded();
        });
    }

    public void OpenPracticeTab()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AppShell appShell = Shell;
            appShell.ShowTab(appShell.PracticeShellTab);
        });
    }

    public void OpenQuotes()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // The quotes open over whatever is on screen; a second reminder tap must not stack another copy on the first.
            if (Microsoft.Maui.Controls.Shell.Current?.CurrentPage is PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed.QuotePage)
            {
                return;
            }

            navigationServiceFactory().GoToQuotesAsync().FireAndForget();
        });
    }

    public void OpenChat()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            navigationServiceFactory().GoToChatAsync().FireAndForget();
        });
    }
}
