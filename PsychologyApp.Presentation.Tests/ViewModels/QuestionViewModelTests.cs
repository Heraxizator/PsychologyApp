using PsychologyApp.Presentation.Shared.Navigation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Tests;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Pages.RunTests.Question;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Shared.UI.Overlays;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>Taking a questionnaire step by step: one question at a time, nothing skipped, one save at the end.</summary>
public sealed class QuestionViewModelTests
{
    private readonly Mock<IToastService> _toasts = new();
    private readonly Mock<INavigationService> _navigation = new();
    private readonly Mock<IUserProgressService> _progress = new();
    private readonly Mock<IQuestionnaireScoringService> _scoring = new();
    private readonly Mock<IQuestionnaireResultDetailService> _detail = new();
    private readonly Mock<ITestCatalogService> _catalog = new();
    private readonly Mock<PsychologyApp.Application.ClinicalCare.IClinicalCareService> _care = new();

    private static List<TestQuestion> Questions(int count) =>
        Enumerable.Range(1, count).Select(n => new TestQuestion
        {
            Number = n,
            Context = $"Question {n}",
            Answers = [new Answer { Number = 1, Ball = 0, Text = "never" }, new Answer { Number = 2, Ball = 1, Text = "often" }]
        }).ToList();

    private QuestionViewModel Create(int count = 3, bool single = true, string? testId = "gad7")
    {
        _scoring.Setup(s => s.TryValidateAllAnswered(It.IsAny<IEnumerable<TestQuestion>>()))
            .Returns<IEnumerable<TestQuestion>>(qs => qs.All(q => q.Answers.Any(a => a.Selected)));
        _scoring.Setup(s => s.Calculate(It.IsAny<IEnumerable<TestQuestion>>(), "gad7"))
            .Returns(new QuestionnaireScoringResult(7, 1, null, "gad7"));
        _detail.Setup(d => d.BuildAsync(It.IsAny<QuestionnaireDetailBuildRequest>(), It.IsAny<ITestCatalogService>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((QuestionnaireResultDetail?)null);
        _progress.Setup(p => p.SaveTestResultAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _navigation.Setup(n => n.GoToTestResultAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TechniqueId?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<QuestionnaireResultDetail?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(NavigationRunStatus.Completed);
        _navigation.Setup(n => n.GoBackAsync()).Returns(Task.CompletedTask);

        QuestionnaireSubmissionService submission = new(_scoring.Object);
        TestRunCoordinator coordinator = new(submission, new QuestionnaireDetailBuilder(_detail.Object, _catalog.Object), _care.Object);
        return new QuestionViewModel(
            Questions(count),
            score => $"score {score}",
            single,
            _toasts.Object,
            new Mock<IDialogService>().Object,
            _navigation.Object,
            _progress.Object,
            submission,
            coordinator,
            _catalog.Object,
            NullLogger<QuestionViewModel>.Instance,
            testId is null ? null : new TestSessionInfo { TestId = testId, AnalyzerId = "gad7" });
    }

    private static void Pick(QuestionViewModel viewModel, int answerIndex) =>
        viewModel.CurrentAnswers[answerIndex].SelectCommand.Execute(null);

    [Fact]
    public void TheFirstQuestionIsShownWithItsAnswersAndNoBackButton()
    {
        QuestionViewModel viewModel = Create(3);

        Assert.Equal(1, viewModel.CurrentQuestionNumber);
        Assert.Equal("Question 1", viewModel.CurrentQuestionContext);
        Assert.Equal(["never", "often"], viewModel.CurrentAnswers.Select(a => a.Text));
        Assert.True(viewModel.IsFirstStep);
        Assert.False(viewModel.IsPreviousVisible);
        Assert.False(viewModel.IsLastStep);
        Assert.False(viewModel.CanAdvance);
        Assert.Equal(3, viewModel.TotalCount);
    }

    [Fact]
    public async Task NextIsRefusedUntilTheCurrentQuestionIsAnsweredAndTheHintShows()
    {
        QuestionViewModel viewModel = Create(3);
        bool hinted = false;
        viewModel.ValidationHintRequested += (_, _) => hinted = true;

        Assert.False(viewModel.NextCommand.CanExecute(null));
        viewModel.NextCommand.Execute(null);
        await Task.Delay(100);

        Assert.Equal(0, viewModel.CurrentIndex);
        Assert.True(hinted);

        Pick(viewModel, 0);
        await Task.Delay(50);
        Assert.True(viewModel.CanAdvance);
    }

    [Fact]
    public async Task ASingleAnswerSelectsOnlyThatAnswer()
    {
        QuestionViewModel viewModel = Create(3);

        Pick(viewModel, 0);
        await Task.Delay(20);
        viewModel.CurrentAnswers[1].SelectCommand.Execute(null);

        Assert.False(viewModel.CurrentAnswers[0].IsSelected);
        Assert.True(viewModel.CurrentAnswers[1].IsSelected);
        Assert.Equal([2], viewModel.CurrentQuestion!.Answers.Where(a => a.Selected).Select(a => a.Number));
    }

    [Fact]
    public async Task ASingleAnswerMovesOnToTheNextQuestionByItself()
    {
        QuestionViewModel viewModel = Create(3);

        Pick(viewModel, 1);
        await VmTestHelpers.WaitUntilAsync(() => viewModel.CurrentIndex == 1, 3000);

        Assert.Equal(2, viewModel.CurrentQuestionNumber);
        Assert.True(viewModel.IsPreviousVisible);
        Assert.False(viewModel.CanAdvance);
    }

    [Fact]
    public async Task TheLastQuestionDoesNotAdvanceByItselfAndFinishSavesOnceAndOpensTheResult()
    {
        QuestionViewModel viewModel = Create(1);

        Pick(viewModel, 0);
        await Task.Delay(500);
        Assert.True(viewModel.IsLastStep);
        _progress.Verify(p => p.SaveTestResultAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never);

        await VmTestHelpers.RunAsync(viewModel.NextCommand);

        _progress.Verify(p => p.SaveTestResultAsync("gad7", 7, It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
        _navigation.Verify(n => n.GoToTestResultAsync(
            7, It.IsAny<string>(), It.IsAny<TechniqueId?>(), "gad7", It.IsAny<string?>(), "gad7", It.IsAny<QuestionnaireResultDetail?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AMultipleChoiceQuestionAllowsSeveralAnswersAndNeverAdvancesByItself()
    {
        QuestionViewModel viewModel = Create(2, single: false);

        Pick(viewModel, 0);
        await Task.Delay(20);
        viewModel.CurrentAnswers[1].SelectCommand.Execute(null);
        await Task.Delay(500);

        Assert.True(viewModel.IsMultiChoiceHintVisible);
        Assert.Equal(2, viewModel.CurrentQuestion!.Answers.Count(a => a.Selected));
        Assert.Equal(0, viewModel.CurrentIndex);

        viewModel.CurrentAnswers[0].SelectCommand.Execute(null);
        await Task.Delay(500);
        Assert.Equal([2], viewModel.CurrentQuestion.Answers.Where(a => a.Selected).Select(a => a.Number));
    }

    [Fact]
    public async Task GoingBackKeepsTheAnswersAndTheFirstStepLeavesTheTest()
    {
        QuestionViewModel viewModel = Create(3);
        Pick(viewModel, 1);
        await VmTestHelpers.WaitUntilAsync(() => viewModel.CurrentIndex == 1, 3000);

        await VmTestHelpers.RunAsync(viewModel.PreviousCommand);

        Assert.Equal(0, viewModel.CurrentIndex);
        Assert.True(viewModel.CurrentAnswers[1].IsSelected);

        await VmTestHelpers.RunAsync(viewModel.PreviousCommand);
        _navigation.Verify(n => n.GoBackAsync(), Times.Once);
    }

    [Fact]
    public async Task ASaveThatFailsIsReportedAndCanBeRetried()
    {
        QuestionViewModel viewModel = Create(1);
        _progress.SetupSequence(p => p.SaveTestResultAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("db"))
            .Returns(Task.CompletedTask);
        Pick(viewModel, 0);
        await Task.Delay(50);

        await VmTestHelpers.RunAsync(viewModel.NextCommand);
        _toasts.Verify(t => t.LongToast(AppStrings.TestsResultSaveFailedMessage, It.IsAny<AppToastKind>()), Times.Once);
        _navigation.Verify(n => n.GoToTestResultAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TechniqueId?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<QuestionnaireResultDetail?>(), It.IsAny<CancellationToken>()), Times.Never);

        await VmTestHelpers.RunAsync(viewModel.NextCommand);
        _navigation.Verify(n => n.GoToTestResultAsync(
            7, It.IsAny<string>(), It.IsAny<TechniqueId?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<QuestionnaireResultDetail?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void LongQuestionnairesUseABarAndShortOnesUseSteps()
    {
        Assert.False(Create(5).UseBarProgress);
        Assert.True(Create(9).UseBarProgress);
    }

    [Fact]
    public async Task TheProgressGrowsWithTheStep()
    {
        QuestionViewModel viewModel = Create(4);
        double first = viewModel.Progress;

        Pick(viewModel, 0);
        await VmTestHelpers.WaitUntilAsync(() => viewModel.CurrentIndex == 1, 3000);

        Assert.True(viewModel.Progress > first);
        Assert.Equal(0.5, viewModel.Progress, 3);
    }
}
