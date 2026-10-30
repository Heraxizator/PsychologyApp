using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

/// <summary>The exercises the companion runs inside the chat, and what it suggests besides practices.</summary>
public sealed class CompanionGuidedTests
{
    private sealed class FixedTime(DateTime utc) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(utc, TimeSpan.Zero);
    }

    private static CompanionDialogue Create(int heavyChats = 0, IReadOnlySet<string>? taken = null) =>
        new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english: false, new Random(3),
            new FixedTime(new DateTime(2026, 10, 16, 12, 0, 0, DateTimeKind.Utc)), recentHeavyChats: heavyChats, recentlyTakenTests: taken);

    private static CompanionReply Say(CompanionDialogue d, CompanionState state, string text) => d.Respond(state, new CompanionInput.FreeText(text));

    private static CompanionReply Tap(CompanionDialogue d, CompanionState state, string kind, string payload) =>
        d.Respond(state, new CompanionInput.QuickReply(new ChatQuickReply(kind, payload, payload)));

    private static readonly CompanionState Anxious = new() { Turns = 3, Emotion = nameof(CompanionEmotion.Anxiety) };

    [Fact]
    public void An_unknown_saved_protocol_is_cleared_instead_of_trapping_the_chat_in_repeated_questions()
    {
        CompanionState restored = CompanionState.Deserialize(
            "{\"turns\":3,\"emotion\":\"Anxiety\",\"protocol\":\"retired\",\"protocolStep\":4,\"protocolNotes\":[\"old answer\"]}");

        CompanionReply reply = Say(Create(), restored, "Меня снова тревожит собеседование");

        Assert.Null(reply.State.Protocol);
        Assert.Empty(reply.State.ProtocolNotes);
        Assert.NotEmpty(reply.Messages);

        CompanionReply invalidChip = Tap(Create(), Anxious, ChatQuickReplyKinds.Act, "protocol:retired");
        Assert.Null(invalidChip.State.Protocol);
        Assert.Null(invalidChip.Action);
    }

    [Fact]
    public void A_thought_is_checked_step_by_step_and_the_change_in_belief_is_named()
    {
        // The chat does the work of a thought record with the person: the summary must carry their own words and both ratings.
        CompanionDialogue dialogue = Create();
        CompanionReply reply = Tap(dialogue, Anxious, ChatQuickReplyKinds.Act, "protocol:think");
        reply = Say(dialogue, reply.State, "Я завалю собеседование");
        reply = Tap(dialogue, reply.State, ChatQuickReplyKinds.Rating, "8");
        reply = Say(dialogue, reply.State, "Я плохо спал и не готовился");
        reply = Say(dialogue, reply.State, "Прошлые два собеседования прошли нормально");
        reply = Say(dialogue, reply.State, "Что волноваться нормально и он справится");
        reply = Say(dialogue, reply.State, "Я волнуюсь, но у меня есть опыт, и я могу подготовиться вечером");
        reply = Say(dialogue, reply.State, "4");

        string summary = Assert.Single(reply.Messages);
        Assert.Contains("Я завалю собеседование", summary);
        Assert.Contains("у меня есть опыт", summary);
        Assert.Contains("8", summary);
        Assert.Contains("4", summary);
        Assert.Null(reply.State.Protocol);
    }

    [Fact]
    public void Looking_deeper_goes_from_a_thought_to_what_it_says_about_oneself_and_a_kinder_wording()
    {
        // The downward arrow: the summary carries the person's own thought, the belief under it and their kinder wording,
        // and points to a psychologist for a belief that keeps coming back.
        CompanionDialogue dialogue = Create();
        CompanionReply reply = Tap(dialogue, Anxious with { Emotion = nameof(CompanionEmotion.Guilt) }, ChatQuickReplyKinds.Act, "protocol:deeper");
        Assert.DoesNotContain("что случилось", reply.Messages[0], StringComparison.OrdinalIgnoreCase);
        reply = Say(dialogue, reply.State, "Я опять подвёл команду");
        reply = Say(dialogue, reply.State, "Что на меня нельзя положиться");
        reply = Say(dialogue, reply.State, "Я никчёмный");
        reply = Say(dialogue, reply.State, "Что одна ошибка не делает его никчёмным");
        reply = Say(dialogue, reply.State, "Я ошибся, но обычно на меня можно положиться");

        string summary = Assert.Single(reply.Messages);
        Assert.Contains("Я опять подвёл команду", summary);
        Assert.Contains("Я никчёмный", summary);
        Assert.Contains("обычно на меня можно положиться", summary);
        Assert.Contains("психолог", summary);
        Assert.Null(reply.State.Protocol);
    }

    [Theory]
    [InlineData(nameof(CompanionEmotion.Guilt), true)]
    [InlineData(nameof(CompanionEmotion.Sadness), true)]
    [InlineData(nameof(CompanionEmotion.Anxiety), false)]
    public void Looking_deeper_is_offered_for_guilt_and_sadness_next_to_checking_the_thought(string emotion, bool offered)
    {
        CompanionReply offer = Tap(Create(), Anxious with { Emotion = emotion }, ChatQuickReplyKinds.Act, "practice");

        Assert.Contains(offer.QuickReplies, chip => chip.Payload == "protocol:think");
        Assert.Equal(offered, offer.QuickReplies.Any(chip => chip.Payload == "protocol:deeper"));
    }

    [Fact]
    public void Crisis_words_while_looking_deeper_open_help_instead_of_the_next_question()
    {
        CompanionDialogue dialogue = Create();
        CompanionReply reply = Tap(dialogue, Anxious with { Emotion = nameof(CompanionEmotion.Guilt) }, ChatQuickReplyKinds.Act, "protocol:deeper");
        reply = Say(dialogue, reply.State, "Я всех подвожу");

        reply = Say(dialogue, reply.State, "Что всем будет лучше без меня");

        Assert.Equal(DialogueActionKind.OpenCrisisHub, reply.Action?.Kind);
    }

    [Fact]
    public void Crisis_words_inside_an_exercise_open_help_instead_of_the_next_question()
    {
        CompanionDialogue dialogue = Create();
        CompanionReply started = Tap(dialogue, Anxious, ChatQuickReplyKinds.Act, "protocol:think");

        CompanionReply reply = Say(dialogue, started.State, "Лучше бы меня не было, хочу умереть");

        Assert.Equal(DialogueActionKind.OpenCrisisHub, reply.Action?.Kind);
    }

    [Fact]
    public void Crisis_words_in_a_textual_exercise_chip_also_open_help()
    {
        CompanionDialogue dialogue = Create();
        CompanionReply started = Tap(dialogue, Anxious, ChatQuickReplyKinds.Act, "protocol:solve");

        CompanionReply reply = Tap(dialogue, started.State, ChatQuickReplyKinds.Act, "say:Я хочу покончить с собой");

        Assert.Equal(DialogueActionKind.OpenCrisisHub, reply.Action?.Kind);
        Assert.Empty(reply.QuickReplies);
    }

    [Fact]
    public void The_person_can_stop_an_exercise_at_any_step()
    {
        CompanionDialogue dialogue = Create();
        CompanionReply started = Tap(dialogue, Anxious, ChatQuickReplyKinds.Act, "protocol:solve");

        CompanionReply stopped = Say(dialogue, started.State, "хватит");

        Assert.Null(stopped.State.Protocol);
        Assert.Contains(CompanionProtocolContent.Stopped(false), stopped.Messages);
    }

    [Fact]
    public void Problem_solving_ends_in_a_first_step_that_is_kept_for_the_next_chat()
    {
        CompanionDialogue dialogue = Create();
        CompanionReply reply = Tap(dialogue, Anxious, ChatQuickReplyKinds.Act, "protocol:solve");
        reply = Say(dialogue, reply.State, "Начальник завалил задачами");
        reply = Say(dialogue, reply.State, "Поговорить с ним, уволиться, молча терпеть");
        reply = Say(dialogue, reply.State, "Поговорить с ним");
        reply = Say(dialogue, reply.State, "Написать ему в среду утром список задач и спросить, что важнее");

        Assert.Equal("Написать ему в среду утром список задач и спросить, что важнее", reply.State.NewPlan);
        Assert.Contains("Поговорить с ним", reply.Messages[0]);
    }

    [Fact]
    public void A_step_decided_last_time_is_asked_about_first_and_answering_resolves_it()
    {
        CompanionDialogue dialogue = Create();

        CompanionReply opened = dialogue.Open(new CompanionState { OpenPlan = "Позвонить маме" }, previous: null);
        Assert.Contains("Позвонить маме", opened.Messages[0]);
        Assert.Equal(["plan:done", "plan:partly", "plan:no"], opened.QuickReplies.Select(chip => chip.Payload));

        CompanionReply answered = Tap(dialogue, opened.State, ChatQuickReplyKinds.Act, "plan:no");
        Assert.True(answered.State.PlanResolved);
        Assert.Null(answered.State.OpenPlan);
    }

    [Fact]
    public void An_offer_for_an_anxious_thought_includes_checking_it_and_an_article_once()
    {
        CompanionDialogue dialogue = Create();

        CompanionReply offer = Tap(dialogue, Anxious, ChatQuickReplyKinds.Act, "practice");
        CompanionReply again = Tap(dialogue, offer.State, ChatQuickReplyKinds.Act, "practice");

        Assert.Contains(offer.QuickReplies, chip => chip.Payload == "protocol:think");
        Assert.Contains(offer.QuickReplies, chip => chip.Payload == "article:anxiety");
        Assert.DoesNotContain(again.QuickReplies, chip => chip.Payload is "protocol:think" or "article:anxiety");
    }

    [Theory]
    [InlineData(nameof(CompanionEvent.Performance), "protocol:think")]
    [InlineData(nameof(CompanionEvent.Overload), "protocol:solve")]
    public void An_exam_fear_gets_a_thought_check_and_too_much_work_a_plan(string happened, string expected)
    {
        CompanionReply offer = Tap(Create(), Anxious with { Event = happened }, ChatQuickReplyKinds.Act, "practice");

        Assert.Contains(offer.QuickReplies, chip => chip.Payload == expected);
    }

    [Fact]
    public void Heavy_chats_that_keep_coming_back_lead_to_the_matching_questionnaire()
    {
        // From the third heavy chat in two weeks the person is pointed at a measure, not only at another practice.
        CompanionReply offer = Tap(Create(heavyChats: 2), Anxious, ChatQuickReplyKinds.Act, "practice");
        Assert.Contains(offer.QuickReplies, chip => chip.Payload == "test:gad7");

        CompanionReply alreadyTaken = Tap(Create(heavyChats: 2, taken: new HashSet<string> { "gad7" }), Anxious, ChatQuickReplyKinds.Act, "practice");
        Assert.DoesNotContain(alreadyTaken.QuickReplies, chip => chip.Payload == "test:gad7");

        CompanionReply firstHeavy = Tap(Create(heavyChats: 1), Anxious, ChatQuickReplyKinds.Act, "practice");
        Assert.DoesNotContain(firstHeavy.QuickReplies, chip => chip.Payload == "test:gad7");
    }

    [Fact]
    public void A_test_or_article_chip_opens_it()
    {
        CompanionDialogue dialogue = Create();

        Assert.Equal(new DialogueAction(DialogueActionKind.OpenTest, TestId: "phq9"), Tap(dialogue, Anxious, ChatQuickReplyKinds.Act, "test:phq9").Action);
        Assert.Equal(new DialogueAction(DialogueActionKind.OpenArticle, ArticleId: "panic"), Tap(dialogue, Anxious, ChatQuickReplyKinds.Act, "article:panic").Action);
    }
}
