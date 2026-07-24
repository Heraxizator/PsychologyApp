using PsychologyApp.Presentation.Shared.Lib.Navigation;

namespace PsychologyApp.Presentation.App;

public sealed class AppShellTabNavigator(Func<AppShell> appShellFactory) : IShellTabNavigator
{
    private AppShell Shell => appShellFactory();

    public void OpenPracticeTabAndPendingTechnique()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AppShell appShell = Shell;
            appShell.MaterializeTab(appShell.PracticeShellTab);
            appShell.CurrentItem = appShell.PracticeShellTab;
            appShell.OpenPendingTechniqueIfNeeded();
        });
    }

    public void OpenPracticeTabAndPendingJournal()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AppShell appShell = Shell;
            appShell.MaterializeTab(appShell.PracticeShellTab);
            appShell.CurrentItem = appShell.PracticeShellTab;
            appShell.OpenPendingJournalIfNeeded();
        });
    }

    public void OpenPracticeTab()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AppShell appShell = Shell;
            appShell.MaterializeTab(appShell.PracticeShellTab);
            appShell.CurrentItem = appShell.PracticeShellTab;
        });
    }

    public void OpenQuotesTab()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AppShell appShell = Shell;
            appShell.MaterializeTab(appShell.QuotesShellTab);
            appShell.CurrentItem = appShell.QuotesShellTab;
        });
    }
}
