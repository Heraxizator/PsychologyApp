namespace PsychologyApp.Application.Conversation.Companion;

/// <summary>Closed, hand-reviewed mapping from a recognised state to practices. The language model never chooses what the app launches.</summary>
public static class TechniqueSuggester
{
    public static IReadOnlyList<TechniqueId> Suggest(SituationAnalysis analysis) => analysis.Emotion switch
    {
        CompanionEmotion.Panic => [TechniqueId.Grounding, TechniqueId.Breathing],
        CompanionEmotion.Anxiety when analysis.HasBodySymptoms => [TechniqueId.Grounding, TechniqueId.Breathing],
        CompanionEmotion.Anxiety => [TechniqueId.Observer, TechniqueId.ThoughtRecord],
        CompanionEmotion.Overthinking => [TechniqueId.Observer, TechniqueId.ThoughtRecord],
        CompanionEmotion.Anger => [TechniqueId.Observer, TechniqueId.Breathing],
        CompanionEmotion.Resentment => [TechniqueId.Observer, TechniqueId.Spin],
        CompanionEmotion.Guilt => [TechniqueId.SelfCompassion, TechniqueId.ThoughtRecord],
        CompanionEmotion.Sadness => [TechniqueId.SelfCompassion, TechniqueId.SmallStep],
        CompanionEmotion.Exhaustion => [TechniqueId.Breathing, TechniqueId.SmallStep],
        CompanionEmotion.Loneliness => [TechniqueId.SelfCompassion, TechniqueId.Anchor],
        CompanionEmotion.Procrastination => [TechniqueId.SmallStep, TechniqueId.Observer],
        _ => [TechniqueId.Breathing, TechniqueId.Grounding]
    };
}
