using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using Xunit;

namespace PsychologyApp.Presentation.Tests.Catalog;

public sealed class TestCatalogParserTests
{
    [Fact]
    public async Task DeserializeAsync_ValidJson_ReturnsValue()
    {
        await using MemoryStream stream = new(System.Text.Encoding.UTF8.GetBytes("""{"title":"T","analyzerId":"beck"}"""));

        ParseResult<JsonGroupedQuestionnaireDefinition> result =
            await TestCatalogParser.DeserializeGroupedQuestionnaireAsync(stream);

        Assert.True(result.IsSuccess);
        Assert.Equal("beck", result.Value!.AnalyzerId);
    }

    [Fact]
    public async Task DeserializeAsync_InvalidJson_ReturnsFailure()
    {
        await using MemoryStream stream = new(System.Text.Encoding.UTF8.GetBytes("{not json"));

        ParseResult<JsonGroupedQuestionnaireDefinition> result =
            await TestCatalogParser.DeserializeGroupedQuestionnaireAsync(stream);

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
    public void ParseSimpleQuestionnaires_IncludesRegisteredGad7Analyzer()
    {
        IReadOnlyList<TestDefinition> items = TestCatalogParser.ParseSimpleQuestionnaires(
        [
            new JsonSimpleQuestionnaireDefinition(
                "GAD-7",
                "Anxiety",
                "Desc",
                ["Step"],
                "Note",
                "gad7",
                ["Not at all", "Several days", "More than half", "Nearly every day"],
                [0, 1, 2, 3],
                ["Q1", "Q2", "Q3", "Q4", "Q5", "Q6", "Q7"],
                true,
                2,
                7,
                "Anxiety")
        ]);

        Assert.Single(items);
        Assert.Equal("gad7", items[0].TestId);
        Assert.Equal(7, items[0].QuestionCount);
        Assert.Equal(7, items[0].Questions!.Count);
    }

    [Fact]
    public void ParseSimpleQuestionnaires_IncludesRegisteredPhq9Analyzer()
    {
        IReadOnlyList<TestDefinition> items = TestCatalogParser.ParseSimpleQuestionnaires(
        [
            new JsonSimpleQuestionnaireDefinition(
                "PHQ-9",
                "Depression",
                "Desc",
                ["Step"],
                "Note",
                "phq9",
                ["Not at all", "Several days", "More than half", "Nearly every day"],
                [0, 1, 2, 3],
                ["Q1", "Q2", "Q3", "Q4", "Q5", "Q6", "Q7", "Q8", "Q9"],
                true,
                3,
                9,
                "Depression")
        ]);

        Assert.Single(items);
        Assert.Equal("phq9", items[0].TestId);
        Assert.Equal(9, items[0].QuestionCount);
        Assert.Equal(9, items[0].Questions!.Count);
    }

    [Fact]
    public void ParseSimpleQuestionnaires_IncludesRegisteredPhq15Analyzer()
    {
        IReadOnlyList<TestDefinition> items = TestCatalogParser.ParseSimpleQuestionnaires(
        [
            new JsonSimpleQuestionnaireDefinition(
                "PHQ-15",
                "Somatic",
                "Desc",
                ["Step"],
                "Note",
                "phq15",
                ["Not at all", "A little", "A lot"],
                [0, 1, 2],
                ["Q1", "Q2", "Q3", "Q4", "Q5", "Q6", "Q7", "Q8", "Q9", "Q10", "Q11", "Q12", "Q13", "Q14", "Q15"],
                true,
                4,
                15,
                "Somatic symptoms")
        ]);

        Assert.Single(items);
        Assert.Equal("phq15", items[0].TestId);
        Assert.Equal(15, items[0].QuestionCount);
        Assert.Equal(15, items[0].Questions!.Count);
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
