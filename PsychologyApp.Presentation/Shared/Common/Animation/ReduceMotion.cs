namespace PsychologyApp.Presentation.Shared.Common;

public static class ReduceMotion
{
    private static Func<bool>? _isEnabled;

    public static void Configure(Func<bool> isReduceMotionEnabled) =>
        _isEnabled = isReduceMotionEnabled;

    // Snapshot: the Android check is a JNI settings query and IsEnabled runs for every animated view.
    // App.OnResume refreshes it after the user may have changed the setting.
    public static void Refresh()
    {
        bool isEnabled = ReduceMotionDetector.IsEnabled();
        Configure(() => isEnabled);
    }

    public static bool IsEnabled => _isEnabled?.Invoke() ?? false;
}
