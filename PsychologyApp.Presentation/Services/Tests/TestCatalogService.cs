using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Abstractions;
using PsychologyApp.Presentation.Models.Tests;

namespace PsychologyApp.Presentation.Services.Tests;

public sealed class TestCatalogService(ITestAssetReader assetReader, ILogger<TestCatalogService> logger)
{
    private const string BeckPath = "tests/beck.json";
    private const string LuscherPath = "tests/luscher.json";
    private const string QuestionnairesPath = "tests/questionnaires.json";

    public async Task<IReadOnlyList<TestDefinition>> LoadCatalogAsync(CancellationToken cancellationToken = default)
    {
        List<TestDefinition> items = [];

        await LoadBeckAsync(items, cancellationToken).ConfigureAwait(false);
        await LoadLuscherAsync(items, cancellationToken).ConfigureAwait(false);
        await LoadQuestionnairesAsync(items, cancellationToken).ConfigureAwait(false);

        return items;
    }

    private async Task LoadBeckAsync(List<TestDefinition> items, CancellationToken cancellationToken)
    {
        ParseResult<JsonGroupedQuestionnaireDefinition>? parseResult =
            await DeserializeAssetAsync<JsonGroupedQuestionnaireDefinition>(BeckPath, cancellationToken)
                .ConfigureAwait(false);

        if (parseResult is null)
        {
            return;
        }

        ParseResult<TestDefinition> definition = TestCatalogParser.ParseGroupedQuestionnaire(parseResult.Value!);
        if (definition.IsSuccess)
        {
            items.Add(definition.Value!);
        }
        else
        {
            logger.LogWarning("Skipped Beck test asset {AssetPath}: {Error}", BeckPath, definition.Error);
        }
    }

    private async Task LoadLuscherAsync(List<TestDefinition> items, CancellationToken cancellationToken)
    {
        ParseResult<List<JsonNavigationTestDefinition>>? parseResult =
            await DeserializeAssetAsync<List<JsonNavigationTestDefinition>>(LuscherPath, cancellationToken)
                .ConfigureAwait(false);

        if (parseResult is null || !parseResult.IsSuccess || parseResult.Value is null)
        {
            return;
        }

        items.AddRange(TestCatalogParser.ParseLuscherDefinitions(parseResult.Value));
    }

    private async Task LoadQuestionnairesAsync(List<TestDefinition> items, CancellationToken cancellationToken)
    {
        ParseResult<List<JsonSimpleQuestionnaireDefinition>>? parseResult =
            await DeserializeAssetAsync<List<JsonSimpleQuestionnaireDefinition>>(QuestionnairesPath, cancellationToken)
                .ConfigureAwait(false);

        if (parseResult is null || !parseResult.IsSuccess || parseResult.Value is null)
        {
            return;
        }

        items.AddRange(TestCatalogParser.ParseSimpleQuestionnaires(parseResult.Value));
    }

    private async Task<ParseResult<T>?> DeserializeAssetAsync<T>(string assetPath, CancellationToken cancellationToken)
    {
        try
        {
            await using Stream stream = await assetReader.OpenAsync(assetPath, cancellationToken).ConfigureAwait(false);
            ParseResult<T> result = await TestCatalogParser.DeserializeAsync<T>(stream, cancellationToken).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                logger.LogWarning("Failed to parse test asset {AssetPath}: {Error}", assetPath, result.Error);
                return null;
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load test asset {AssetPath}", assetPath);
            return null;
        }
    }
}
