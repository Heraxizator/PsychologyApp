using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
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

    public static TheoryData<string> ShippedScenarioFiles()
    {
        TheoryData<string> data = [];
        foreach (string path in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "conversations"), "*.json"))
        {
            data.Add(Path.GetFileName(path));
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(ShippedScenarioFiles))]
    public void Shipped_scenarios_are_valid_and_match_their_file_name(string fileName)
    {
        string json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "conversations", fileName));

        ParseResult<ConversationScenario> result = ConversationScenarioParser.Parse(json);

        Assert.True(result.IsSuccess, result.Error);
        Assert.Equal(fileName.Split('.')[0], result.Value!.Id);
    }

    [Theory]
    [InlineData(TechniqueId.Observer)]
    [InlineData(TechniqueId.Grounding)]
    public void Every_dialogue_technique_ships_russian_and_english_scenarios(TechniqueId id)
    {
        string dir = Path.Combine(AppContext.BaseDirectory, "conversations");

        Assert.True(File.Exists(Path.Combine(dir, $"{id}.json")));
        Assert.True(File.Exists(Path.Combine(dir, $"{id}.en.json")));
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
    [InlineData("I don't want to live anymore.")]
    [InlineData("I dont want to live")]
    [InlineData("I do not want to live like this")]
    [InlineData("I really don't want to keep living")]
    [InlineData("Не хочу уже совсем жить")]
    [InlineData("я так устала жить")]
    [InlineData("Не вижу смысла в жизни")]
    [InlineData("sometimes I wish I was dead")]
    [InlineData("I just want to disappear")]
    [InlineData("Иногда хочется исчезнуть")]
    [InlineData("Хочу уснуть и не проснуться")]
    public void Detects_crisis_phrases(string text) => Assert.True(_detector.IsCrisis(text));

    [Theory]
    [InlineData("Поссорился с коллегой, очень злюсь")]
    [InlineData("Не хочу идти на работу")]
    [InlineData("Хочу жить лучше и больше путешествовать")]
    [InlineData("I want to live in Paris")]
    [InlineData("I don't want to go to work today")]
    [InlineData("Не хочу больше слушать этот шум, хочу жить в тишине")]
    [InlineData("")]
    [InlineData(null)]
    public void Ignores_ordinary_text(string? text) => Assert.False(_detector.IsCrisis(text));
}

public class SituationAnalyzerTieBreakTests
{
    private readonly LexiconSituationAnalyzer _analyzer = new();

    [Fact]
    public void When_scores_tie_the_state_mentioned_first_wins()
    {
        Assert.Equal(CompanionEmotion.Anger, _analyzer.Analyze("My brother lied to me again and I'm furious. I can't stop thinking about it.").Emotion);
        Assert.Equal(CompanionEmotion.Overthinking, _analyzer.Analyze("I can't stop thinking about it and I'm furious.").Emotion);
        Assert.Equal(CompanionEmotion.Exhaustion, _analyzer.Analyze("Кажется, у меня выгорание. Ничего не радует.").Emotion);
    }
}
