namespace PsychologyApp.Presentation.Features.ManageJournal;

public enum JournalLowMoodSupportKind
{
    /// <summary>The mood is not low, or the day is not today: nothing is offered.</summary>
    None,

    /// <summary>A short practice and the anchor thoughts, with urgent help below them.</summary>
    PracticesAndHelp,

    /// <summary>The note reads like a crisis: no suggestions, only the way to help.</summary>
    HelpOnly
}

/// <summary>
/// What the journal offers after a low check-in. In a crisis suggestions are not shown, the person gets the way to help (the rule of
/// the chat applies here too); otherwise a low mood today gets something small to do and help one tap away.
/// </summary>
public static class JournalLowMoodSupport
{
    public static JournalLowMoodSupportKind For(bool isToday, int moodLevel, bool noteLooksLikeCrisis)
    {
        if (noteLooksLikeCrisis)
        {
            return JournalLowMoodSupportKind.HelpOnly;
        }

        return isToday && moodLevel is 1 or 2 ? JournalLowMoodSupportKind.PracticesAndHelp : JournalLowMoodSupportKind.None;
    }
}
