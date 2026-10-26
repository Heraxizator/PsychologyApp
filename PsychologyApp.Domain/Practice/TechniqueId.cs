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
    Nightmare,

    /// <summary>Popping balloons with taps: a short, bounded game to let acute tension out.</summary>
    Balloons,

    /// <summary>The cyclic sigh: a double breath in through the nose and a long breath out, paced for a few minutes.</summary>
    CyclicSigh,

    /// <summary>A short progressive muscle relaxation: tense and let go of five muscle groups in turn.</summary>
    MuscleRelax,

    /// <summary>"Keep the mind busy": short neutral tasks that take attention away from spinning thoughts.</summary>
    BusyMind
}

public enum TechniqueUiKind
{
    None,
    Entry,
    Polarity,
    Paper,
    Copied
}
