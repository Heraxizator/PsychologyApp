using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Application.Chat;

/// <summary>
/// The trauma-informed part of the companion. It does not dig for memories: recovering "hidden" memories with leading questions creates
/// false ones, and reliving an event without a therapist can flood a person with no one to stop it. Instead, as in cognitive processing
/// therapy without a written account (as effective for PTSD as with it, Resick et al., 2008), it works on what the event changed: beliefs
/// about blame, safety, trust, control, oneself and closeness that got stuck. And it maps what sets the reactions off. Distress is checked
/// before the work starts; at 7 of 10 or a flashback in the words, the work waits and the person is helped back to the present first.
/// Crisis words are checked before everything, as everywhere. What is found goes into the summary for a specialist.
/// </summary>
public sealed partial class CompanionDialogue
{
    private static bool IsTraumaProtocol(CompanionState state) =>
        state.Protocol is CompanionTraumaContent.Impact or CompanionTraumaContent.Triggers;

    /// <summary>A hard event was mentioned: thanks, no probing for details, and the choices of what to do with it.</summary>
    private CompanionReply AcknowledgeTrauma(CompanionState state, CompanionEmotion emotion, CompanionTheme? theme) =>
        Reply(
            [CompanionTraumaContent.Acknowledge(english)],
            CompanionTraumaContent.Choices(english),
            (state with { TraumaAcknowledged = true, TurnsSinceOffer = 0 }).WithAsked("trigger").WithAsked("what_happened"),
            emotion,
            theme);

    /// <summary>A chat opened from the trauma articles or course: straight to the choices, nothing to tell first.</summary>
    public CompanionReply OpenTopic(CompanionState state, string topic) => topic == CompanionTraumaContent.Topic
        ? WithPending(new CompanionReply(
            [CompanionTraumaContent.TopicGreeting(english)],
            CompanionTraumaContent.Choices(english),
            null,
            state with { TraumaAcknowledged = true },
            CompanionEmotion.Unknown,
            null))
        : Open(state, previous: null);

    private CompanionReply AskTrauma(CompanionState state, CompanionEmotion emotion, CompanionTheme? theme)
    {
        IReadOnlyList<ChatQuickReply> stop = [CompanionProtocolContent.StopChip(english)];
        if (state.Protocol == CompanionTraumaContent.Triggers)
        {
            return Reply([CompanionTraumaContent.TriggerQuestion(state.ProtocolStep, english)], stop, state, emotion, theme);
        }

        string themeKey = state.ProtocolNotes.ElementAtOrDefault(0) ?? string.Empty;
        (string question, IReadOnlyList<ChatQuickReply> chips) = state.ProtocolStep switch
        {
            CompanionTraumaContent.ImpactDistress => (CompanionTraumaContent.DistressQuestion(english), RatingReplies()),
            CompanionTraumaContent.ImpactTheme => (CompanionTraumaContent.ThemeQuestion(english), CompanionTraumaContent.ThemeChips(english)),
            CompanionTraumaContent.ImpactPhrase => (CompanionTraumaContent.PhraseQuestion(themeKey, english), [.. CompanionTraumaContent.PhraseChips(themeKey, english), .. stop]),
            CompanionTraumaContent.ImpactBeliefBefore => (CompanionTraumaContent.BeliefQuestion(english), RatingReplies()),
            4 => (CompanionTraumaContent.ThemeQuestion(themeKey, 0, english), stop),
            5 => (CompanionTraumaContent.ThemeQuestion(themeKey, 1, english), stop),
            CompanionTraumaContent.ImpactBalanced => (CompanionTraumaContent.BalancedQuestion(english), stop),
            _ => (CompanionTraumaContent.BeliefAgain(english), RatingReplies())
        };
        return Reply([question], chips, state, emotion, theme);
    }

    private static bool IsTraumaRatingStep(CompanionState state) =>
        state.Protocol == CompanionTraumaContent.Impact
        && state.ProtocolStep is CompanionTraumaContent.ImpactDistress or CompanionTraumaContent.ImpactBeliefBefore or CompanionTraumaContent.ImpactBeliefAfter;

    private CompanionReply ContinueTrauma(CompanionState state, string? words, int? rating, CompanionEmotion emotion, CompanionTheme? theme)
    {
        // A memory taking over in the middle of the work stops it: first the present, then, maybe, the rest another time.
        if (words is { Length: > 0 } && analyzer.Analyze(words).IsFlashback)
        {
            return Stabilise(state, emotion, theme);
        }

        if (IsTraumaRatingStep(state))
        {
            if (rating is not { } value && !(words is not null && TryParseRating(words, out value)))
            {
                return AskTrauma(state, emotion, theme);
            }

            return state.ProtocolStep switch
            {
                CompanionTraumaContent.ImpactDistress when value >= CompanionTraumaContent.TooHard => Stabilise(state, emotion, theme),
                CompanionTraumaContent.ImpactDistress => AskTrauma(state with { ProtocolStep = CompanionTraumaContent.ImpactTheme }, emotion, theme),
                CompanionTraumaContent.ImpactBeliefBefore => AskTrauma(state with { ProtocolBelief = value, ProtocolStep = state.ProtocolStep + 1 }, emotion, theme),
                _ => FinishImpact(state, value, emotion, theme)
            };
        }

        if (string.IsNullOrWhiteSpace(words))
        {
            return AskTrauma(state, emotion, theme);
        }

        string answer = words;
        if (state.Protocol == CompanionTraumaContent.Impact && state.ProtocolStep == CompanionTraumaContent.ImpactTheme)
        {
            if ((CompanionTraumaContent.Themes.Contains(words) ? words : CompanionTraumaContent.ParseTheme(words)) is not { } picked)
            {
                return AskTrauma(state, emotion, theme);
            }

            answer = picked;
        }

        CompanionState answered = state with
        {
            Turns = state.Turns + 1,
            ProtocolNotes = [.. state.ProtocolNotes, answer.Length > MaxProtocolNote ? answer[..MaxProtocolNote] : answer],
            ProtocolStep = state.ProtocolStep + 1
        };

        return answered.Protocol == CompanionTraumaContent.Triggers && answered.ProtocolStep >= CompanionTraumaContent.TriggerSteps
            ? FinishTriggers(answered, emotion, theme)
            : AskTrauma(answered, emotion, theme);
    }

    private CompanionReply Stabilise(CompanionState state, CompanionEmotion emotion, CompanionTheme? theme) =>
        Reply([CompanionTraumaContent.TooHardNow(english)], CompanionTraumaContent.StabiliseChips(english), EndProtocol(state), emotion, theme);

    private CompanionReply FinishImpact(CompanionState state, int after, CompanionEmotion emotion, CompanionTheme? theme)
    {
        // Notes: the theme, the stuck phrase, the answers to the two questions, the fairer wording.
        IReadOnlyList<string> notes = state.ProtocolNotes;
        StuckPointNote note = new(
            notes.ElementAtOrDefault(0) ?? string.Empty,
            notes.ElementAtOrDefault(1) ?? string.Empty,
            notes.ElementAtOrDefault(4) ?? string.Empty,
            state.ProtocolBelief ?? after,
            after,
            _time.GetUtcNow().UtcDateTime);
        CompanionState done = EndProtocol(state) with { TraumaNotes = TraumaNotes.Deserialize(state.TraumaNotes).With(note).Serialize() };
        return Reply([CompanionTraumaContent.ImpactSummary(note, english)], AfterTraumaWorkChips(), done, emotion, theme);
    }

    private CompanionReply FinishTriggers(CompanionState state, CompanionEmotion emotion, CompanionTheme? theme)
    {
        IReadOnlyList<string> notes = state.ProtocolNotes;
        TriggerNote note = new(
            notes.ElementAtOrDefault(0) ?? string.Empty,
            notes.ElementAtOrDefault(1) ?? string.Empty,
            notes.ElementAtOrDefault(2) ?? string.Empty,
            _time.GetUtcNow().UtcDateTime);
        CompanionState done = EndProtocol(state) with { TraumaNotes = TraumaNotes.Deserialize(state.TraumaNotes).With(note).Serialize() };
        return Reply([CompanionTraumaContent.TriggerSummary(note, english)], AfterTraumaWorkChips(), done, emotion, theme);
    }

    /// <summary>After the work: another round, the article on how trauma is treated, or more talk.</summary>
    private IReadOnlyList<ChatQuickReply> AfterTraumaWorkChips() =>
    [
        .. CompanionTraumaContent.Choices(english).Take(2),
        new ChatQuickReply(ChatQuickReplyKinds.Act, english ? "How trauma is treated" : "Как лечат травму", "article:" + Articles.ArticleIds.TraumaTreatment),
        new ChatQuickReply(ChatQuickReplyKinds.More, CompanionContent.MoreLabel(english))
    ];
}
