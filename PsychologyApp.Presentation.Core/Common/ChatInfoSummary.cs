using PsychologyApp.Application.Chat;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// What one chat holds, for its info page: the feeling it recognised, how the tension moved, the practice and the step the person
/// decided on. A part the chat never reached is null and its row is left out, so a short chat shows no empty rows.
/// </summary>
/// <param name="Emotion">The recognised state as a <c>CompanionEmotion</c> name, or null while unknown.</param>
/// <param name="Practice">The practice that helped in this chat, else the one done last.</param>
/// <param name="Step">The step the person decided on, in their own words.</param>
public sealed record ChatInfoSummary(string? Emotion, int? FirstTension, int? LastTension, TechniqueId? Practice, string? Step)
{
    public static ChatInfoSummary From(ChatSessionDTO session)
    {
        CompanionState state = CompanionState.Deserialize(session.StateJson);
        string? emotion = string.IsNullOrWhiteSpace(session.Emotion) || session.Emotion == "Unknown" ? null : session.Emotion;
        string? practice = state.HelpedPractices.LastOrDefault() ?? state.LastPractice ?? state.PendingPractice;
        TechniqueId? practiceId = Enum.TryParse(practice, out TechniqueId id) ? id : null;
        string? step = string.IsNullOrWhiteSpace(state.NewPlan) ? null : state.NewPlan.Trim();
        return new ChatInfoSummary(emotion, session.FirstIntensity, session.LastIntensity, practiceId, step);
    }

    public bool IsEmpty => Emotion is null && FirstTension is null && Practice is null && Step is null;

    /// <summary>"7 → 4" when the tension was rated twice and moved, else the one rating; null when it was never rated.</summary>
    public string? TensionText => (FirstTension, LastTension) switch
    {
        ({ } first, { } last) when first != last => $"{first} → {last}",
        ({ } first, _) => $"{first}",
        (null, { } last) => $"{last}",
        _ => null
    };
}
