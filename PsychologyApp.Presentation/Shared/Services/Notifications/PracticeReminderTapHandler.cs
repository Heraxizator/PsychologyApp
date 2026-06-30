using PsychologyApp.Presentation.App;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public static class PracticeReminderTapHandler
{
    public static void Handle(TechniqueId techniqueId)
    {
        UserPreferences.SetPendingTechnique(techniqueId);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (Shell.Current is not AppShell shell)
            {
                return;
            }

            shell.MaterializeTab(shell.PracticeShellTab);
            shell.CurrentItem = shell.PracticeShellTab;
            shell.OpenPendingTechniqueIfNeeded();
        });
    }
}
