using PsychologyApp.Presentation.Models.Tests;
using Xunit;

namespace PsychologyApp.Presentation.Tests.Catalog;

public sealed class TestCatalogParserTests
{
    [Fact]
    public async Task DeserializeAsync_ValidJson_ReturnsValue()
    {
        await using MemoryStream stream = new(System.Text.Encoding.UTF8.GetBytes("""{"title":"T","analyzerId":"beck"}"""));

        ParseResult<JsonGroupedQuestionnaireDefinition> result =
            await TestCatalogParser.DeserializeAsync<JsonGroupedQuestionnaireDefinition>(stream);

        Assert.True(result.IsSuccess);
        Assert.Equal("beck", result.Value!.AnalyzerId);
    }

    [Fact]
    public async Task DeserializeAsync_InvalidJson_ReturnsFailure()
    {
        await using MemoryStream stream = new(System.Text.Encoding.UTF8.GetBytes("{not json"));

        ParseResult<JsonGroupedQuestionnaireDefinition> result =
            await TestCatalogParser.DeserializeAsync<JsonGroupedQuestionnaireDefinition>(stream);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void ParseLuscherDefinitions_MapsStandardAndBriefTargets()
    {
        IReadOnlyList<TestDefinition> items = TestCatalogParser.ParseLuscherDefinitions(
        [
            new JsonNavigationTestDefinition("Brief", "Sub", "Desc", ["Step"], "Note", "brief"),
            new JsonNavigationTestDefinition("Standard", "Sub", "Desc", ["Step"], "Note", "standard"),
            new JsonNavigationTestDefinition("Legacy", "Sub", "Desc", ["Step"], "Note", "StandardTestPage")
        ]);

        Assert.Equal(3, items.Count);
        Assert.Equal(TestIds.LuscherBrief, items[0].TestId);
        Assert.Equal(LuscherMode.Brief, items[0].LuscherMode);
        Assert.Equal(TestIds.LuscherStandard, items[1].TestId);
        Assert.Equal(TestIds.LuscherStandard, items[2].TestId);
    }

    [Fact]
    public void ParseSimpleQuestionnaires_SkipsUnknownAnalyzer()
    {
        IReadOnlyList<TestDefinition> items = TestCatalogParser.ParseSimpleQuestionnaires(
        [
            new JsonSimpleQuestionnaireDefinition(
                "Unknown",
                "Sub",
                "Desc",
                ["Step"],
                "Note",
                "unknown_analyzer",
                ["Yes", "No"],
                [1, 0],
                ["Q1"],
                true)
        ]);

        Assert.Empty(items);
    }

    [Fact]
    public void ParseGroupedQuestionnaire_BuildsQuestionsForKnownAnalyzer()
    {
        ParseResult<TestDefinition> result = TestCatalogParser.ParseGroupedQuestionnaire(
            new JsonGroupedQuestionnaireDefinition(
                "Beck",
                "Sub",
                "Desc",
                ["Step"],
                "Note",
                "beck",
                true,
                [new JsonGroupedQuestionDefinition([new JsonAnswerDefinition(1, "A"), new JsonAnswerDefinition(0, "B")])]));

        Assert.True(result.IsSuccess);
        Assert.Equal("beck", result.Value!.TestId);
        Assert.Single(result.Value.Questions!);
        Assert.Equal(2, result.Value.Questions![0].Answers.Count);
    }
}
