namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>A light tap under the finger. Silent where the device cannot vibrate.</summary>
public static class UiHaptics
{
    public static void Tick()
    {
        try
        {
            if (HapticFeedback.Default.IsSupported)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            }
        }
        catch (Exception ex) when (ex is FeatureNotSupportedException or PermissionException)
        {
            // No vibration on this device: the picture and the words are enough.
        }
    }
}
