using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;

namespace PsychologyApp.Presentation.Models.Tests;

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
            Action = CreateStartAction(definition, navigationService)
        };

        return item;
    }

    private static Action CreateStartAction(TestDefinition definition, INavigationService navigationService) =>
        definition.Kind switch
        {
            TestKind.LuscherStandard => () => navigationService.GoToStandardTestAsync().FireAndForget(),
            TestKind.LuscherBrief => () => navigationService.GoToAlternativeTestAsync().FireAndForget(),
            TestKind.Questionnaire => () => StartQuestionnaireAsync(definition, navigationService).FireAndForget(),
            _ => () => { }
        };

    private static Task StartQuestionnaireAsync(TestDefinition definition, INavigationService navigationService)
    {
        if (definition.Questions is null || definition.AnalyzerId is null)
        {
            return Task.CompletedTask;
        }

        Func<int, string>? analyzer = TestScoreAnalyzers.Resolve(definition.AnalyzerId);
        if (analyzer is null)
        {
            return Task.CompletedTask;
        }

        List<Question> questions = CloneQuestions(definition.Questions);

        return navigationService.GoToQuestionPageAsync(
            questions,
            analyzer,
            definition.SingleAnswer,
            new TestSessionInfo { TestId = definition.TestId, AnalyzerId = definition.AnalyzerId });
    }

    private static List<Question> CloneQuestions(IReadOnlyList<Question> source) =>
        source.Select(question => new Question
        {
            Number = question.Number,
            Context = question.Context,
            Answers = question.Answers
                .Select(answer => new Answer
                {
                    Number = answer.Number,
                    Ball = answer.Ball,
                    Text = answer.Text,
                    Selected = false
                })
                .ToList()
        }).ToList();
}
