using System.Text.Json;
using PsychologyApp.Presentation.Models.Quotes;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Serialization;
using Xunit;

namespace PsychologyApp.Presentation.Tests.Serialization;

public sealed class AppJsonSerializerContextTests
{
    [Fact]
    public async Task QuotesRuAsset_DeserializesWithNonEmptyEntries()
    {
        await using FileStream stream = OpenAsset("quotes/quotes.ru.json");

        List<QuoteJsonEntry>? entries = await JsonSerializer.DeserializeAsync(
            stream,
            AppJsonSerializerContext.Default.ListQuoteJsonEntry);

        Assert.NotNull(entries);
        Assert.NotEmpty(entries);
        Assert.Contains(entries, entry => !string.IsNullOrWhiteSpace(entry.Text));
    }

    [Fact]
    public async Task BeckAsset_DeserializesGroupedQuestionnaire()
    {
        await using FileStream stream = OpenAsset("tests/beck.json");

        ParseResult<JsonGroupedQuestionnaireDefinition> result =
            await TestCatalogParser.DeserializeGroupedQuestionnaireAsync(stream);

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Value!.AnalyzerId));
        Assert.NotEmpty(result.Value.Questions);
    }

    [Fact]
    public async Task QuestionnairesAsset_DeserializesSimpleQuestionnaires()
    {
        await using FileStream stream = OpenAsset("tests/questionnaires.json");

        ParseResult<List<JsonSimpleQuestionnaireDefinition>> result =
            await TestCatalogParser.DeserializeSimpleQuestionnairesAsync(stream);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);
    }

    [Fact]
    public async Task LuscherAsset_DeserializesNavigationTests()
    {
        await using FileStream stream = OpenAsset("tests/luscher.json");

        ParseResult<List<JsonNavigationTestDefinition>> result =
            await TestCatalogParser.DeserializeNavigationTestsAsync(stream);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value);
    }

    private static FileStream OpenAsset(string relativePath)
    {
        string assetPath = Path.Combine(GetRawAssetsRoot(), relativePath.Replace('/', Path.DirectorySeparatorChar));
        Assert.True(File.Exists(assetPath), $"Missing test asset: {assetPath}");
        return File.OpenRead(assetPath);
    }

    private static string GetRawAssetsRoot()
    {
        string? current = AppContext.BaseDirectory;
        while (current is not null)
        {
            string candidate = Path.Combine(
                current,
                "PsychologyApp.Presentation",
                "Resources",
                "Raw");

            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new InvalidOperationException("Could not locate PsychologyApp.Presentation/Resources/Raw.");
    }
}
