using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Models.Tests;
using Xunit;

namespace PsychologyApp.Application.Tests.Conversation;

public class ConversationScenarioParserTests
{
    private const string Valid = """
    {
      "id": "T", "start": "a", "crisis": "c",
      "nodes": [
        { "id": "a", "kind": "askText", "text": ["hi"], "capture": "x", "next": "b" },
        { "id": "b", "kind": "end", "text": ["done {x}"] },
        { "id": "c", "kind": "end", "text": ["help"] }
      ]
    }
    """;

    [Theory]
    [InlineData("ru")]
    [InlineData("en")]
    public void Shipped_observer_scenarios_are_valid(string language)
    {
        string json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "conversations", language == "ru" ? "Observer.json" : $"Observer.{language}.json"));

        ParseResult<ConversationScenario> result = ConversationScenarioParser.Parse(json);

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal("Observer", result.Value!.Id);
    }

    [Fact]
    public void Valid_scenario_parses()
    {
        ParseResult<ConversationScenario> result = ConversationScenarioParser.Parse(Valid);

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal(3, result.Value!.Nodes.Count);
    }

    [Fact]
    public void Malformed_json_fails_cleanly()
    {
        Assert.False(ConversationScenarioParser.Parse("{ nope").IsSuccess);
    }

    [Theory]
    [InlineData("\"next\": \"b\"", "\"next\": \"missing\"", "missing node")]
    [InlineData("\"id\": \"b\", \"kind\": \"end\"", "\"id\": \"a\", \"kind\": \"end\"", "Duplicate")]
    [InlineData("\"kind\": \"askText\"", "\"kind\": \"shout\"", "unknown kind")]
    [InlineData("done {x}", "done {y}", "placeholder")]
    [InlineData("\"capture\": \"x\", ", "", "placeholder")]
    public void Broken_scenarios_are_rejected_with_reason(string find, string replace, string expectedFragment)
    {
        ParseResult<ConversationScenario> result = ConversationScenarioParser.Parse(Valid.Replace(find, replace));

        Assert.False(result.IsSuccess);
        Assert.Contains(expectedFragment, result.Error);
    }

    [Fact]
    public void Say_loop_without_input_is_rejected()
    {
        const string json = """
        {
          "id": "T", "start": "a", "crisis": "c",
          "nodes": [
            { "id": "a", "kind": "say", "text": ["1"], "next": "b" },
            { "id": "b", "kind": "say", "text": ["2"], "next": "a" },
            { "id": "c", "kind": "end", "text": ["help"] }
          ]
        }
        """;

        ParseResult<ConversationScenario> result = ConversationScenarioParser.Parse(json);

        Assert.False(result.IsSuccess);
        Assert.Contains("loop", result.Error);
    }

    [Fact]
    public void Scenario_that_cannot_end_is_rejected()
    {
        const string json = """
        {
          "id": "T", "start": "a", "crisis": "c",
          "nodes": [
            { "id": "a", "kind": "askText", "text": ["1"], "next": "a" },
            { "id": "c", "kind": "end", "text": ["help"] }
          ]
        }
        """;

        Assert.False(ConversationScenarioParser.Parse(json).IsSuccess);
    }
}

public class KeywordCrisisDetectorTests
{
    private readonly KeywordCrisisDetector _detector = new();

    [Theory]
    [InlineData("Я хочу покончить с собой")]
    [InlineData("не хочу больше жить")]
    [InlineData("Мысли о СУИЦИДЕ")]
    [InlineData("i feel suicidal")]
    public void Detects_crisis_phrases(string text) => Assert.True(_detector.IsCrisis(text));

    [Theory]
    [InlineData("Поссорился с коллегой, очень злюсь")]
    [InlineData("Не хочу идти на работу")]
    [InlineData("")]
    [InlineData(null)]
    public void Ignores_ordinary_text(string? text) => Assert.False(_detector.IsCrisis(text));
}
