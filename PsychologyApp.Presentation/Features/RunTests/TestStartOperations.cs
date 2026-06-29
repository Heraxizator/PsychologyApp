using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.RunTests;

public static class TestStartOperations
{
    public static Task StartAsync(TestDefinition definition, INavigationService navigationService) =>
        definition.Kind switch
        {
            TestKind.LuscherStandard => navigationService.GoToStandardTestAsync(),
            TestKind.LuscherBrief => navigationService.GoToAlternativeTestAsync(),
            TestKind.Questionnaire => StartQuestionnaireAsync(definition, navigationService),
            _ => Task.CompletedTask
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
