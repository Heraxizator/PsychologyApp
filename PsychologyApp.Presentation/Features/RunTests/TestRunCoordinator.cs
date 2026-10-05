using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
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
    QuestionnaireDetailBuilder detailBuilder,
    IClinicalCareService? clinicalCareService = null)
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

        // An answer other than "never" to the question about suicidal thoughts is a risk signal in its own right, whatever the total:
        // it is recorded exactly like the risk check, so the startup gate and the dashboard treat it the same way.
        if (submission.SelfHarmItemEndorsed && clinicalCareService is not null)
        {
            // The result is already saved at this point, so a failure to record the risk must not fail the whole completion
            // (a retry would save the result twice); the crisis screen still opens below.
            try
            {
                await clinicalCareService.AssessRiskAsync(
                    new RiskAssessmentInput { HasSelfHarmThoughts = true, Source = $"test:{request.Session.AnalyzerId}" },
                    cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                System.Diagnostics.Trace.TraceError("Could not record the risk signal from a questionnaire: " + ex);
            }
        }

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
            // A repeated tap on "finish" is dropped as a duplicate: the page is already opening, which is not a failure to retry.
            status => status is NavigationRunStatus.Completed or NavigationRunStatus.DroppedDuplicate,
            maxAttempts: 3,
            delay: TimeSpan.FromMilliseconds(150),
            cancellationToken);

        if (!completed)
        {
            throw new TestCompletionNavigationException();
        }

        // After the result is on screen: help first, then the numbers. Back from the crisis screen returns to the result.
        if (submission.SelfHarmItemEndorsed)
        {
            await navigationService.GoToCrisisHubAsync();
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
