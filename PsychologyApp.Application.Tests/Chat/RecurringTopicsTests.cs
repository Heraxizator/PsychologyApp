using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

/// <summary>Topics that keep coming back across chats: how they are counted, and when the companion names a repeat.</summary>
public sealed class RecurringTopicsTests
{
    private static readonly DateTime Now = new(2026, 10, 20, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void A_person_is_a_more_specific_topic_than_an_area_of_life()
    {
        Assert.Equal("person:Boss", RecurringTopics.Key(CompanionTheme.Work, CompanionPerson.Boss));
        Assert.Equal("theme:Work", RecurringTopics.Key(CompanionTheme.Work, null));
        Assert.Null(RecurringTopics.Key(null, null));
    }

    [Fact]
    public void Only_chats_of_the_last_month_count_and_a_damaged_date_is_skipped()
    {
        string stored = RecurringTopics.Record(RecurringTopics.Record(null, Now.AddDays(-40)), Now.AddDays(-5)) + ",not-a-date";
        stored = RecurringTopics.Record(stored, Now.AddDays(-1));

        IReadOnlyDictionary<string, int> counts = RecurringTopics.EarlierChats(
            new Dictionary<string, string> { [RecurringTopics.MemoryKey("person:Boss")] = stored, ["name"] = "Аня" }, Now);

        Assert.Equal(2, counts["person:Boss"]);
        Assert.Single(counts);
    }

    [Fact]
    public void The_memory_keeps_only_the_latest_dates()
    {
        string? stored = null;
        for (int day = 0; day < 20; day++)
        {
            stored = RecurringTopics.Record(stored, Now.AddDays(-day));
        }

        Assert.Equal(8, stored!.Split(',').Length);
    }

    private static CompanionReply Say(IReadOnlyDictionary<string, int> earlier, string text) =>
        new CompanionDialogue(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english: false, new Random(5), earlierTopicChats: earlier)
            .Respond(new CompanionState { Turns = 2 }, new CompanionInput.FreeText(text));

    [Fact]
    public void The_third_chat_on_a_topic_in_a_month_is_named_as_a_repeat_with_a_way_to_look_under_it()
    {
        CompanionReply reply = Say(new Dictionary<string, int> { ["person:Boss"] = 2 }, "Начальник опять накричал на меня при всех, мне очень обидно");

        Assert.Equal("person:Boss", reply.State.TopicCounted);
        Assert.True(reply.State.PatternNoted);
        Assert.Contains(reply.Messages, message => message.Contains("в 3-м нашем разговоре", StringComparison.Ordinal));
        Assert.Contains(reply.QuickReplies, chip => chip.Payload == "protocol:deeper");
    }

    [Fact]
    public void A_second_chat_on_a_topic_is_counted_but_not_named_yet()
    {
        CompanionReply reply = Say(new Dictionary<string, int> { ["person:Boss"] = 1 }, "Начальник опять накричал на меня при всех, мне очень обидно");

        Assert.Equal("person:Boss", reply.State.TopicCounted);
        Assert.False(reply.State.PatternNoted);
        Assert.DoesNotContain(reply.Messages, message => message.Contains("нашем разговоре", StringComparison.Ordinal));
    }

    [Fact]
    public void A_panic_attack_is_not_answered_with_an_observation_about_topics()
    {
        CompanionReply reply = Say(
            new Dictionary<string, int> { ["person:Boss"] = 5, ["theme:Work"] = 5 },
            "У меня паническая атака на работе после разговора с начальником, сердце колотится, задыхаюсь");

        Assert.False(reply.State.PatternNoted);
    }
}
