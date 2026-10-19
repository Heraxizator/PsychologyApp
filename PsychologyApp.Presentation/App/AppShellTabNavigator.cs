using PsychologyApp.Presentation.App.Routes;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Lib.Navigation;

namespace PsychologyApp.Presentation.App;

/// <summary>
/// Navigation asked for from outside the app: a reminder tap or the home-screen widget. It waits for the startup (onboarding,
/// the safety gate) and never takes the person away from the crisis hub or the risk check: the gate is pushed on the current
/// tab, and a tab switch would hide it behind another tab.
/// </summary>
public sealed class AppShellTabNavigator(Func<AppShell> appShellFactory, Func<INavigationService> navigationServiceFactory) : IShellTabNavigator
{
    private AppShell Shell => appShellFactory();

    public void OpenPracticeTabAndPendingTechnique() => Run(appShell =>
    {
        appShell.ShowTab(appShell.PracticeShellTab);
        appShell.OpenPendingTechniqueIfNeeded();
    });

    public void OpenPracticeTabAndPendingJournal() => Run(appShell =>
    {
        appShell.ShowTab(appShell.PracticeShellTab);
        appShell.OpenPendingJournalIfNeeded();
    });

    public void OpenPracticeTab() => Run(appShell => appShell.ShowTab(appShell.PracticeShellTab));

    public void OpenJournal() => Run(appShell => appShell.ShowTab(appShell.JournalShellTab));

    public void OpenQuotes() => Run(_ =>
    {
        // The quotes open over whatever is on screen; a second reminder tap must not stack another copy on the first.
        if (Microsoft.Maui.Controls.Shell.Current?.CurrentPage is PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed.QuotePage)
        {
            return;
        }

        navigationServiceFactory().GoToQuotesAsync().FireAndForget();
    });

    public void OpenChat() => Run(_ => navigationServiceFactory().GoToChatAsync().FireAndForget());

    private void Run(Action<AppShell> navigate) => RunAsync(navigate).FireAndForget();

    private async Task RunAsync(Action<AppShell> navigate)
    {
        AppShell appShell = Shell;
        await appShell.StartupCompleted;
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (!AppShell.IsSafetyPageShown)
            {
                navigate(appShell);
            }
        });
    }
}
