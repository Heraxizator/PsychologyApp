using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Application.Chat;

public abstract record CompanionInput
{
    public sealed record FreeText(string Value) : CompanionInput;

    public sealed record QuickReply(ChatQuickReply Reply) : CompanionInput;
}

/// <param name="Messages">Companion messages to append, in order. Quick replies belong to the last one.</param>
/// <param name="Action">Something the UI must do after showing the messages (open a practice, open the crisis hub).</param>
/// <param name="State">The state to persist for the next turn.</param>
/// <param name="Emotion">Recognised state after this turn.</param>
public sealed record CompanionReply(
    IReadOnlyList<string> Messages,
    IReadOnlyList<ChatQuickReply> QuickReplies,
    DialogueAction? Action,
    CompanionState State,
    CompanionEmotion Emotion,
    CompanionTheme? Theme)
{
    public static CompanionReply Empty(CompanionState state) => new([], [], null, state, CompanionEmotion.Unknown, null);
}

/// <summary>
/// The messenger companion. A deterministic dialogue manager: it listens (quotes the person's own words), asks one
/// meaningful question at a time, measures tension on a 0..10 scale and, when it makes sense, offers a practice.
/// Every decision is derived from <see cref="CompanionState"/> plus the new input, so a chat can be resumed after a restart
/// and behaviour is fully testable. No language model is involved.
/// </summary>
public sealed class CompanionDialogue(
    ISituationAnalyzer analyzer,
    ICrisisDetector crisisDetector,
    bool english,
    Random? random = null,
    TimeProvider? time = null)
{
    private readonly Random _random = random ?? Random.Shared;
    private readonly TimeProvider _time = time ?? TimeProvider.System;

    private const int HighTension = 8;
    private const int LowTension = 3;

    /// <summary>First messages of a new chat. With a previous chat on record the companion remembers it and asks how things are now.</summary>
    public CompanionReply Open(CompanionState state, ChatSessionDTO? previous)
    {
        if (previous is { MessageCount: >= 2 } && !string.IsNullOrWhiteSpace(previous.Emotion) && previous.Emotion != nameof(CompanionEmotion.Unknown))
        {
            int days = (int)(_time.GetUtcNow().UtcDateTime - previous.UpdatedAt).TotalDays;
            return new CompanionReply(
                [CompanionDialogueContent.ReturningGreeting(previous.Title, days, previous.LastIntensity, english)],
                CompanionDialogueContent.CheckInReplies(english),
                null,
                state,
                CompanionEmotion.Unknown,
                null);
        }

        return new CompanionReply(
            [CompanionContent.Greeting(english, _random), CompanionContent.PrivacyNote(english)],
            [],
            null,
            state,
            CompanionEmotion.Unknown,
            null);
    }

    public CompanionReply Respond(CompanionState state, CompanionInput input) => input switch
    {
        CompanionInput.FreeText text => RespondToText(state, text.Value),
        CompanionInput.QuickReply quick => RespondToQuickReply(state, quick.Reply),
        _ => CompanionReply.Empty(state)
    };

    /// <summary>Message posted when the person comes back from a practice started in this chat.</summary>
    public CompanionReply FollowUpAfterPractice(CompanionState state) => new(
        [CompanionDialogueContent.FollowUpAfterPractice(english)],
        RatingReplies(),
        null,
        state with { PendingPractice = null, PendingPracticeStartedUtc = null, AwaitingPostPracticeRating = true, ScaleAsked = true },
        ParseEmotion(state.Emotion),
        null);

    private CompanionReply RespondToText(CompanionState previous, string raw)
    {
        string text = raw.Trim();
        if (text.Length == 0)
        {
            return CompanionReply.Empty(previous);
        }

        if (crisisDetector.IsCrisis(text))
        {
            return new CompanionReply(
                [CompanionContent.Crisis(english)],
                [],
                new DialogueAction(DialogueActionKind.OpenCrisisHub),
                previous,
                ParseEmotion(previous.Emotion),
                null);
        }

        CompanionState state = previous.WithText(text) with
        {
            TurnsSinceOffer = Math.Min(previous.TurnsSinceOffer + 1, 99),
            AwaitingPostPracticeRating = false
        };

        SituationAnalysis analysis = analyzer.Analyze(text);
        if (analysis.Emotion == CompanionEmotion.Unknown)
        {
            analysis = analyzer.Analyze(string.Join(' ', state.RecentTexts)) with { HasBodySymptoms = analysis.HasBodySymptoms, IsIntense = analysis.IsIntense };
        }

        CompanionEmotion emotion = analysis.Emotion != CompanionEmotion.Unknown ? analysis.Emotion : ParseEmotion(state.Emotion);
        CompanionTheme? theme = analysis.Themes.Count > 0 ? analysis.Themes[0] : ParseTheme(state.Theme);
        state = state with
        {
            Emotion = emotion.ToString(),
            Theme = theme?.ToString(),
            UnknownStreak = emotion == CompanionEmotion.Unknown ? state.UnknownStreak + 1 : 0
        };
        analysis = analysis with { Emotion = emotion };

        string? quote = QuoteExtractor.Pick(text, analyzer, emotion);
        List<string> messages = [Listen(state, analysis, emotion, quote)];

        bool urgent = emotion == CompanionEmotion.Panic
            || (emotion == CompanionEmotion.Anxiety && analysis.IsIntense && analysis.HasBodySymptoms);
        if (urgent)
        {
            messages.Add(CompanionDialogueContent.UrgentBridge(english));
            return Offer(state, messages, analysis, emotion, theme, calming: false);
        }

        if (emotion == CompanionEmotion.Unknown && state.UnknownStreak >= 2)
        {
            messages.Add(CompanionDialogueContent.EmotionPrompt(english));
            return Reply(messages, EmotionReplies(), state with { UnknownStreak = 0 }, emotion, theme);
        }

        if (!state.ScaleAsked && state.Turns >= 2 && emotion != CompanionEmotion.Unknown)
        {
            messages.Add(CompanionDialogueContent.ScaleQuestion(english));
            return Reply(messages, RatingReplies(), state with { ScaleAsked = true }, emotion, theme);
        }

        if (state.Turns >= 3 && state.TurnsSinceOffer >= 3 && emotion != CompanionEmotion.Unknown)
        {
            return Offer(state, messages, analysis, emotion, theme, calming: false);
        }

        return AskNextQuestion(state, messages, emotion, theme);
    }

    private CompanionReply RespondToQuickReply(CompanionState state, ChatQuickReply reply)
    {
        CompanionEmotion emotion = ParseEmotion(state.Emotion);
        CompanionTheme? theme = ParseTheme(state.Theme);

        switch (reply.Kind)
        {
            case ChatQuickReplyKinds.Practice when Enum.TryParse(reply.Payload, out TechniqueId id):
                return new CompanionReply(
                    [CompanionDialogueContent.PracticeStarted(english)],
                    [],
                    new DialogueAction(DialogueActionKind.StartTechnique, id),
                    state with
                    {
                        PendingPractice = id.ToString(),
                        PendingPracticeStartedUtc = _time.GetUtcNow().UtcDateTime,
                        TurnsSinceOffer = 0
                    },
                    emotion,
                    theme);

            case ChatQuickReplyKinds.More:
                return Reply([CompanionDialogueContent.Question("more", english)], [], state with { TurnsSinceOffer = 0 }, emotion, theme);

            case ChatQuickReplyKinds.Rating when int.TryParse(reply.Payload, out int rating):
                return RespondToRating(state, Math.Clamp(rating, 0, 10), emotion, theme);

            case ChatQuickReplyKinds.Emotion when Enum.TryParse(reply.Payload, out CompanionEmotion picked):
                CompanionState pickedState = state with { Emotion = picked.ToString(), UnknownStreak = 0 };
                List<string> pickedMessages = [CompanionContent.Understood(picked, english)];
                return pickedState.Turns >= 2
                    ? Offer(pickedState, pickedMessages, new SituationAnalysis(picked, 1, false, false, []), picked, theme, calming: false)
                    : AskNextQuestion(pickedState, pickedMessages, picked, theme);

            case ChatQuickReplyKinds.CheckIn:
                return Reply([CompanionDialogueContent.CheckInResponse(reply.Payload ?? "other", english)], [], state, emotion, theme);

            default:
                return CompanionReply.Empty(state);
        }
    }

    private CompanionReply RespondToRating(CompanionState previous, int rating, CompanionEmotion emotion, CompanionTheme? theme)
    {
        int? before = previous.LastIntensity ?? previous.FirstIntensity;
        CompanionState state = previous with
        {
            FirstIntensity = previous.FirstIntensity ?? rating,
            LastIntensity = rating,
            ScaleAsked = true,
            AwaitingPostPracticeRating = false
        };

        if (previous.AwaitingPostPracticeRating)
        {
            int reference = before ?? rating;
            if (rating < reference)
            {
                return Reply([CompanionDialogueContent.PostPracticeBetter(reference, rating, english)], [], state, emotion, theme);
            }

            if (rating == reference)
            {
                string alternative = state.OfferedAlternative ?? state.OfferedPrimary ?? TechniqueId.Breathing.ToString();
                return Reply(
                    [CompanionDialogueContent.PostPracticeSame(rating, english)],
                    [PracticeReply(alternative, CompanionDialogueContent.AnotherPracticeLabel(english)), new(ChatQuickReplyKinds.More, CompanionDialogueContent.TalkLabel(english))],
                    state,
                    emotion,
                    theme);
            }

            return Reply([CompanionDialogueContent.PostPracticeWorse(reference, rating, english)], [], state, emotion, theme);
        }

        SituationAnalysis analysis = new(emotion, 1, false, rating >= HighTension, []);
        if (rating >= HighTension)
        {
            return Offer(state, [CompanionDialogueContent.ScaleHigh(english)], analysis, emotion, theme, calming: true);
        }

        List<string> messages = [rating <= LowTension ? CompanionDialogueContent.ScaleLow(english) : CompanionDialogueContent.ScaleMedium(english)];
        return state.Turns >= 3 && state.TurnsSinceOffer >= 3 && emotion != CompanionEmotion.Unknown
            ? Offer(state, messages, analysis, emotion, theme, calming: false)
            : AskNextQuestion(state, messages, emotion, theme);
    }

    private string Listen(CompanionState state, SituationAnalysis analysis, CompanionEmotion emotion, string? quote)
    {
        if (state.Turns == 1)
        {
            string validation = CompanionContent.Reflection(analysis with { Emotion = emotion }, english, _random);
            return quote is null ? validation : $"{CompanionDialogueContent.QuoteLine(quote, english, _random)} {validation}";
        }

        string ack = CompanionDialogueContent.Acknowledgement(english, _random);
        if (quote is null || state.Turns % 2 != 0)
        {
            return ack;
        }

        return english ? $"“{quote}”. {ack}" : $"«{quote}». {ack}";
    }

    private CompanionReply AskNextQuestion(CompanionState state, List<string> messages, CompanionEmotion emotion, CompanionTheme? theme)
    {
        string? id = CompanionDialogueContent.NextQuestionId(emotion, state.AskedQuestions);
        if (id is null)
        {
            return emotion == CompanionEmotion.Unknown
                ? Reply(messages, [], state, emotion, theme)
                : Offer(state, messages, new SituationAnalysis(emotion, 1, false, false, []), emotion, theme, calming: false);
        }

        messages.Add(CompanionDialogueContent.Question(id, english));
        return Reply(messages, [], state.WithAsked(id), emotion, theme);
    }

    private CompanionReply Offer(
        CompanionState state,
        List<string> messages,
        SituationAnalysis analysis,
        CompanionEmotion emotion,
        CompanionTheme? theme,
        bool calming)
    {
        IReadOnlyList<TechniqueId> suggestions = calming && emotion is not (CompanionEmotion.Panic or CompanionEmotion.Anxiety)
            ? [TechniqueId.Breathing, TechniqueId.Grounding]
            : TechniqueSuggester.Suggest(analysis with { Emotion = emotion });

        messages.Add(CompanionContent.OfferLine(suggestions[0], english));

        List<ChatQuickReply> quick =
        [
            PracticeReply(suggestions[0].ToString(), CompanionContent.StartLabel(suggestions[0], english))
        ];
        if (suggestions.Count > 1)
        {
            quick.Add(PracticeReply(suggestions[1].ToString(), CompanionContent.AlternativeLabel(suggestions[1], english)));
        }

        quick.Add(new ChatQuickReply(ChatQuickReplyKinds.More, CompanionContent.MoreLabel(english)));

        return Reply(
            messages,
            quick,
            state with
            {
                TurnsSinceOffer = 0,
                OfferedPrimary = suggestions[0].ToString(),
                OfferedAlternative = suggestions.Count > 1 ? suggestions[1].ToString() : null
            },
            emotion,
            theme);
    }

    private static CompanionReply Reply(
        List<string> messages,
        IReadOnlyList<ChatQuickReply> quick,
        CompanionState state,
        CompanionEmotion emotion,
        CompanionTheme? theme) =>
        new(messages, quick, null, state, emotion, theme);

    private static ChatQuickReply PracticeReply(string techniqueId, string label) =>
        new(ChatQuickReplyKinds.Practice, label, techniqueId);

    private static IReadOnlyList<ChatQuickReply> RatingReplies() =>
        Enumerable.Range(0, 11)
            .Select(n => new ChatQuickReply(ChatQuickReplyKinds.Rating, n.ToString(System.Globalization.CultureInfo.InvariantCulture), n.ToString(System.Globalization.CultureInfo.InvariantCulture)))
            .ToArray();

    private IReadOnlyList<ChatQuickReply> EmotionReplies() =>
        CompanionContent.ClarifyChoices(english)
            .Select(c => new ChatQuickReply(ChatQuickReplyKinds.Emotion, c.Label, c.Emotion.ToString()))
            .ToArray();

    private static CompanionEmotion ParseEmotion(string? name) =>
        Enum.TryParse(name, out CompanionEmotion e) ? e : CompanionEmotion.Unknown;

    private static CompanionTheme? ParseTheme(string? name) =>
        Enum.TryParse(name, out CompanionTheme t) ? t : null;
}
