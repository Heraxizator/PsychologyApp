using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed record QuestionnaireCompletionRequest(
    IEnumerable<Question> Questions,
    TestSessionInfo Session,
    DateTime StartedAtUtc);

public sealed record QuestionnaireSavedResult(
    QuestionnaireSubmission Submission,
    QuestionnaireResultDetail? Detail,
    QuestionnaireCompletionRequest Request);

public sealed class TestRunCoordinator(
    QuestionnaireSubmissionService submissionService,
    QuestionnaireDetailBuilder detailBuilder)
{
    public Task StartAsync(TestDefinition definition, INavigationService navigationService) =>
        definition.Kind switch
        {
            TestKind.LuscherStandard => navigationService.GoToStandardTestAsync(),
            TestKind.LuscherBrief => navigationService.GoToAlternativeTestAsync(),
            TestKind.Questionnaire => StartQuestionnaireAsync(definition, navigationService),
            _ => Task.CompletedTask
        };

    public async Task RetakeAsync(
        string testId,
        ITestCatalogService testCatalogService,
        INavigationService navigationService,
        CancellationToken cancellationToken = default)
    {
        TestDefinition? definition = await testCatalogService.GetByIdAsync(testId, cancellationToken);
        if (definition is null)
        {
            return;
        }

        await navigationService.GoToRootAsync();
        await StartAsync(definition, navigationService);
    }

    public async Task CompleteQuestionnaireAsync(
        QuestionnaireCompletionRequest request,
        IUserProgressService userProgressService,
        INavigationService navigationService,
        CancellationToken cancellationToken = default)
    {
        QuestionnaireSavedResult saved = await SaveQuestionnaireAsync(
            request,
            userProgressService,
            cancellationToken);

        await NavigateQuestionnaireResultAsync(saved, navigationService, cancellationToken);
    }

    public async Task<QuestionnaireSavedResult> SaveQuestionnaireAsync(
        QuestionnaireCompletionRequest request,
        IUserProgressService userProgressService,
        CancellationToken cancellationToken = default)
    {
        QuestionnaireSubmission submission = submissionService.Calculate(
            request.Questions,
            request.Session.AnalyzerId);

        QuestionnaireResultDetail? detail = await detailBuilder.BuildAsync(
            request.Questions,
            request.Session,
            request.StartedAtUtc,
            cancellationToken);

        string? detailJson = detailBuilder.Serialize(detail);

        await submissionService.SaveAsync(
            userProgressService,
            request.Session,
            submission.Score,
            submission.Interpretation,
            cancellationToken,
            detailJson);

        return new QuestionnaireSavedResult(submission, detail, request);
    }

    public async Task NavigateQuestionnaireResultAsync(
        QuestionnaireSavedResult saved,
        INavigationService navigationService,
        CancellationToken cancellationToken = default)
    {
        QuestionnaireSubmission submission = saved.Submission;
        QuestionnaireCompletionRequest request = saved.Request;

        bool completed = await NavigationRetry.TryExecuteWithRetryAsync(
            () => navigationService.GoToTestResultAsync(
                submission.Score,
                submission.Interpretation,
                submission.RecommendedTechnique,
                request.Session.TestId,
                submission.InterpretationDetail,
                request.Session.AnalyzerId,
                saved.Detail,
                cancellationToken),
            status => status == NavigationRunStatus.Completed,
            maxAttempts: 3,
            delay: TimeSpan.FromMilliseconds(150),
            cancellationToken);

        if (!completed)
        {
            throw new TestCompletionNavigationException();
        }
    }

    private static Task StartQuestionnaireAsync(TestDefinition definition, INavigationService navigationService)
    {
        if (definition.Questions is null || definition.AnalyzerId is null)
        {
            return Task.CompletedTask;
        }

        string analyzerId = definition.AnalyzerId;
        Func<int, string> scoreAnalyzer = score =>
            TestScoreLabelMapper.GetSummary(analyzerId, score) ?? string.Empty;

        List<Question> questions = definition.Questions
            .Select(question => new Question
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
            })
            .ToList();

        return navigationService.GoToQuestionPageAsync(
            questions,
            scoreAnalyzer,
            definition.SingleAnswer,
            new TestSessionInfo { TestId = definition.TestId, AnalyzerId = definition.AnalyzerId });
    }
}
