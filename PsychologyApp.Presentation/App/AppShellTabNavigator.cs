using PsychologyApp.Presentation.Shared.Lib.Navigation;

namespace PsychologyApp.Presentation.App;

public sealed class AppShellTabNavigator(AppShell appShell) : IShellTabNavigator
{
    public void OpenPracticeTabAndPendingTechnique()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            appShell.MaterializeTab(appShell.PracticeShellTab);
            appShell.CurrentItem = appShell.PracticeShellTab;
            appShell.OpenPendingTechniqueIfNeeded();
        });
    }

    public void OpenPracticeTabAndPendingJournal()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            appShell.MaterializeTab(appShell.PracticeShellTab);
            appShell.CurrentItem = appShell.PracticeShellTab;
            appShell.OpenPendingJournalIfNeeded();
        });
    }

    public void OpenQuotesTab()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            appShell.MaterializeTab(appShell.QuotesShellTab);
            appShell.CurrentItem = appShell.QuotesShellTab;
        });
    }
}
