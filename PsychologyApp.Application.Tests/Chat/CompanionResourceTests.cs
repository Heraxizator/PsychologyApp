using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Application.Models;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

public class CompanionResourceSuggesterTests
{
    private static readonly QuotSeed[] Catalog =
    [
        new("A", "Grief is love with nowhere to go.", "hope"),
        new("A", "Every ending carries a beginning.", "healing"),
        new("B", "Be gentle with the person you are becoming.", "self-love"),
        new("B", "Understanding another heals both.", "empathy"),
        new("C", "General wisdom for any day.", "general")
    ];

    [Theory]
    [InlineData(CompanionEmotion.Panic, CompanionResourceKind.Somatic)]
    [InlineData(CompanionEmotion.Anxiety, CompanionResourceKind.Somatic)]
    [InlineData(CompanionEmotion.Overthinking, CompanionResourceKind.Test)]
    [InlineData(CompanionEmotion.Exhaustion, CompanionResourceKind.Test)]
    [InlineData(CompanionEmotion.Sadness, CompanionResourceKind.Quote)]
    [InlineData(CompanionEmotion.Loneliness, CompanionResourceKind.Quote)]
    [InlineData(CompanionEmotion.Guilt, CompanionResourceKind.Quote)]
    [InlineData(CompanionEmotion.Resentment, CompanionResourceKind.Quote)]
    [InlineData(CompanionEmotion.Anger, CompanionResourceKind.Quote)]
    [InlineData(CompanionEmotion.Procrastination, CompanionResourceKind.Quote)]
    [InlineData(CompanionEmotion.Unknown, CompanionResourceKind.None)]
    public void Each_feeling_maps_to_a_fitting_resource(CompanionEmotion emotion, CompanionResourceKind expected) =>
        Assert.Equal(expected, CompanionResourceSuggester.Suggest(emotion, calming: false));

    [Fact]
    public void A_calming_moment_always_suggests_calming_audio_regardless_of_the_feeling()
    {
        Assert.Equal(CompanionResourceKind.Prayer, CompanionResourceSuggester.Suggest(CompanionEmotion.Anger, calming: true));
        Assert.Equal(CompanionResourceKind.Prayer, CompanionResourceSuggester.Suggest(CompanionEmotion.Unknown, calming: true));
    }

    [Fact]
    public void A_quote_is_picked_from_the_feelings_own_theme()
    {
        QuotSeed? quote = CompanionResourceSuggester.PickQuote(Catalog, CompanionEmotion.Sadness, new Random(1));

        Assert.NotNull(quote);
        Assert.Equal("hope", quote!.Theme);
    }

    [Fact]
    public void A_feeling_with_no_matching_theme_falls_back_to_a_general_quote()
    {
        QuotSeed[] noMatch = [new("A", "Only general here.", "general")];

        QuotSeed? quote = CompanionResourceSuggester.PickQuote(noMatch, CompanionEmotion.Sadness, new Random(1));

        Assert.Equal("general", quote!.Theme);
    }

    [Fact]
    public void An_empty_catalog_yields_no_quote()
    {
        Assert.Null(CompanionResourceSuggester.PickQuote([], CompanionEmotion.Sadness, new Random(1)));
    }
}

public class CompanionResourceDialogueTests
{
    private static readonly QuotSeed[] Catalog =
    [
        new("Rumi", "Grief is love with nowhere to go.", "hope"),
        new("Rumi", "Every ending carries a beginning.", "healing"),
        new("Author", "Be gentle with the person you are becoming.", "self-love")
    ];

    private static CompanionDialogue Create(
        bool english = false,
        IReadOnlyList<QuotSeed>? quotes = null,
        MoodEntryDTO? todayLowMood = null,
        TestResultDTO? recentStressTest = null) =>
        new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english, new Random(3),
            quotes: quotes ?? Catalog, todayLowMood: todayLowMood, recentStressTest: recentStressTest);

    private static CompanionReply Say(CompanionDialogue d, CompanionState state, string text) =>
        d.Respond(state, new CompanionInput.FreeText(text));

    private static CompanionReply Tap(CompanionDialogue d, CompanionState state, string payload) =>
        d.Respond(state, new CompanionInput.QuickReply(new ChatQuickReply(ChatQuickReplyKinds.Act, "tap", payload)));

    [Fact]
    public void An_offer_for_sadness_includes_a_matching_quote_and_a_chip_for_more()
    {
        CompanionDialogue d = Create();
        CompanionState state = new() { Turns = 2, Emotion = "Sadness", TurnsSinceOffer = 5, ScaleAsked = true };

        CompanionReply reply = Say(d, state, "Так грустно, что хочется плакать без причины");

        Assert.Contains(reply.Messages, m => m.Contains('«') && m.Contains('»'));
        Assert.Contains(reply.QuickReplies, c => c.Payload == "resource:quotes");
        Assert.Equal("quote", reply.State.OfferedResource);
    }

    [Fact]
    public void The_resource_is_offered_only_once_per_chat()
    {
        CompanionDialogue d = Create();
        CompanionState state = new() { Turns = 2, Emotion = "Sadness", TurnsSinceOffer = 5, ScaleAsked = true, OfferedResource = "quote" };

        CompanionReply reply = Say(d, state, "Так грустно, что хочется плакать без причины");

        Assert.DoesNotContain(reply.QuickReplies, c => c.Payload == "resource:quotes");
    }

    [Fact]
    public void Tapping_the_somatic_chip_returns_the_open_somatic_action()
    {
        CompanionDialogue d = Create();

        CompanionReply reply = Tap(d, new CompanionState { Emotion = "Anxiety" }, "resource:somatic");

        Assert.Equal(DialogueActionKind.OpenSomatic, reply.Action?.Kind);
        Assert.Single(reply.Messages);
    }

    [Fact]
    public void Tapping_the_test_chip_returns_the_open_tests_action()
    {
        CompanionDialogue d = Create();

        CompanionReply reply = Tap(d, new CompanionState { Emotion = "Overthinking" }, "resource:test");

        Assert.Equal(DialogueActionKind.OpenTests, reply.Action?.Kind);
    }

    [Fact]
    public void Tapping_the_prayer_chip_returns_the_open_prayers_action()
    {
        CompanionDialogue d = Create();

        CompanionReply reply = Tap(d, new CompanionState { Emotion = "Panic" }, "resource:prayer");

        Assert.Equal(DialogueActionKind.OpenPrayers, reply.Action?.Kind);
    }

    [Fact]
    public void Tapping_more_quotes_returns_the_open_quotes_action()
    {
        CompanionDialogue d = Create();

        CompanionReply reply = Tap(d, new CompanionState { Emotion = "Sadness" }, "resource:quotes");

        Assert.Equal(DialogueActionKind.OpenQuotes, reply.Action?.Kind);
    }

    [Fact]
    public void A_high_tension_calming_offer_suggests_audio_instead_of_the_emotions_usual_resource()
    {
        CompanionDialogue d = Create();
        CompanionState state = new() { Turns = 3, Emotion = "Anger", ScaleAsked = true };

        CompanionReply reply = d.Respond(state, new CompanionInput.QuickReply(new ChatQuickReply(ChatQuickReplyKinds.Rating, "9", "9")));

        Assert.Contains(reply.QuickReplies, c => c.Payload == "resource:prayer");
    }

    [Fact]
    public void No_resource_is_suggested_before_a_feeling_is_known()
    {
        CompanionDialogue d = Create();
        CompanionState state = new() { Turns = 3, TurnsSinceOffer = 5 };

        CompanionReply reply = Say(d, state, "Даже не знаю, что сказать");

        Assert.DoesNotContain(reply.QuickReplies, c => c.Payload is not null && c.Payload.StartsWith("resource:"));
    }

    [Fact]
    public void With_an_empty_catalog_the_offer_still_works_without_a_quote()
    {
        CompanionDialogue d = Create(quotes: []);
        CompanionState state = new() { Turns = 2, Emotion = "Sadness", TurnsSinceOffer = 5, ScaleAsked = true };

        CompanionReply reply = Say(d, state, "Так грустно, что хочется плакать без причины");

        Assert.Null(reply.State.OfferedResource);
        Assert.NotEmpty(reply.Messages);
    }
}

public class CompanionJournalTests
{
    private static CompanionDialogue Create(MoodEntryDTO? todayLowMood = null, bool english = false) =>
        new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english, new Random(3), todayLowMood: todayLowMood);

    private static CompanionReply Say(CompanionDialogue d, CompanionState state, string text) =>
        d.Respond(state, new CompanionInput.FreeText(text));

    [Fact]
    public void Opening_with_a_low_mood_already_logged_skips_the_ordinary_greeting()
    {
        MoodEntryDTO mood = new() { MoodLevel = 2, RecordedAt = DateTime.UtcNow };
        CompanionReply reply = Create(mood).Open(new CompanionState(), previous: null);

        Assert.Single(reply.Messages);
        Assert.Contains("дневник", reply.Messages[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void A_note_left_in_the_journal_is_quoted_back()
    {
        MoodEntryDTO mood = new() { MoodLevel = 1, Note = "поругалась с сестрой", RecordedAt = DateTime.UtcNow };
        CompanionReply reply = Create(mood).Open(new CompanionState(), previous: null);

        Assert.Contains("поругалась с сестрой", reply.Messages[0]);
    }

    [Fact]
    public void No_journal_entry_means_the_ordinary_greeting_is_unchanged()
    {
        CompanionReply reply = Create().Open(new CompanionState(), previous: null);

        Assert.Equal(2, reply.Messages.Count);
    }

    [Fact]
    public void Saying_thanks_offers_to_log_the_journal_once_the_feeling_and_tension_are_known()
    {
        CompanionReply reply = Say(Create(), new CompanionState { Turns = 3, Emotion = "Anxiety", LastIntensity = 6 }, "Спасибо");

        Assert.Contains(reply.QuickReplies, c => c.Payload == "journal:log");
    }

    [Fact]
    public void The_journal_chip_is_not_offered_without_a_known_feeling()
    {
        CompanionReply reply = Say(Create(), new CompanionState { Turns = 3 }, "Спасибо");

        Assert.DoesNotContain(reply.QuickReplies, c => c.Payload == "journal:log");
    }

    [Fact]
    public void The_journal_chip_is_not_offered_twice_in_the_same_chat()
    {
        CompanionReply reply = Say(
            Create(),
            new CompanionState { Turns = 3, Emotion = "Anxiety", LastIntensity = 6, JournalLogged = true },
            "Спасибо");

        Assert.DoesNotContain(reply.QuickReplies, c => c.Payload == "journal:log");
    }

    [Fact]
    public void Tapping_the_journal_chip_logs_a_mood_matched_to_the_tension_and_marks_the_chat_as_logged()
    {
        CompanionDialogue d = Create();
        CompanionState state = new() { Emotion = "Sadness", LastIntensity = 8 };

        CompanionReply reply = d.Respond(state, new CompanionInput.QuickReply(new ChatQuickReply(ChatQuickReplyKinds.Act, "Записать в дневник", "journal:log")));

        Assert.Equal(DialogueActionKind.LogMood, reply.Action?.Kind);
        Assert.Equal(2, reply.Action!.MoodLevel); // 8/10 tension -> a hard day, near the bottom of the 1..5 scale
        Assert.Contains("грусть", reply.Action.Note, StringComparison.OrdinalIgnoreCase);
        Assert.True(reply.State.JournalLogged);
        Assert.Single(reply.Messages);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(5, 3)]
    [InlineData(10, 1)]
    public void Tension_converts_to_the_journals_1_to_5_mood_scale(int tension, int expectedMood) =>
        Assert.Equal(expectedMood, CompanionResourceContent.IntensityToMoodLevel(tension));

    [Fact]
    public void A_missing_tension_lands_in_the_middle_of_the_mood_scale() =>
        Assert.Equal(3, CompanionResourceContent.IntensityToMoodLevel(null));
}

public class CompanionTestReferenceTests
{
    private static CompanionDialogue Create(TestResultDTO? recentStressTest) =>
        new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english: false, random: new Random(3), recentStressTest: recentStressTest);

    private static CompanionReply Tap(CompanionDialogue d, CompanionState state, string payload) =>
        d.Respond(state, new CompanionInput.QuickReply(new ChatQuickReply(ChatQuickReplyKinds.Act, "tap", payload)));

    [Fact]
    public void An_existing_stress_result_is_referenced_instead_of_suggesting_a_fresh_test()
    {
        TestResultDTO result = new() { TestId = "pss10", Summary = "Умеренный стресс", CompletedAt = DateTime.UtcNow.AddDays(-3) };
        CompanionDialogue d = Create(result);
        CompanionState state = new() { Turns = 2, Emotion = "Overthinking", TurnsSinceOffer = 5, ScaleAsked = true };

        CompanionReply reply = d.Respond(state, new CompanionInput.FreeText("Мысли крутятся по кругу и не дают покоя весь день"));

        Assert.Contains(reply.Messages, m => m.Contains("Умеренный стресс"));
        Assert.Contains(reply.QuickReplies, c => c.Payload == "resource:test-history");
        Assert.DoesNotContain(reply.QuickReplies, c => c.Payload == "resource:test");
    }

    [Fact]
    public void Tapping_the_test_history_chip_opens_that_specific_tests_history()
    {
        CompanionDialogue d = Create(recentStressTest: null);

        CompanionReply reply = Tap(d, new CompanionState { Emotion = "Overthinking" }, "resource:test-history");

        Assert.Equal(DialogueActionKind.OpenTestHistory, reply.Action?.Kind);
        Assert.Equal(CompanionResourceContent.StressTestId, reply.Action.TestId);
    }
}
