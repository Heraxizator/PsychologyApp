using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

/// <summary>
/// The trauma-informed part of the companion: it never asks for the details of a hard event, works on what the event changed instead, and
/// steps back to stabilising whenever it is too hard.
/// </summary>
public sealed class CompanionTraumaTests
{
    private sealed class FixedTime(DateTime utc) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(utc, TimeSpan.Zero);
    }

    private static CompanionDialogue Create(bool english = false) =>
        new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english, new Random(3), new FixedTime(new DateTime(2026, 10, 17, 12, 0, 0, DateTimeKind.Utc)));

    private static CompanionReply Say(CompanionDialogue d, CompanionState state, string text) => d.Respond(state, new CompanionInput.FreeText(text));

    private static CompanionReply Tap(CompanionDialogue d, CompanionState state, string kind, string payload) =>
        d.Respond(state, new CompanionInput.QuickReply(new ChatQuickReply(kind, payload, payload)));

    private static CompanionReply StartImpact(CompanionDialogue dialogue) =>
        Tap(dialogue, new CompanionState { Turns = 2, TraumaAcknowledged = true }, ChatQuickReplyKinds.Act, "protocol:" + CompanionTraumaContent.Impact);

    [Theory]
    [InlineData("В детстве отчим меня избивал, и я до сих пор об этом думаю")]
    [InlineData("Год назад я попала в аварию и с тех пор не могу сесть в машину")]
    public void A_hard_event_is_met_without_asking_for_its_details(string text)
    {
        CompanionDialogue dialogue = Create();
        CompanionState state = dialogue.Open(new CompanionState(), previous: null).State;

        CompanionReply reply = Say(dialogue, state, text);

        Assert.Equal([CompanionTraumaContent.Acknowledge(false)], reply.Messages);
        Assert.Contains(reply.QuickReplies, chip => chip.Payload == "protocol:" + CompanionTraumaContent.Impact);
        Assert.Contains("what_happened", reply.State.AskedQuestions);
        Assert.True(reply.State.TraumaAcknowledged);
    }

    [Theory]
    [InlineData("Я пытаюсь уснуть уже час")]
    [InlineData("Это не травма, просто устал")]
    public void Ordinary_words_and_a_denial_are_not_read_as_trauma(string text) =>
        Assert.False(new LexiconSituationAnalyzer().Analyze(text).MentionsTrauma);

    [Fact]
    public void A_stuck_point_is_worked_through_without_the_event_and_noted_for_a_specialist()
    {
        CompanionDialogue dialogue = Create();
        CompanionReply reply = StartImpact(dialogue);
        Assert.Equal(CompanionTraumaContent.DistressQuestion(false), reply.Messages[0]);

        reply = Tap(dialogue, reply.State, ChatQuickReplyKinds.Rating, "4");
        reply = Tap(dialogue, reply.State, ChatQuickReplyKinds.Act, "say:guilt");
        reply = Tap(dialogue, reply.State, ChatQuickReplyKinds.Act, "say:Это моя вина");
        reply = Tap(dialogue, reply.State, ChatQuickReplyKinds.Rating, "9");
        Assert.Equal(CompanionTraumaContent.ThemeQuestion("guilt", 0, false), reply.Messages[0]);
        reply = Say(dialogue, reply.State, "Мне было девять, я не знал, что так бывает");
        reply = Say(dialogue, reply.State, "Отвечает взрослый, который это сделал");
        reply = Say(dialogue, reply.State, "Я был ребёнком и не мог это остановить");
        reply = Say(dialogue, reply.State, "5");

        string summary = Assert.Single(reply.Messages);
        Assert.Contains("Это моя вина", summary);
        Assert.Contains("Я был ребёнком и не мог это остановить", summary);
        Assert.Null(reply.State.Protocol);
        StuckPointNote note = Assert.Single(TraumaNotes.Deserialize(reply.State.TraumaNotes).StuckPoints);
        Assert.Equal(("guilt", 9, 5), (note.Theme, note.Before, note.After));
    }

    [Fact]
    public void When_it_is_too_hard_the_work_waits_and_the_present_comes_first()
    {
        // At 7 of 10 the person is helped back to the present; questions about beliefs would only add weight.
        CompanionDialogue dialogue = Create();

        CompanionReply reply = Tap(dialogue, StartImpact(dialogue).State, ChatQuickReplyKinds.Rating, "8");

        Assert.Equal([CompanionTraumaContent.TooHardNow(false)], reply.Messages);
        Assert.Contains(reply.QuickReplies, chip => chip.Payload == nameof(TechniqueId.Flashback));
        Assert.Null(reply.State.Protocol);
    }

    [Fact]
    public void A_memory_taking_over_in_the_middle_stops_the_work()
    {
        CompanionDialogue dialogue = Create();
        CompanionReply reply = Tap(dialogue, StartImpact(dialogue).State, ChatQuickReplyKinds.Rating, "3");
        reply = Tap(dialogue, reply.State, ChatQuickReplyKinds.Act, "say:safety");

        reply = Say(dialogue, reply.State, "Меня снова накрыло воспоминание, я будто снова там");

        Assert.Equal([CompanionTraumaContent.TooHardNow(false)], reply.Messages);
        Assert.Null(reply.State.Protocol);
    }

    [Fact]
    public void Crisis_words_in_the_trauma_work_open_help()
    {
        CompanionDialogue dialogue = Create();
        CompanionReply reply = Tap(dialogue, StartImpact(dialogue).State, ChatQuickReplyKinds.Rating, "3");

        reply = Say(dialogue, reply.State, "Не хочу жить после этого");

        Assert.Equal(DialogueActionKind.OpenCrisisHub, reply.Action?.Kind);
    }

    [Fact]
    public void The_trigger_map_is_three_questions_and_is_noted()
    {
        CompanionDialogue dialogue = Create();
        CompanionReply reply = Tap(dialogue, new CompanionState { TraumaAcknowledged = true }, ChatQuickReplyKinds.Act, "protocol:" + CompanionTraumaContent.Triggers);
        reply = Say(dialogue, reply.State, "В метро в час пик, много людей");
        reply = Say(dialogue, reply.State, "Сердце колотится, мысль что сейчас случится плохое");
        reply = Say(dialogue, reply.State, "Выйти на станцию раньше и подышать");

        TriggerNote note = Assert.Single(TraumaNotes.Deserialize(reply.State.TraumaNotes).Triggers);
        Assert.Equal("В метро в час пик, много людей", note.Situation);
        Assert.Contains("Выйти на станцию раньше", reply.Messages[0]);
    }

    [Fact]
    public void A_chat_opened_on_the_trauma_topic_starts_with_the_choices()
    {
        CompanionReply opened = Create().OpenTopic(new CompanionState(), CompanionTraumaContent.Topic);

        Assert.Equal([CompanionTraumaContent.TopicGreeting(false)], opened.Messages);
        Assert.Equal(CompanionTraumaContent.Choices(false), opened.QuickReplies);
    }

    [Fact]
    public void Notes_keep_the_latest_entries_and_survive_a_round_trip()
    {
        TraumaNotes notes = TraumaNotes.Empty;
        for (int i = 0; i < TraumaNotes.MaxEntries + 2; i++)
        {
            notes = notes.With(new StuckPointNote("trust", $"phrase {i}", "fairer", 8, 4, new DateTime(2026, 10, 17, 0, 0, 0, DateTimeKind.Utc)));
        }

        TraumaNotes read = TraumaNotes.Deserialize(notes.Serialize());

        Assert.Equal(TraumaNotes.MaxEntries, read.StuckPoints.Count);
        Assert.Equal("phrase 11", read.StuckPoints[^1].Phrase);
        Assert.Equal(TraumaNotes.Empty.StuckPoints, TraumaNotes.Deserialize("{ broken").StuckPoints);
    }
}
