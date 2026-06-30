using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using PsychologyApp.Presentation.Shared.Abstractions;

namespace PsychologyApp.Presentation.Shared.Platform;

public sealed class MauiTestCatalogProvider(ITestAssetReader assetReader, ILogger<MauiTestCatalogProvider> logger)
    : ITestCatalogProvider
{
    private const string BeckPath = "tests/beck.json";
    private const string LuscherPath = "tests/luscher.json";
    private const string QuestionnairesPath = "tests/questionnaires.json";

    public async Task<IReadOnlyList<TestDefinition>> LoadAllAsync(CancellationToken cancellationToken = default)
    {
        List<TestDefinition> items = [];

        await LoadBeckAsync(items, cancellationToken).ConfigureAwait(false);
        await LoadLuscherAsync(items, cancellationToken).ConfigureAwait(false);
        await LoadQuestionnairesAsync(items, cancellationToken).ConfigureAwait(false);

        if (items.Count == 0)
        {
            logger.LogError("Embedded test catalog is empty after loading all assets.");
            throw new InvalidOperationException("Embedded test catalog is empty.");
        }

        return items;
    }

    private async Task LoadBeckAsync(List<TestDefinition> items, CancellationToken cancellationToken)
    {
        ParseResult<JsonGroupedQuestionnaireDefinition>? parseResult =
            await DeserializeGroupedQuestionnaireAsync(BeckPath, cancellationToken)
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
            await DeserializeNavigationTestsAsync(LuscherPath, cancellationToken)
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
            await DeserializeSimpleQuestionnairesAsync(QuestionnairesPath, cancellationToken)
                .ConfigureAwait(false);

        if (parseResult is null || !parseResult.IsSuccess || parseResult.Value is null)
        {
            return;
        }

        items.AddRange(TestCatalogParser.ParseSimpleQuestionnaires(parseResult.Value));
    }

    private async Task<ParseResult<JsonGroupedQuestionnaireDefinition>?> DeserializeGroupedQuestionnaireAsync(
        string assetPath,
        CancellationToken cancellationToken)
    {
        try
        {
            await using Stream stream = await assetReader.OpenAsync(assetPath, cancellationToken).ConfigureAwait(false);
            ParseResult<JsonGroupedQuestionnaireDefinition> result =
                await TestCatalogParser.DeserializeGroupedQuestionnaireAsync(stream, cancellationToken).ConfigureAwait(false);

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

    private async Task<ParseResult<List<JsonNavigationTestDefinition>>?> DeserializeNavigationTestsAsync(
        string assetPath,
        CancellationToken cancellationToken)
    {
        try
        {
            await using Stream stream = await assetReader.OpenAsync(assetPath, cancellationToken).ConfigureAwait(false);
            ParseResult<List<JsonNavigationTestDefinition>> result =
                await TestCatalogParser.DeserializeNavigationTestsAsync(stream, cancellationToken).ConfigureAwait(false);

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

    private async Task<ParseResult<List<JsonSimpleQuestionnaireDefinition>>?> DeserializeSimpleQuestionnairesAsync(
        string assetPath,
        CancellationToken cancellationToken)
    {
        try
        {
            await using Stream stream = await assetReader.OpenAsync(assetPath, cancellationToken).ConfigureAwait(false);
            ParseResult<List<JsonSimpleQuestionnaireDefinition>> result =
                await TestCatalogParser.DeserializeSimpleQuestionnairesAsync(stream, cancellationToken).ConfigureAwait(false);

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
