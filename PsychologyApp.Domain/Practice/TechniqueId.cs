namespace PsychologyApp.Domain.Practice;

public enum TechniqueId
{
    Spin,
    Comparison,
    Polarity,
    Paper,
    Future,
    Hack,
    Experience,
    Copied,
    Extend,
    Resize,
    Check,
    Observer,
    Anchor,
    Grounding,
    Breathing,
    SmallStep,
    ThoughtRecord,
    SelfCompassion,

    /// <summary>Where is it in the body: noticing the body sensation of a feeling and breathing with it (body scan).</summary>
    BodyScan,

    /// <summary>Help when a traumatic memory takes over: a guided return to the present (dialogue).</summary>
    Flashback,

    /// <summary>Rewriting a recurring nightmare and rehearsing the new version (imagery rehearsal therapy).</summary>
    Nightmare
}

public enum TechniqueUiKind
{
    None,
    Entry,
    Polarity,
    Paper,
    Copied
}
