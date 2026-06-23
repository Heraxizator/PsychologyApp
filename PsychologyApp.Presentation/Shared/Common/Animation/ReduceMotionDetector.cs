namespace PsychologyApp.Presentation.Shared.Common;

internal static class ReduceMotionDetector
{
    internal static bool IsEnabled()
    {
#if IOS || MACCATALYST
        return UIKit.UIAccessibility.IsReduceMotionEnabled;
#elif ANDROID
        try
        {
            Android.Content.Context? context = Android.App.Application.Context;
            if (context?.ContentResolver is null)
            {
                return false;
            }

            float scale = Android.Provider.Settings.Global.GetFloat(
                context.ContentResolver,
                Android.Provider.Settings.Global.TransitionAnimationScale,
                1f);
            return scale == 0f;
        }
        catch
        {
            return false;
        }
#else
        return false;
#endif
    }
}
