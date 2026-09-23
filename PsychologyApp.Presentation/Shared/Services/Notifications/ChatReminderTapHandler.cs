using PsychologyApp.Presentation.Shared.Lib.Navigation;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public static class ChatReminderTapHandler
{
    private static IShellTabNavigator? _shellTabNavigator;

    public static void Configure(IShellTabNavigator shellTabNavigator) =>
        _shellTabNavigator = shellTabNavigator;

    public static void Handle() => _shellTabNavigator?.OpenChat();
}
