namespace PsychologyApp.Presentation.Common;

public static class ReduceMotion
{
    private static Func<bool>? _isEnabled;

    public static void Configure(Func<bool> isReduceMotionEnabled) =>
        _isEnabled = isReduceMotionEnabled;

    public static bool IsEnabled => _isEnabled?.Invoke() ?? false;
}
