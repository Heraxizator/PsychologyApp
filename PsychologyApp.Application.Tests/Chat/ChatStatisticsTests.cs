using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

public class ChatStatisticsTests
{
    private static readonly DateTime Now = new(2026, 9, 21, 12, 0, 0, DateTimeKind.Utc);
    private static readonly IReadOnlyDictionary<string, string> NoMemory = new Dictionary<string, string>();

    private static ChatSessionDTO Chat(int daysAgo, string? emotion = null, int? first = null, int? last = null, int userMessages = 3, int turns = 2) => new()
    {
        Id = daysAgo * 10 + (emotion?.Length ?? 0),
        Title = "t",
        CreatedAt = Now.AddDays(-daysAgo),
        UpdatedAt = Now.AddDays(-daysAgo),
        Emotion = emotion,
        FirstIntensity = first,
        LastIntensity = last,
        UserMessageCount = userMessages,
        MessageCount = userMessages * 2,
        StateJson = new CompanionState { Turns = turns }.Serialize()
    };

    private static CompanionProfile Compute(IReadOnlyList<ChatSessionDTO> chats, IReadOnlyDictionary<string, string>? memory = null) =>
        ChatStatistics.Compute(chats, memory ?? NoMemory, Now, TimeZoneInfo.Utc);

    [Fact]
    public void No_chats_gives_an_empty_profile_with_an_invitation()
    {
        CompanionProfile profile = Compute([]);

        Assert.False(profile.HasHistory);
        Assert.Equal(0, profile.Chats);
        Assert.Null(profile.SinceUtc);
        Assert.Single(ChatProfileContent.Insights(profile, english: false));
        Assert.Contains("Напишите", ChatProfileContent.Insights(profile, english: false)[0]);
    }

    [Fact]
    public void Chats_where_nothing_was_said_do_not_count()
    {
        CompanionProfile profile = Compute([Chat(0, turns: 0), Chat(1)]);

        Assert.Equal(1, profile.Chats);
    }

    [Fact]
    public void Messages_days_and_the_first_date_are_summed_up()
    {
        CompanionProfile profile = Compute([Chat(0, userMessages: 4), Chat(3, userMessages: 6), Chat(3, userMessages: 1)]);

        Assert.Equal(3, profile.Chats);
        Assert.Equal(11, profile.UserMessages);
        Assert.Equal(2, profile.Days);
        Assert.Equal(Now.AddDays(-3), profile.SinceUtc);
    }

    [Theory]
    [InlineData(new[] { 0, 1, 2 }, 3)]
    [InlineData(new[] { 1, 2 }, 2)]
    [InlineData(new[] { 0, 2 }, 1)]
    [InlineData(new[] { 2, 3 }, 0)]
    public void The_streak_counts_consecutive_days_up_to_today_or_yesterday(int[] daysAgo, int expected)
    {
        CompanionProfile profile = Compute(daysAgo.Select(d => Chat(d)).ToList());

        Assert.Equal(expected, profile.StreakDays);
    }

    [Fact]
    public void The_most_frequent_feelings_come_first_with_their_share()
    {
        CompanionProfile profile = Compute(
        [
            Chat(1, "Anxiety"), Chat(2, "Anxiety"), Chat(3, "Anxiety"), Chat(4, "Anger"), Chat(5, "Unknown"), Chat(6)
        ]);

        Assert.Equal(CompanionEmotion.Anxiety, profile.Emotions[0].Emotion);
        Assert.Equal(3, profile.Emotions[0].Chats);
        Assert.Equal(0.75, profile.Emotions[0].Share);
        Assert.Equal(2, profile.Emotions.Count);
    }

    [Fact]
    public void Tension_is_summarised_over_the_chats_where_it_was_measured_twice()
    {
        CompanionProfile profile = Compute(
        [
            Chat(1, first: 8, last: 4), Chat(2, first: 6, last: 6), Chat(3, first: 5, last: 2), Chat(4, first: 7), Chat(5)
        ]);

        Assert.Equal(3, profile.TensionMeasured);
        Assert.Equal(2, profile.TensionImproved);
        Assert.Equal((4 + 0 + 3) / 3.0, profile.AverageDrop!.Value, 3);
        Assert.Equal([2, 6, 4], profile.Tension.Select(t => t.After));
        Assert.Contains("2 из 3 разговоров", ChatProfileContent.Insights(profile, english: false)[0]);
    }

    [Fact]
    public void Only_the_latest_tension_points_are_kept_oldest_first()
    {
        CompanionProfile profile = Compute(Enumerable.Range(1, 12).Select(d => Chat(d, "Anger" + d, first: 5, last: d % 10)).ToList());

        Assert.Equal(ChatStatistics.MaxTensionPoints, profile.Tension.Count);
        Assert.True(profile.Tension.SequenceEqual(profile.Tension.OrderBy(t => t.AtUtc)));
    }

    [Fact]
    public void Practices_are_ordered_by_how_often_they_helped_and_carry_both_counters()
    {
        Dictionary<string, string> memory = new()
        {
            [ChatMemoryKeys.Tried(TechniqueId.Breathing)] = "5",
            [ChatMemoryKeys.Helped(TechniqueId.Breathing)] = "1",
            [ChatMemoryKeys.Tried(TechniqueId.Grounding)] = "2",
            [ChatMemoryKeys.Helped(TechniqueId.Grounding)] = "2",
            ["junk"] = "7",
            [ChatMemoryKeys.HelpedPrefix + "NoSuchTechnique"] = "9",
            [ChatMemoryKeys.Name] = "  "
        };

        CompanionProfile profile = Compute([Chat(0)], memory);

        Assert.Equal([TechniqueId.Grounding, TechniqueId.Breathing], profile.Practices.Select(p => p.Technique));
        Assert.Equal(7, profile.PracticesTried);
        Assert.Equal(3, profile.PracticesHelped);
        Assert.Null(profile.UserName);
        Assert.Equal(TechniqueId.Grounding, ChatMemoryKeys.MostHelped(memory));
    }

    [Fact]
    public void A_tie_between_helpful_practices_is_settled_by_name_not_by_chance()
    {
        Dictionary<string, string> memory = new()
        {
            [ChatMemoryKeys.Helped(TechniqueId.Grounding)] = "1",
            [ChatMemoryKeys.Helped(TechniqueId.Breathing)] = "1"
        };

        Assert.Equal(TechniqueId.Breathing, ChatMemoryKeys.MostHelped(memory));
        Assert.Null(ChatMemoryKeys.MostHelped(NoMemory));
    }

    [Theory]
    [InlineData(0, 0, 0.0, 5)]
    [InlineData(4, 0, 0.8, 1)]
    [InlineData(5, 1, 0.0, 20)]
    [InlineData(25, 2, 0.0, 55)]
    [InlineData(80, 3, 1.0, 0)]
    [InlineData(500, 3, 1.0, 0)]
    public void Trust_grows_with_the_number_of_messages(int messages, int level, double progress, int toNext)
    {
        TrustLevel trust = ChatStatistics.Trust(messages);

        Assert.Equal(level, trust.Index);
        Assert.Equal(progress, trust.Progress, 3);
        Assert.Equal(toNext, trust.MessagesToNext);
    }

    [Fact]
    public void Every_trust_level_has_a_name_in_both_languages()
    {
        for (int level = 0; level < ChatProfileContent.TrustLevels; level++)
        {
            Assert.False(string.IsNullOrWhiteSpace(ChatProfileContent.TrustName(level, english: false)));
            Assert.False(string.IsNullOrWhiteSpace(ChatProfileContent.TrustName(level, english: true)));
        }
    }

    [Theory]
    [InlineData(1, "1 сообщение")]
    [InlineData(2, "2 сообщения")]
    [InlineData(5, "5 сообщений")]
    [InlineData(11, "11 сообщений")]
    [InlineData(21, "21 сообщение")]
    public void The_hint_uses_correct_russian_plurals(int left, string expected)
    {
        string hint = ChatProfileContent.TrustHint(new TrustLevel(0, 0, left), english: false);

        Assert.Contains(expected, hint);
    }

    [Fact]
    public void Insights_say_only_what_is_true_for_the_person()
    {
        Dictionary<string, string> memory = new() { [ChatMemoryKeys.Helped(TechniqueId.Breathing)] = "2" };
        CompanionProfile profile = Compute([Chat(0, "Anxiety", 8, 3), Chat(1, "Anxiety", 7, 4), Chat(2, "Anxiety", 6, 5)], memory);

        IReadOnlyList<string> insights = ChatProfileContent.Insights(profile, english: false);

        Assert.Contains(insights, i => i.Contains("3 из 3 разговоров"));
        Assert.Contains(insights, i => i.Contains("сработала 2 раза"));
        Assert.Contains(insights, i => i.Contains("100% разговоров"));
        Assert.Contains(insights, i => i.Contains("3 дня подряд"));
    }

    [Fact]
    public void The_tension_caption_is_honest_when_it_rose()
    {
        CompanionProfile profile = Compute([Chat(0, first: 3, last: 6), Chat(1, first: 4, last: 5)]);

        string caption = ChatProfileContent.TensionCaption(profile, english: false);

        Assert.Contains("росло", caption);
        Assert.Contains("всё равно важно", caption);
    }

    [Fact]
    public void English_texts_are_produced_for_a_full_profile()
    {
        Dictionary<string, string> memory = new() { [ChatMemoryKeys.Helped(TechniqueId.Breathing)] = "1" };
        CompanionProfile profile = Compute([Chat(0, "Anxiety", 8, 3), Chat(1, "Anxiety", 7, 4), Chat(2, "Anxiety", 6, 5)], memory);

        Assert.All(ChatProfileContent.Insights(profile, english: true), line => Assert.DoesNotMatch("[А-Яа-я]", line));
        Assert.DoesNotMatch("[А-Яа-я]", ChatProfileContent.TensionCaption(profile, english: true));
        Assert.DoesNotMatch("[А-Яа-я]", ChatProfileContent.Since(profile.SinceUtc, english: true));
        Assert.All(ChatProfileContent.Abilities(english: true), a => Assert.DoesNotMatch("[А-Яа-я]", a));
    }
}
