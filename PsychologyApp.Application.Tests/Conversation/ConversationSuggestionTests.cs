using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Models.Tests;
using Xunit;

namespace PsychologyApp.Application.Tests.Conversation;

/// <summary>Text questions can offer chips, the emotion palette and the last problem, so a dialogue is answered with taps.</summary>
public class ConversationSuggestionTests
{
    private static ConversationScenario Load(string fileName)
    {
        ParseResult<ConversationScenario> parsed = ConversationScenarioParser.Parse(
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "conversations", fileName)));
        Assert.True(parsed.IsSuccess, parsed.Error);
        return parsed.Value!;
    }

    [Fact]
    public void A_text_question_carries_its_suggestions_and_flags_into_the_prompt()
    {
        const string json = """
        {
          "id": "T", "start": "a", "crisis": "c",
          "nodes": [
            { "id": "a", "kind": "askText", "text": ["hi"], "capture": "x", "next": "b",
              "suggestions": ["One", " Two ", ""], "emotions": true, "recent": true },
            { "id": "b", "kind": "end", "text": ["done {x}"] },
            { "id": "c", "kind": "end", "text": ["help"] }
          ]
        }
        """;
        ParseResult<ConversationScenario> parsed = ConversationScenarioParser.Parse(json);
        Assert.True(parsed.IsSuccess, parsed.Error);

        ConversationTurn first = new ConversationSession(parsed.Value!, new KeywordCrisisDetector()).Start();

        Assert.Equal(["One", "Two"], first.Prompt!.Suggestions);
        Assert.True(first.Prompt.OffersEmotions);
        Assert.True(first.Prompt.OffersRecent);
        Assert.Equal("x", first.Prompt.Capture);
    }

    [Theory]
    [InlineData("""{ "id": "a", "kind": "askRating", "text": ["rate"], "next": "b", "suggestions": ["x"] }""")]
    [InlineData("""{ "id": "a", "kind": "say", "text": ["hi"], "next": "b", "emotions": true }""")]
    [InlineData("""{ "id": "a", "kind": "askChoice", "text": ["pick"], "recent": true, "choices": [ { "label": "x", "next": "b" } ] }""")]
    public void Only_text_questions_may_offer_help_for_typing(string node)
    {
        string json = $$"""
        {
          "id": "T", "start": "a", "crisis": "c",
          "nodes": [
            {{node}},
            { "id": "b", "kind": "end", "text": ["done"] },
            { "id": "c", "kind": "end", "text": ["help"] }
          ]
        }
        """;

        ParseResult<ConversationScenario> result = ConversationScenarioParser.Parse(json);

        Assert.False(result.IsSuccess);
        Assert.Contains("only text questions", result.Error);
    }

    [Theory]
    [InlineData("Spin.json")]
    [InlineData("Spin.en.json")]
    public void Spin_offers_the_last_problem_then_the_emotion_palette_and_keeps_answers_as_form_fields(string file)
    {
        ConversationSession session = new(Load(file), new KeywordCrisisDetector(), new Random(1));

        ConversationTurn episode = session.Start();
        Assert.True(episode.Prompt!.OffersRecent);
        Assert.NotEmpty(episode.Prompt.Suggestions!);

        ConversationTurn feeling = session.SubmitText("Ссора с братом");
        Assert.True(feeling.Prompt!.OffersEmotions);

        ConversationTurn rating = session.SubmitText("Обида");
        Assert.Equal(ConversationInputKind.Rating, rating.Prompt!.Kind);

        ConversationTurn replay = session.SubmitRating(8);
        Assert.Equal(ConversationInputKind.Choice, replay.Prompt!.Kind);

        session.SubmitChoice(0);
        ConversationTurn end = session.SubmitRating(5);

        Assert.Equal(ConversationStatus.Completed, end.Status);
        Assert.Contains("8", end.BotMessages[^1]);
        Assert.Contains("5", end.BotMessages[^1]);
        Assert.Equal("Ссора с братом", session.Captured["f0"]);
        Assert.Equal("Обида", session.Captured["f1"]);
        Assert.Equal(8, session.GetRating("f2"));
    }

    [Fact]
    public void Spin_ends_early_when_the_charge_is_already_low()
    {
        ConversationSession session = new(Load("Spin.json"), new KeywordCrisisDetector(), new Random(1));
        session.Start();
        session.SubmitText("Разговор");
        session.SubmitText("Тревога");

        ConversationTurn end = session.SubmitRating(2);

        Assert.Equal(ConversationStatus.Completed, end.Status);
    }

    [Theory]
    [InlineData("Extend.json")]
    [InlineData("Extend.en.json")]
    public void Extend_collects_three_backups_with_chips_and_compares_the_anxiety(string file)
    {
        ConversationSession session = new(Load(file), new KeywordCrisisDetector(), new Random(1));

        session.Start();
        session.SubmitText("Не возьмут на работу");
        ConversationTurn backup = session.SubmitRating(7);
        Assert.NotEmpty(backup.Prompt!.Suggestions!);

        session.SubmitText("A");
        session.SubmitText("B");
        session.SubmitText("C");
        ConversationTurn end = session.SubmitRating(4);

        Assert.Equal(ConversationStatus.Completed, end.Status);
        Assert.Equal(["Не возьмут на работу", "A", "B", "C"], new[] { "f0", "f1", "f2", "f3" }.Select(k => session.Captured[k]));
    }

    [Fact]
    public void A_crisis_phrase_in_a_new_dialogue_stops_the_practice()
    {
        ConversationSession session = new(Load("Extend.json"), new KeywordCrisisDetector(), new Random(1));
        session.Start();

        ConversationTurn turn = session.SubmitText("не хочу жить");

        Assert.Equal(ConversationStatus.Interrupted, turn.Status);
    }
}
