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
public sealed record SituationAnalysis(
    CompanionEmotion Emotion,
    double Confidence,
    bool HasBodySymptoms,
    bool IsIntense,
    IReadOnlyList<CompanionTheme> Themes,
    CompanionEmotion Secondary = CompanionEmotion.Unknown,
    IReadOnlyList<CompanionPerson>? Persons = null)
{
    public static SituationAnalysis Empty { get; } = new(CompanionEmotion.Unknown, 0, false, false, []);
}

/// <summary>Turns free text into a structured guess about the person's state. Swap the implementation (e.g. an on-device embedding model) without touching callers.</summary>
public interface ISituationAnalyzer
{
    SituationAnalysis Analyze(string text);
}
