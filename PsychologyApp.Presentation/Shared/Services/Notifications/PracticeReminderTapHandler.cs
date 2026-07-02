using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Lib.Navigation;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public static class PracticeReminderTapHandler
{
    private static IShellTabNavigator? _shellTabNavigator;

    public static void Configure(IShellTabNavigator shellTabNavigator) =>
        _shellTabNavigator = shellTabNavigator;

    public static void Handle(TechniqueId techniqueId)
    {
        UserPreferences.SetPendingTechnique(techniqueId);
        _shellTabNavigator?.OpenPracticeTabAndPendingTechnique();
    }
}
