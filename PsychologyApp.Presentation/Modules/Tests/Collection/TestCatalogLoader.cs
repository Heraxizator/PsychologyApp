using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Tests;
using PsychologyApp.Presentation.Services;
using System.Text.Json;

namespace PsychologyApp.Presentation.Modules.Tests.Collection;

public static class TestCatalogLoader
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<IReadOnlyList<TestItem>> LoadAllAsync(INavigationService navigationService)
    {
        List<TestItem> items = [];

        TestItem? beck = await LoadBeckTestAsync(navigationService).ConfigureAwait(false);
        if (beck is not null)
        {
            items.Add(beck);
        }

        items.AddRange(await LoadLuscherTestsAsync(navigationService).ConfigureAwait(false));
        items.AddRange(await LoadSimpleQuestionnairesAsync(navigationService).ConfigureAwait(false));

        return items;
    }

    public static async Task<TestItem?> LoadBeckTestAsync(INavigationService navigationService) =>
        await LoadGroupedQuestionnaireAsync(navigationService, "tests/beck.json").ConfigureAwait(false);

    public static async Task<IReadOnlyList<TestItem>> LoadLuscherTestsAsync(INavigationService navigationService)
    {
        List<JsonNavigationTestDefinition>? definitions =
            await DeserializePackageAsync<List<JsonNavigationTestDefinition>>("tests/luscher.json")
                .ConfigureAwait(false);

        if (definitions is null || definitions.Count == 0)
        {
            return [];
        }

        return definitions.Select(def => new TestItem
        {
            Title = def.Title,
            Subtitle = def.Subtitle,
            Description = def.Description,
            Algorithm = def.Algorithm,
            Comment = def.Comment,
            Action = () => NavigateToTargetAsync(navigationService, def.NavigationTarget).FireAndForget()
        }).ToList();
    }

    public static async Task<IReadOnlyList<TestItem>> LoadSimpleQuestionnairesAsync(INavigationService navigationService)
    {
        List<JsonSimpleQuestionnaireDefinition>? definitions =
            await DeserializePackageAsync<List<JsonSimpleQuestionnaireDefinition>>("tests/questionnaires.json")
                .ConfigureAwait(false);

        if (definitions is null || definitions.Count == 0)
        {
            return [];
        }

        List<TestItem> items = [];

        foreach (JsonSimpleQuestionnaireDefinition definition in definitions)
        {
            Func<int, string>? analyzer = TestScoreAnalyzers.Resolve(definition.AnalyzerId);
            if (analyzer is null)
            {
                continue;
            }

            items.Add(
                TestItem.CreateBuilder()
                    .SetTitle(definition.Title)
                    .SetSubtitle(definition.Subtitle)
                    .SetDescription(definition.Description)
                    .SetAlgorithm(definition.Algorithm)
                    .ConfigureYesNoQuestionnaire(
                        navigationService,
                        analyzer,
                        definition.Answers,
                        definition.Balls,
                        definition.Questions,
                        definition.SingleAnswer)
                    .SetComment(definition.Comment)
                    .Build());
        }

        return items;
    }

    private static async Task<TestItem?> LoadGroupedQuestionnaireAsync(INavigationService navigationService, string assetPath)
    {
        JsonGroupedQuestionnaireDefinition? definition =
            await DeserializePackageAsync<JsonGroupedQuestionnaireDefinition>(assetPath).ConfigureAwait(false);

        if (definition is null)
        {
            return null;
        }

        Func<int, string>? analyzer = TestScoreAnalyzers.Resolve(definition.AnalyzerId);
        if (analyzer is null)
        {
            return null;
        }

        List<Question> questions = definition.Questions
            .Select((group, index) => new Question
            {
                Number = index + 1,
                Answers = group.Answers
                    .Select(answer => new Answer
                    {
                        Ball = answer.Ball,
                        Text = answer.Text,
                        Selected = false
                    })
                    .ToList()
            })
            .ToList();

        return TestItem.CreateBuilder()
            .SetTitle(definition.Title)
            .SetSubtitle(definition.Subtitle)
            .SetDescription(definition.Description)
            .SetAlgorithm(definition.Algorithm)
            .SetActionQuestions(navigationService, analyzer, questions, definition.SingleAnswer)
            .SetComment(definition.Comment)
            .Build();
    }

    private static async Task<T?> DeserializePackageAsync<T>(string assetPath)
    {
        string localizedPath = ContentAssets.Localized(assetPath);
        try
        {
            await using Stream stream = await OpenAssetWithFallbackAsync(localizedPath, assetPath);
            return await JsonSerializer.DeserializeAsync<T>(stream, SerializerOptions).ConfigureAwait(false);
        }
        catch
        {
            return default;
        }
    }

    private static async Task<Stream> OpenAssetWithFallbackAsync(string primaryPath, string fallbackPath)
    {
        try
        {
            return await FileSystem.OpenAppPackageFileAsync(primaryPath);
        }
        catch when (!string.Equals(primaryPath, fallbackPath, StringComparison.Ordinal))
        {
            return await FileSystem.OpenAppPackageFileAsync(fallbackPath);
        }
    }

    private static Task NavigateToTargetAsync(INavigationService navigationService, string target) => target switch
    {
        "StandardTestPage" => navigationService.GoToStandardTestAsync(),
        "AlternativeTestPage" => navigationService.GoToAlternativeTestAsync(),
        _ => navigationService.GoToAlternativeTestAsync()
    };
}

