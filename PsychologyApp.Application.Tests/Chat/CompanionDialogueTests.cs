using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

public class CompanionDialogueTests
{
    private sealed class FixedTime(DateTime utc) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(utc, TimeSpan.Zero);
    }

    private static readonly DateTime Now = new(2026, 9, 21, 12, 0, 0, DateTimeKind.Utc);

    private static CompanionDialogue Create(bool english = false) =>
        new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english, new Random(3), new FixedTime(Now));

    private static CompanionReply Say(CompanionDialogue d, CompanionState state, string text) =>
        d.Respond(state, new CompanionInput.FreeText(text));

    private static CompanionReply Tap(CompanionDialogue d, CompanionState state, string kind, string label, string? payload = null) =>
        d.Respond(state, new CompanionInput.QuickReply(new ChatQuickReply(kind, label, payload)));

    [Fact]
    public void First_chat_greets_and_promises_privacy_without_quick_replies()
    {
        CompanionReply reply = Create().Open(new CompanionState(), previous: null);

        Assert.Equal(2, reply.Messages.Count);
        Assert.Contains("устройстве", reply.Messages[1]);
        Assert.Empty(reply.QuickReplies);
    }

    [Fact]
    public void Returning_person_is_reminded_of_the_last_chat_and_asked_how_it_is_now()
    {
        ChatSessionDTO previous = new()
        {
            Id = 1, Title = "Тревога · работа", Emotion = "Anxiety", LastIntensity = 8, MessageCount = 6,
            CreatedAt = Now.AddDays(-3), UpdatedAt = Now.AddDays(-3)
        };

        CompanionReply reply = Create().Open(new CompanionState(), previous);

        Assert.Single(reply.Messages);
        Assert.Contains("«Тревога · работа»", reply.Messages[0]);
        Assert.Contains("3 дня назад", reply.Messages[0]);
        Assert.Contains("8/10", reply.Messages[0]);
        Assert.Equal(4, reply.QuickReplies.Count);
        Assert.All(reply.QuickReplies, q => Assert.Equal(ChatQuickReplyKinds.CheckIn, q.Kind));
    }

    [Fact]
    public void A_previous_chat_with_no_conversation_is_not_mentioned()
    {
        ChatSessionDTO previous = new() { Id = 1, Title = "Новый чат", Emotion = "Anxiety", MessageCount = 1, UpdatedAt = Now };

        Assert.Equal(2, Create().Open(new CompanionState(), previous).Messages.Count);
    }

    [Fact]
    public void First_message_is_quoted_back_then_one_question_follows()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Завтра важная презентация, я очень тревожусь. Не могу нормально спать");

        Assert.Equal(CompanionEmotion.Anxiety, reply.Emotion);
        Assert.Equal(2, reply.Messages.Count);
        Assert.Contains("«", reply.Messages[0]);
        Assert.Contains("тревож", reply.Messages[0], StringComparison.OrdinalIgnoreCase);
        Assert.EndsWith("?", reply.Messages[1]);
        Assert.Empty(reply.QuickReplies);
        Assert.Equal(1, reply.State.Turns);
        Assert.Single(reply.State.AskedQuestions);
    }

    [Fact]
    public void Panic_gets_help_for_the_body_first_not_questions()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Паническая атака, не могу дышать, сердце колотится");

        Assert.Contains(reply.Messages, m => m.Contains("Заземление 5-4-3-2-1"));
        Assert.Equal(4, reply.QuickReplies.Count);
        Assert.Equal("Grounding", reply.QuickReplies[0].Payload);
        Assert.Contains(reply.QuickReplies, c => c.Payload == "resource:somatic");
        Assert.Equal(ChatQuickReplyKinds.More, reply.QuickReplies[^1].Kind);
    }

    [Fact]
    public void Second_message_asks_for_a_tension_rating_with_eleven_chips()
    {
        CompanionDialogue d = Create();
        CompanionState state = Say(d, new CompanionState(), "Меня бесит начальник, так злюсь").State;

        CompanionReply reply = Say(d, state, "Он опять при всех раскритиковал мой отчёт");

        Assert.Equal(11, reply.QuickReplies.Count);
        Assert.All(reply.QuickReplies, q => Assert.Equal(ChatQuickReplyKinds.Rating, q.Kind));
        Assert.Contains("от 0 до 10", reply.Messages[^1]);
        Assert.True(reply.State.ScaleAsked);
    }

    [Fact]
    public void High_rating_leads_to_a_calming_practice_and_is_remembered()
    {
        CompanionDialogue d = Create();
        CompanionState state = Say(d, Say(d, new CompanionState(), "Меня бесит начальник").State, "Опять раскритиковал при всех").State;

        CompanionReply reply = Tap(d, state, ChatQuickReplyKinds.Rating, "9", "9");

        Assert.Contains(reply.Messages, m => m.Contains("очень сильное напряжение"));
        Assert.Equal(9, reply.State.FirstIntensity);
        Assert.Equal(9, reply.State.LastIntensity);
        Assert.Contains(reply.QuickReplies, q => q.Kind == ChatQuickReplyKinds.Practice);
    }

    [Fact]
    public void Low_rating_keeps_listening_instead_of_pushing_a_practice()
    {
        CompanionDialogue d = Create();
        CompanionState state = Say(d, Say(d, new CompanionState(), "Меня бесит начальник").State, "Опять раскритиковал при всех").State;

        CompanionReply reply = Tap(d, state, ChatQuickReplyKinds.Rating, "2", "2");

        Assert.Contains(reply.Messages, m => m.Contains("не на пределе"));
        Assert.DoesNotContain(reply.QuickReplies, q => q.Kind == ChatQuickReplyKinds.Practice);
        Assert.EndsWith("?", reply.Messages[^1]);
    }

    [Fact]
    public void A_practice_is_offered_from_the_third_message_and_questions_never_repeat()
    {
        CompanionDialogue d = Create();
        CompanionState state = new();
        CompanionReply reply = CompanionReply.Empty(state);
        string[] messages =
        [
            "Мне тревожно из-за работы", "Начальник постоянно давит сроками", "Я боюсь, что меня уволят"
        ];

        foreach (string message in messages)
        {
            reply = Say(d, state, message);
            state = reply.State;
            if (reply.QuickReplies.Any(q => q.Kind == ChatQuickReplyKinds.Rating))
            {
                state = Tap(d, state, ChatQuickReplyKinds.Rating, "5", "5").State;
            }
        }

        Assert.True(reply.QuickReplies.Any(q => q.Kind == ChatQuickReplyKinds.Practice) || state.OfferedPrimary is not null);
        Assert.Equal(state.AskedQuestions.Distinct().Count(), state.AskedQuestions.Count);
    }

    [Fact]
    public void Unclear_messages_lead_to_a_choice_of_feelings_after_two_tries()
    {
        CompanionDialogue d = Create();

        CompanionReply first = Say(d, new CompanionState(), "Ну как-то всё не так");
        CompanionReply second = Say(d, first.State, "Не знаю даже что сказать");

        Assert.Empty(first.QuickReplies);
        Assert.EndsWith("?", first.Messages[^1]);
        Assert.Equal(8, second.QuickReplies.Count);
        Assert.All(second.QuickReplies, q => Assert.Equal(ChatQuickReplyKinds.Emotion, q.Kind));
    }

    [Fact]
    public void Picking_a_feeling_sets_it_and_the_conversation_continues()
    {
        CompanionDialogue d = Create();
        CompanionState state = Say(d, Say(d, new CompanionState(), "Ну как-то всё не так").State, "Не знаю даже что сказать").State;

        CompanionReply reply = Tap(d, state, ChatQuickReplyKinds.Emotion, "Злость", nameof(CompanionEmotion.Anger));

        Assert.Equal("Anger", reply.State.Emotion);
        Assert.Contains("злость", reply.Messages[0]);
    }

    [Fact]
    public void Choosing_a_practice_starts_it_and_remembers_to_ask_about_it_later()
    {
        CompanionReply reply = Tap(Create(), new CompanionState { Emotion = "Anger" }, ChatQuickReplyKinds.Practice, "Начнём", "Observer");

        Assert.Equal(new DialogueAction(DialogueActionKind.StartTechnique, TechniqueId.Observer), reply.Action);
        Assert.Equal("Observer", reply.State.PendingPractice);
        Assert.Equal(Now, reply.State.PendingPracticeStartedUtc);
        Assert.Contains("вернитесь сюда", reply.Messages[0]);
    }

    [Fact]
    public void After_a_practice_the_tension_is_compared_with_the_earlier_rating()
    {
        CompanionDialogue d = Create();
        CompanionState state = new() { Emotion = "Anger", FirstIntensity = 8, LastIntensity = 8, PendingPractice = "Observer", PendingPracticeStartedUtc = Now };

        CompanionReply follow = d.FollowUpAfterPractice(state);
        CompanionReply better = Tap(d, follow.State, ChatQuickReplyKinds.Rating, "4", "4");
        CompanionReply same = Tap(d, follow.State, ChatQuickReplyKinds.Rating, "8", "8");
        CompanionReply worse = Tap(d, follow.State, ChatQuickReplyKinds.Rating, "9", "9");

        Assert.Null(follow.State.PendingPractice);
        Assert.True(follow.State.AwaitingPostPracticeRating);
        Assert.Equal(11, follow.QuickReplies.Count);
        Assert.Contains("Было 8, стало 4", better.Messages[0]);
        Assert.Contains("около 8", same.Messages[0]);
        Assert.Equal(2, same.QuickReplies.Count);
        Assert.Contains("Стало сильнее", worse.Messages[0]);
        Assert.False(better.State.AwaitingPostPracticeRating);
    }

    [Theory]
    [InlineData("Мне не хочется жить", "Russian")]
    [InlineData("I don't want to live anymore", "English")]
    public void Crisis_text_gets_help_and_opens_the_crisis_hub_without_advancing_the_dialogue(string text, string _)
    {
        CompanionState state = new() { Turns = 2 };

        CompanionReply reply = Say(Create(english: text.StartsWith("I")), state, text);

        Assert.Equal(DialogueActionKind.OpenCrisisHub, reply.Action!.Kind);
        Assert.Single(reply.Messages);
        Assert.Empty(reply.QuickReplies);
        Assert.Equal(2, reply.State.Turns);
    }

    [Fact]
    public void Blank_input_does_nothing()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "   ");

        Assert.Empty(reply.Messages);
    }

    [Fact]
    public void Check_in_answers_get_a_fitting_response()
    {
        CompanionDialogue d = Create();

        Assert.Contains("хорошая новость", Tap(d, new CompanionState(), ChatQuickReplyKinds.CheckIn, "Стало легче", "better").Messages[0]);
        Assert.Contains("Мне жаль", Tap(d, new CompanionState(), ChatQuickReplyKinds.CheckIn, "Стало тяжелее", "worse").Messages[0]);
        Assert.Contains("О чём", Tap(d, new CompanionState(), ChatQuickReplyKinds.CheckIn, "О другом", "other").Messages[0]);
    }

    [Fact]
    public void English_conversation_speaks_english()
    {
        CompanionReply reply = Say(Create(english: true), new CompanionState(), "I feel so anxious about tomorrow's meeting. I can't sleep");

        Assert.Contains("“", reply.Messages[0]);
        Assert.DoesNotContain(reply.Messages, m => m.Any(c => c is >= 'Ѐ' and <= 'ӿ'));
    }

    [Fact]
    public void State_survives_a_round_trip_and_a_damaged_blob_yields_a_fresh_state()
    {
        CompanionState state = new()
        {
            Turns = 4, Emotion = "Guilt", Theme = "Family", FirstIntensity = 8, LastIntensity = 5, TurnsSinceOffer = 1, UnknownStreak = 1,
            ScaleAsked = true, AwaitingPostPracticeRating = true, AskedQuestions = ["kind", "need"], RecentTexts = ["a", "б"],
            PendingPractice = "Observer", PendingPracticeStartedUtc = Now, OfferedPrimary = "SelfCompassion", OfferedAlternative = "ThoughtRecord"
        };

        CompanionState restored = CompanionState.Deserialize(state.Serialize());

        Assert.Equal(state.Turns, restored.Turns);
        Assert.Equal(state.Emotion, restored.Emotion);
        Assert.Equal(state.Theme, restored.Theme);
        Assert.Equal(state.FirstIntensity, restored.FirstIntensity);
        Assert.Equal(state.LastIntensity, restored.LastIntensity);
        Assert.Equal(state.AskedQuestions, restored.AskedQuestions);
        Assert.Equal(state.RecentTexts, restored.RecentTexts);
        Assert.Equal(state.PendingPracticeStartedUtc, restored.PendingPracticeStartedUtc);
        Assert.True(restored.ScaleAsked);
        Assert.True(restored.AwaitingPostPracticeRating);
        Assert.Equal(0, CompanionState.Deserialize("{ not json").Turns);
        Assert.Equal(0, CompanionState.Deserialize(null).Turns);
    }

    [Fact]
    public void Recent_texts_are_capped_and_asked_questions_are_bounded()
    {
        CompanionState state = new();
        for (int i = 0; i < 10; i++)
        {
            state = state.WithText($"t{i}").WithAsked($"q{i}");
        }

        Assert.Equal(["t7", "t8", "t9"], state.RecentTexts);
        Assert.Equal(10, state.Turns);
    }
}

public class QuoteExtractorTests
{
    private readonly LexiconSituationAnalyzer _analyzer = new();

    [Fact]
    public void Prefers_the_sentence_that_carries_the_feeling()
    {
        string? quote = QuoteExtractor.Pick("Вчера был обычный день. Мне так обидно, что меня не позвали. Потом я пошёл домой.", _analyzer, CompanionEmotion.Resentment);

        Assert.Equal("Мне так обидно, что меня не позвали", quote);
    }

    [Fact]
    public void Long_sentences_are_shortened_at_a_word_boundary()
    {
        string text = "Я очень тревожусь " + string.Join(' ', Enumerable.Repeat("потому что всё время думаю о будущем", 6));

        string? quote = QuoteExtractor.Pick(text, _analyzer, CompanionEmotion.Anxiety, maxLength: 60);

        Assert.NotNull(quote);
        Assert.EndsWith("…", quote);
        Assert.True(quote!.Length <= 61);
    }

    [Theory]
    [InlineData("Ок")]
    [InlineData("   ")]
    [InlineData("")]
    public void Very_short_text_is_not_quoted(string text) =>
        Assert.Null(QuoteExtractor.Pick(text, _analyzer, CompanionEmotion.Unknown));

    [Fact]
    public void Falls_back_to_the_longest_sentence_when_no_feeling_is_recognised()
    {
        string? quote = QuoteExtractor.Pick("Да. Сегодня был очень странный и долгий день на работе.", _analyzer, CompanionEmotion.Unknown);

        Assert.Equal("Сегодня был очень странный и долгий день на работе", quote);
    }
}
