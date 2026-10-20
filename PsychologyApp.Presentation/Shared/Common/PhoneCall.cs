namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>Opens the phone app with a help number filled in. Used where a person in distress must reach a line in one tap.</summary>
public static class PhoneCall
{
    public static async Task DialAsync(string number)
    {
        try
        {
            if (PhoneDialer.Default.IsSupported)
            {
                PhoneDialer.Default.Open(number);
                return;
            }

            await Launcher.Default.OpenAsync($"tel:{number}");
        }
        catch (Exception bestEffortError)
        {
            // Best-effort dial.
            BestEffort.Report(bestEffortError);
        }
    }
}
