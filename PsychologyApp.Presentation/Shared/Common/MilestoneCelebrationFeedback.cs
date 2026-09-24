namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>Stronger tactile cue than the per-step entry tick — reserved for streak/lifetime milestone celebrations.</summary>
public static class MilestoneCelebrationFeedback
{
    public static void Play()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Milestone haptic skipped: {ex.GetType().Name}: {ex.Message}");
        }
    }
}
