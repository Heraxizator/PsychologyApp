namespace PsychologyApp.Domain.Practice;

/// <summary>What a practice field is about, so the screen can help fill it without typing.</summary>
public enum EntryFieldRole
{
    None,

    /// <summary>The problem or situation the practice works on: the same one is often carried from one practice to the next.</summary>
    Problem,

    /// <summary>A feeling to name: picked from a palette of emotions instead of typed.</summary>
    Emotion,

    /// <summary>A more balanced thought in place of an automatic one.</summary>
    Reframe,

    /// <summary>Kind words for a friend in the same situation.</summary>
    Kindness,

    /// <summary>The same kindness, turned to oneself.</summary>
    SelfKindness,

    /// <summary>A 0..10 rating of tension taken at the start: it is the practice's "before", so the screen does not ask it a second time.</summary>
    IntensityBefore,

    /// <summary>The same rating at the end: it is the practice's "after", so the result page shows it instead of asking again.</summary>
    IntensityAfter
}
