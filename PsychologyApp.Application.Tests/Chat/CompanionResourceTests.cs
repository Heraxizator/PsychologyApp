using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
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

    private static CompanionDialogue Create(bool english = false, IReadOnlyList<QuotSeed>? quotes = null) =>
        new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english, new Random(3), quotes: quotes ?? Catalog);

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
