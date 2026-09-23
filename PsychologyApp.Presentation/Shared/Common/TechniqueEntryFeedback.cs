namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>Tiny tactile reward for adding an entry during a technique session (paper, polarity, copied-thought lists).</summary>
public static class TechniqueEntryFeedback
{
    public static void PlayAddFeedback()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Entry-add haptic skipped: {ex.GetType().Name}: {ex.Message}");
        }
    }
}
