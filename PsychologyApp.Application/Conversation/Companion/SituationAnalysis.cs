namespace PsychologyApp.Application.Conversation.Companion;

public enum CompanionEmotion
{
    Unknown,
    Panic,
    Anxiety,
    Overthinking,
    Anger,
    Resentment,
    Guilt,
    Sadness,
    Exhaustion,
    Loneliness,
    Procrastination
}

public enum CompanionTheme
{
    Work,
    Relationships,
    Family,
    Health,
    Money,
    Study
}

/// <summary>What happened, as far as the words say: the situation behind the feeling.</summary>
public enum CompanionEvent
{
    None,
    Conflict,
    Humiliation,
    Breakup,
    JobLoss,
    HealthWorry,
    CaringForIll,
    Failure,
    Betrayal,
    Overload,
    Sleeplessness,
    Performance,
    Loss
}

public enum CompanionPerson
{
    Boss,
    Colleague,
    Partner,
    Parent,
    Child,
    Friend,
    Relative
}

/// <param name="Emotion">Best-matching state, or <see cref="CompanionEmotion.Unknown"/> when nothing in the text was recognised.</param>
/// <param name="Confidence">0..1, how clearly the top state beat the runner-up.</param>
/// <param name="HasBodySymptoms">The text mentions physical sensations (chest, stomach, shaking...).</param>
/// <param name="IsIntense">The text contains intensifiers such as "very", "unbearable".</param>
/// <param name="Secondary">A second state that scored almost as high (mixed feelings), or Unknown.</param>
/// <param name="Persons">Who the message is about (boss, partner, parent...), most mentioned first.</param>
/// <param name="Event">The situation behind the feeling ("a quarrel", "a breakup", "lost a job"), or None.</param>
/// <param name="HasLoss">Someone has died or was lost ("my grandmother died"): the reply is condolence, not a practice.</param>
public sealed record SituationAnalysis(
    CompanionEmotion Emotion,
    double Confidence,
    bool HasBodySymptoms,
    bool IsIntense,
    IReadOnlyList<CompanionTheme> Themes,
    CompanionEmotion Secondary = CompanionEmotion.Unknown,
    IReadOnlyList<CompanionPerson>? Persons = null,
    bool HasLoss = false,
    CompanionEvent Event = CompanionEvent.None)
{
    public static SituationAnalysis Empty { get; } = new(CompanionEmotion.Unknown, 0, false, false, []);
}

/// <summary>Turns free text into a structured guess about the person's state. Swap the implementation (e.g. an on-device embedding model) without touching callers.</summary>
public interface ISituationAnalyzer
{
    SituationAnalysis Analyze(string text);
}
