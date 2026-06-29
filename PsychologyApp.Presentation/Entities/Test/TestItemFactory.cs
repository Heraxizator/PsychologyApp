using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Entities.Test;

public static class TestItemFactory
{
    public static TestItem Create(TestDefinition definition, INavigationService navigationService)
    {
        TestItem item = new()
        {
            TestId = definition.TestId,
            AnalyzerId = definition.AnalyzerId,
            Title = definition.Title,
            Subtitle = definition.Subtitle,
            Description = definition.Description,
            Comment = definition.Comment,
            Algorithm = definition.Algorithm.ToList(),
            MetaText = BuildMetaText(definition),
            StartAsync = () => TestStartOperations.StartAsync(definition, navigationService)
        };

        return item;
    }

    private static string? BuildMetaText(TestDefinition definition)
    {
        List<string> parts = [];

        if (definition.EstimatedMinutes is int minutes and > 0)
        {
            parts.Add(AppStrings.TestDuration(minutes));
        }

        if (definition.QuestionCount is int count and > 0)
        {
            parts.Add(AppStrings.TestQuestionCount(count));
        }

        if (!string.IsNullOrWhiteSpace(definition.Construct))
        {
            parts.Add(definition.Construct);
        }

        return parts.Count == 0 ? null : string.Join(" · ", parts);
    }
}
