using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Lib.Navigation;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public static class MoodReminderTapHandler
{
    private static IShellTabNavigator? _shellTabNavigator;

    public static void Configure(IShellTabNavigator shellTabNavigator) =>
        _shellTabNavigator = shellTabNavigator;

    public static void Handle()
    {
        UserPreferences.SetPendingOpenJournal();
        _shellTabNavigator?.OpenPracticeTabAndPendingJournal();
    }
}
