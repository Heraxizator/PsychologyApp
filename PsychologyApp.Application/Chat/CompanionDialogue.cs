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
/// The messenger companion. A deterministic dialogue manager that behaves like an attentive listener:
/// it recognises what the person is doing (greeting, thanking, refusing to talk, asking for advice, asking who it is),
/// quotes their own words back, notices who the story is about and mixed feelings, asks one fitting question at a time,
/// measures tension on 0..10, summarises now and then, returns to the first thing they said and offers a practice when it makes sense.
/// Every decision is derived from <see cref="CompanionState"/> plus the new input, so a chat resumes after a restart and behaviour is testable.
/// No language model is involved.
/// </summary>
public sealed partial class CompanionDialogue(
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
    private const int RecapEveryTurns = 4;
    private const int MaxRedraws = 4;
    private const int RememberedReplies = 8;
    private const int RememberedPrefixLength = 80;
    private const int LongMessageWords = 30;

    /// <summary>First messages of a new chat. With a previous chat on record the companion remembers it and asks how things are now.</summary>
    public CompanionReply Open(CompanionState state, ChatSessionDTO? previous)
    {
        if (previous is { MessageCount: >= 2 } && !string.IsNullOrWhiteSpace(previous.Emotion) && previous.Emotion != nameof(CompanionEmotion.Unknown))
        {
            int days = (int)(_time.GetUtcNow().UtcDateTime - previous.UpdatedAt).TotalDays;
            return WithPending(new CompanionReply(
                [CompanionDialogueContent.ReturningGreeting(previous.Title, days, previous.LastIntensity, english)],
                CompanionDialogueContent.CheckInReplies(english),
                null,
                state,
                CompanionEmotion.Unknown,
                null));
        }

        return WithPending(new CompanionReply(
            [CompanionSmallTalk.FirstGreeting(Hour(), english, _random, state.UserName), CompanionContent.PrivacyNote(english)],
            [],
            null,
            state,
            CompanionEmotion.Unknown,
            null));
    }

    /// <summary>
    /// One turn. If a reply would repeat something the companion said a moment ago, it is drawn again (a few times at most),
    /// so answers that have several wordings do not sound like a recording.
    /// </summary>
    public CompanionReply Respond(CompanionState state, CompanionInput input)
    {
        CompanionState clean = state with { NamePending = false };
        CompanionReply reply = Compute(clean, state.NamePending, input);
        for (int attempt = 0; attempt < MaxRedraws && RepeatsRecent(reply, state); attempt++)
        {
            reply = Compute(clean, state.NamePending, input);
        }

        return reply with { State = reply.State with { RecentReplies = Remember(state.RecentReplies, reply.Messages) } };
    }

    private CompanionReply Compute(CompanionState state, bool namePending, CompanionInput input) => WithPending(input switch
    {
        CompanionInput.FreeText text => RespondToText(state, text.Value, namePending),
        CompanionInput.QuickReply quick => RespondToQuickReply(state, quick.Reply),
        _ => CompanionReply.Empty(state)
    });

    private static string Prefix(string message) => message.Length <= RememberedPrefixLength ? message : message[..RememberedPrefixLength];

    private static bool RepeatsRecent(CompanionReply reply, CompanionState state) =>
        reply.Messages.Any(m => state.RecentReplies.Contains(Prefix(m)));

    private static IReadOnlyList<string> Remember(IReadOnlyList<string> recent, IReadOnlyList<string> messages) =>
        [.. recent.Concat(messages.Select(Prefix)).TakeLast(RememberedReplies)];

    private int Hour() => _time.GetLocalNow().Hour;

    /// <summary>Message posted when the person comes back from a practice started in this chat.</summary>
    public CompanionReply FollowUpAfterPractice(CompanionState state) => WithPending(new(
        [CompanionDialogueContent.FollowUpAfterPractice(english)],
        RatingReplies(),
        null,
        state with { PendingPractice = null, PendingPracticeStartedUtc = null, AwaitingPostPracticeRating = true, ScaleAsked = true },
        ParseEmotion(state.Emotion),
        null));

    /// <summary>A bare "yes" or "no" is an answer only if the last thing the companion did was ask something.</summary>
    private static CompanionReply WithPending(CompanionReply reply)
    {
        if (reply.Messages.Count == 0)
        {
            return reply;
        }

        string last = reply.Messages[^1].TrimEnd();
        bool asked = last.EndsWith('?');
        return reply with { State = reply.State with { QuestionPending = asked, LastQuestion = asked ? last : reply.State.LastQuestion } };
    }

    private CompanionReply RespondToText(CompanionState previous, string raw, bool namePending)
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

        SituationAnalysis single = analyzer.Analyze(text);
        Utterance act = UtteranceClassifier.Classify(text, single.Emotion != CompanionEmotion.Unknown);
        if (act is Utterance.Yes or Utterance.No && !previous.QuestionPending)
        {
            act = Utterance.Acknowledge;
        }

        // "7" typed as text answers the tension question exactly like the chip does.
        if (TryParseRating(text, out int typedRating) && (previous.AwaitingPostPracticeRating || IsScaleQuestion(previous)))
        {
            return RespondToRating(previous, typedRating, ParseEmotion(previous.Emotion), ParseTheme(previous.Theme));
        }

        // "хорошо" after a question about the problem is agreement; only in a chat with no problem yet it is good news.
        if (act == Utterance.SharesGoodNews && previous.QuestionPending && ParseEmotion(previous.Emotion) != CompanionEmotion.Unknown && text.Split(' ').Length <= 3)
        {
            act = Utterance.Yes;
        }

        // Right after "what should I call you?" a short reply is a name, unless it is a feeling or another kind of move.
        if (namePending && act == Utterance.Statement && single.Emotion == CompanionEmotion.Unknown && UtteranceClassifier.LooksLikeBareName(text, out string bareName))
        {
            return Introduce(previous, bareName);
        }

        if (act == Utterance.IntroducesSelf && UtteranceClassifier.TryExtractName(text, out string given))
        {
            return Introduce(previous, given);
        }

        return act == Utterance.Statement
            ? RespondToStatement(previous, text, single)
            : RespondToConversationalMove(previous, act, text, single);
    }

    private CompanionReply Introduce(CompanionState previous, string name)
    {
        CompanionState state = previous with { UserName = name };
        CompanionEmotion emotion = ParseEmotion(state.Emotion);
        return Reply([CompanionSmallTalk.Nice(name, emotion != CompanionEmotion.Unknown, english)], [], state, emotion, ParseTheme(state.Theme));
    }

    /// <summary>The person is not describing a feeling: greeting, thanks, "I don't know", "who are you?", "what should I do?".</summary>
    private CompanionReply RespondToConversationalMove(CompanionState previous, Utterance act, string text, SituationAnalysis single)
    {
        // "What is this, I'm so scared" is a feeling, not a request for a lesson: explain only topics we know.
        string? explanation = act == Utterance.AsksToExplain ? CompanionKnowledge.Explain(text, english) : null;
        if (act == Utterance.AsksToExplain && explanation is null && single.Emotion != CompanionEmotion.Unknown)
        {
            return RespondToStatement(previous, text, single);
        }

        CompanionState state = previous with
        {
            Turns = previous.Turns + 1,
            TurnsSinceOffer = Math.Min(previous.TurnsSinceOffer + 1, 99),
            AwaitingPostPracticeRating = false
        };
        CompanionEmotion emotion = single.Emotion != CompanionEmotion.Unknown ? single.Emotion : ParseEmotion(state.Emotion);
        CompanionTheme? theme = ParseTheme(state.Theme);

        // "yes" or "I don't know" to "how strong is it, 0 to 10?" is not an answer: help with the scale instead of moving on.
        if (act is Utterance.Yes or Utterance.No or Utterance.Acknowledge or Utterance.DontKnow && IsScaleQuestion(previous))
        {
            return Reply([CompanionSmallTalk.ScaleHelp(english)], RatingReplies(), state, emotion, theme);
        }

        return act switch
        {
            Utterance.AsksToExplain => explanation is not null
                ? Reply([explanation], [], state, emotion, theme)
                : Reply([CompanionKnowledge.ExplainFallback(english)], CompanionKnowledge.TopicChips(english), state, emotion, theme),
            Utterance.AsksWhy => Reply([CompanionKnowledge.Why(emotion, english)], [], state, emotion, theme),
            Utterance.AsksIfNormal => Reply([CompanionKnowledge.Normal(emotion, english)], [], state, emotion, theme),
            Utterance.AsksForAdvice when emotion != CompanionEmotion.Unknown => Reply(
                [CompanionKnowledge.WhatHelps(emotion, english), CompanionKnowledge.AdviceOffer(english)],
                [CompanionKnowledge.PracticeChip(english), CompanionActContent.Understand(english)],
                state,
                emotion,
                theme),
            Utterance.Acknowledge => Reply([CompanionSmallTalk.Acknowledge(previous.QuestionPending, english, _random)], [], state, emotion, theme),
            Utterance.Reaction => Reply([CompanionSmallTalk.Reaction(text, UtteranceClassifier.IsSadReaction(text), english, _random)], [], state, emotion, theme),
            Utterance.SharesGoodNews => Reply([CompanionSmallTalk.GoodNews(english, _random)], CompanionSmallTalk.GoodNewsReplies(english), state, emotion, theme),
            Utterance.AsksHowAreYou => Reply([CompanionSmallTalk.HowAreYou(english, _random)], [], state, emotion, theme),
            Utterance.AsksName => Reply([CompanionSmallTalk.Name(english)], [], state with { NamePending = true }, emotion, theme),
            Utterance.AsksCapabilities => Reply([CompanionSmallTalk.Capabilities(english)], CompanionSmallTalk.CapabilityReplies(english), state, emotion, theme),
            Utterance.AsksToRepeat => Reply([CompanionSmallTalk.Repeat(previous.LastQuestion, english, _random)], [], state, emotion, theme),
            Utterance.SkipsQuestion => AskNextQuestion(state, [CompanionSmallTalk.Skipped(english, _random)], emotion, theme),
            Utterance.Greeting => Reply([CompanionSmallTalk.GreetingReply(Hour(), english, _random, state.UserName)], [], state, emotion, theme),
            Utterance.Thanks => Reply([CompanionSmallTalk.Thanks(state.UserName, english)], [CompanionActContent.Continue(english), CompanionActContent.Enough(english)], state, emotion, theme),
            Utterance.Goodbye => Reply([CompanionSmallTalk.Goodbye(state.UserName, english)], [], state, emotion, theme),
            Utterance.DontKnow => Reply([CompanionActContent.DontKnow(english)], EmotionReplies(), state with { UnknownStreak = 0 }, emotion, theme),
            Utterance.RefusesToTalk => Reply(
                [CompanionActContent.Refuses(english)],
                [CompanionActContent.BodyPractice(english), CompanionActContent.TalkOther(english), CompanionActContent.Enough(english)],
                state,
                emotion,
                theme),
            Utterance.AsksForAdvice => Reply([CompanionActContent.Advice(english)], [], state, emotion, theme),
            Utterance.AsksAboutCompanion => Reply([CompanionActContent.AboutCompanion(english)], [], state, emotion, theme),
            Utterance.ComplainsAboutCompanion => Reply(
                [CompanionActContent.Complaint(english)],
                [CompanionActContent.Vent(english), CompanionActContent.Understand(english), CompanionActContent.BodyPractice(english)],
                state,
                emotion,
                theme),
            Utterance.Yes => Reply([CompanionActContent.YesProbe(english, _random)], [], state, emotion, theme),
            Utterance.No => Reply([CompanionActContent.NoProbe(english, _random)], [], state, emotion, theme),
            _ => CompanionReply.Empty(state)
        };
    }

    private CompanionReply RespondToStatement(CompanionState previous, string text, SituationAnalysis analysis)
    {
        bool firstStatement = previous.RecentTexts.Count == 0;
        CompanionState state = previous.WithText(text) with
        {
            TurnsSinceOffer = Math.Min(previous.TurnsSinceOffer + 1, 99),
            AwaitingPostPracticeRating = false
        };

        if (analysis.Emotion == CompanionEmotion.Unknown)
        {
            analysis = analyzer.Analyze(string.Join(' ', state.RecentTexts)) with { HasBodySymptoms = analysis.HasBodySymptoms, IsIntense = analysis.IsIntense, Persons = analysis.Persons };
        }

        CompanionEmotion emotion = analysis.Emotion != CompanionEmotion.Unknown ? analysis.Emotion : ParseEmotion(state.Emotion);
        CompanionTheme? theme = analysis.Themes.Count > 0 ? analysis.Themes[0] : ParseTheme(state.Theme);
        CompanionPerson? person = analysis.Persons is { Count: > 0 } persons ? persons[0] : ParsePerson(state.Person);
        string? quote = QuoteExtractor.Pick(text, analyzer, emotion);

        state = state with
        {
            Emotion = emotion.ToString(),
            Theme = theme?.ToString(),
            Person = person?.ToString(),
            Secondary = analysis.Secondary.ToString(),
            UnknownStreak = emotion == CompanionEmotion.Unknown ? state.UnknownStreak + 1 : 0,
            FirstQuote = state.FirstQuote ?? quote
        };
        analysis = analysis with { Emotion = emotion };

        // Parroting a whole one-sentence message back sounds mechanical; a quote is for picking one sentence out of several.
        string? spoken = quote is not null && text.TrimEnd('.', '!', '?', '…', ' ') == quote ? null : quote;
        string heard = Listen(state, analysis, emotion, ParseEmotion(previous.Emotion), spoken, firstStatement);
        if (!firstStatement && text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length >= LongMessageWords)
        {
            heard = $"{CompanionSmallTalk.LongMessage(english, _random)} {heard}";
        }

        List<string> messages = [heard];

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

        bool known = emotion != CompanionEmotion.Unknown;
        if (!state.ScaleAsked && state.Turns >= 2 && known)
        {
            messages.Add(CompanionDialogueContent.ScaleQuestion(english));
            return Reply(messages, RatingReplies(), state with { ScaleAsked = true }, emotion, theme);
        }

        if (known && state.Turns >= 4 && state.Turns - state.LastRecapTurn >= RecapEveryTurns)
        {
            messages.Add(CompanionActContent.Recap(emotion, theme, person, state.FirstIntensity, state.LastIntensity, english));
            return Reply(messages, CompanionActContent.RecapReplies(english), state with { LastRecapTurn = state.Turns }, emotion, theme);
        }

        if (known && state.Turns >= 3 && state.TurnsSinceOffer >= 3)
        {
            return Offer(state, messages, analysis, emotion, theme, calming: false);
        }

        if (known && state.Turns >= 5 && !state.CallbackAsked && state.FirstQuote is { } first)
        {
            messages.Add(CompanionActContent.Callback(first, english));
            return Reply(messages, [], state with { CallbackAsked = true }, emotion, theme);
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

            case ChatQuickReplyKinds.Recap:
                return Reply(
                    [reply.Payload == "yes" ? CompanionActContent.RecapYes(english) : CompanionActContent.RecapNo(english)],
                    [],
                    state,
                    emotion,
                    theme);

            case ChatQuickReplyKinds.Act:
                return RespondToActChip(state, reply.Payload, emotion, theme);

            default:
                return CompanionReply.Empty(state);
        }
    }

    private CompanionReply RespondToActChip(CompanionState state, string? payload, CompanionEmotion emotion, CompanionTheme? theme)
    {
        switch (payload)
        {
            case "continue":
            case "vent":
                return Reply([CompanionDialogueContent.Question("more", english)], [], state, emotion, theme);

            case "enough":
                return Reply([CompanionActContent.Goodbye(english)], [], state, emotion, theme);

            case "other":
                return Reply([CompanionActContent.OtherTopic(english)], [], state, emotion, theme);

            case "understand":
                return AskNextQuestion(state, [], emotion, theme);

            case "practice":
                return Offer(state, [], new SituationAnalysis(emotion, 1, false, false, []), emotion, theme, calming: false);

            case { } topic when topic.StartsWith("topic:", StringComparison.Ordinal):
                return CompanionKnowledge.ExplainById(topic["topic:".Length..], english) is { } explanation
                    ? Reply([explanation, CompanionActContent.OtherTopic(english)], [], state, emotion, theme)
                    : CompanionReply.Empty(state);

            case "body":
                return Offer(
                    state,
                    [CompanionDialogueContent.UrgentBridge(english)],
                    new SituationAnalysis(emotion, 1, true, false, []),
                    emotion,
                    theme,
                    calming: true);

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

    private string Listen(CompanionState state, SituationAnalysis analysis, CompanionEmotion emotion, CompanionEmotion before, string? quote, bool firstStatement)
    {
        if (firstStatement)
        {
            string validation = analysis.Secondary != CompanionEmotion.Unknown && emotion != CompanionEmotion.Unknown
                ? CompanionActContent.Mixed(emotion, analysis.Secondary, english)
                : CompanionContent.Reflection(analysis with { Emotion = emotion }, english, _random);
            return quote is null ? validation : $"{CompanionDialogueContent.QuoteLine(quote, english, _random)} {validation}";
        }

        string ack = emotion != CompanionEmotion.Unknown && before != CompanionEmotion.Unknown && emotion != before
            ? CompanionSmallTalk.Shift(before, emotion, english)
            : emotion != CompanionEmotion.Unknown && state.Turns % 2 == 0
                ? CompanionSmallTalk.Validation(emotion, english, _random)
                : CompanionDialogueContent.Acknowledgement(english, _random);
        if (state.UserName is not null && state.Turns % 3 == 0)
        {
            ack = CompanionSmallTalk.Address(state.UserName, ack);
        }

        if (quote is null || state.RecentTexts.Count % 2 != 0)
        {
            return ack;
        }

        return english ? $"“{quote}”. {ack}" : $"«{quote}». {ack}";
    }

    private CompanionReply AskNextQuestion(CompanionState state, List<string> messages, CompanionEmotion emotion, CompanionTheme? theme)
    {
        string? id = emotion == CompanionEmotion.Unknown
            ? null
            : CompanionActContent.TargetedQuestionId(emotion, ParsePerson(state.Person), state.AskedQuestions);
        id ??= CompanionDialogueContent.NextQuestionId(emotion, state.AskedQuestions);

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

    private static readonly IReadOnlyList<ChatQuickReply> RatingChips = Enumerable.Range(0, 11)
        .Select(n => n.ToString(System.Globalization.CultureInfo.InvariantCulture))
        .Select(label => new ChatQuickReply(ChatQuickReplyKinds.Rating, label, label))
        .ToArray();

    private static IReadOnlyList<ChatQuickReply> RatingReplies() => RatingChips;

    private IReadOnlyList<ChatQuickReply> EmotionReplies() =>
        CompanionContent.ClarifyChoices(english)
            .Select(c => new ChatQuickReply(ChatQuickReplyKinds.Emotion, c.Label, c.Emotion.ToString()))
            .ToArray();

    private static bool IsScaleQuestion(CompanionState state) =>
        state.QuestionPending
        && state.LastQuestion is { } question
        && (question.Contains("от 0 до 10", StringComparison.Ordinal) || question.Contains("from 0 to 10", StringComparison.Ordinal));

    /// <summary>"7", "7/10", "около 6", "about 4": a typed answer to the tension question. Anything longer is a story, not a number.</summary>
    private static bool TryParseRating(string text, out int rating)
    {
        rating = 0;
        System.Text.RegularExpressions.Match m = RatingPattern().Match(text.Trim());
        return m.Success && int.TryParse(m.Groups["n"].Value, out rating) && rating is >= 0 and <= 10;
    }

    [System.Text.RegularExpressions.GeneratedRegex(
        @"^(?:около|примерно|где-то|наверное|about|around|maybe)?\s*(?<n>\d{1,2})(?:\s*(?:/|из|out of)\s*10)?\s*[.!]*$",
        System.Text.RegularExpressions.RegexOptions.CultureInvariant | System.Text.RegularExpressions.RegexOptions.IgnoreCase)]
    private static partial System.Text.RegularExpressions.Regex RatingPattern();

    private static CompanionEmotion ParseEmotion(string? name) =>
        Enum.TryParse(name, out CompanionEmotion e) ? e : CompanionEmotion.Unknown;

    private static CompanionTheme? ParseTheme(string? name) =>
        Enum.TryParse(name, out CompanionTheme t) ? t : null;

    private static CompanionPerson? ParsePerson(string? name) =>
        Enum.TryParse(name, out CompanionPerson p) ? p : null;
}
