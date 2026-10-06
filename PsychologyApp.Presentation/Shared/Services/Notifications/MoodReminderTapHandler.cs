using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Lib.Navigation;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public static class MoodReminderTapHandler
{
    private static IShellTabNavigator? _shellTabNavigator;
    // A reminder can be tapped before the app window exists (cold start); the pending flag must still be stored, so fall back to the default store.
    private static IUserPreferencesStore? _preferences;

    public static void Configure(IShellTabNavigator shellTabNavigator, IUserPreferencesStore preferences)
    {
        _shellTabNavigator = shellTabNavigator;
        _preferences = preferences;
    }

    public static void Handle()
    {
        (_preferences ?? new MauiUserPreferencesStore()).SetPendingOpenJournal();
        _shellTabNavigator?.OpenPracticeTabAndPendingJournal();
    }
}
