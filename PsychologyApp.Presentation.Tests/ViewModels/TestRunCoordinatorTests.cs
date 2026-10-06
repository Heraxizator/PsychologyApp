using PsychologyApp.Presentation.Shared.Navigation;
using Moq;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Tests;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Features.RunTests;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>Finishing a questionnaire: the result is saved once, an answer about suicidal thoughts is recorded as a risk, and help opens before the numbers.</summary>
public sealed class TestRunCoordinatorTests
{
    private readonly Mock<IQuestionnaireScoringService> _scoring = new();
    private readonly Mock<IQuestionnaireResultDetailService> _detail = new();
    private readonly Mock<ITestCatalogService> _catalog = new();
    private readonly Mock<IUserProgressService> _progress = new();
    private readonly Mock<IClinicalCareService> _care = new();
    private readonly Mock<INavigationService> _navigation = new();
    private readonly List<string> _calls = [];

    private TestRunCoordinator Create(bool selfHarm, int score = 12, string analyzerId = "beck")
    {
        _scoring.Setup(s => s.Calculate(It.IsAny<IEnumerable<TestQuestion>>(), analyzerId))
            .Returns(new QuestionnaireScoringResult(score, 1, TechniqueId.Grounding, analyzerId, selfHarm));
        _detail.Setup(d => d.BuildAsync(It.IsAny<QuestionnaireDetailBuildRequest>(), It.IsAny<ITestCatalogService>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((QuestionnaireResultDetail?)null);
        _detail.Setup(d => d.Serialize(It.IsAny<QuestionnaireResultDetail?>())).Returns("{\"detail\":1}");
        _progress.Setup(p => p.SaveTestResultAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Callback(() => _calls.Add("saved")).Returns(Task.CompletedTask);
        _care.Setup(c => c.AssessRiskAsync(It.IsAny<RiskAssessmentInput>(), It.IsAny<CancellationToken>()))
            .Callback(() => _calls.Add("risk")).ReturnsAsync(new RiskAssessmentDTO());
        _navigation.Setup(n => n.GoToTestResultAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TechniqueId?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<QuestionnaireResultDetail?>(), It.IsAny<CancellationToken>()))
            .Callback(() => _calls.Add("result")).ReturnsAsync(NavigationRunStatus.Completed);
        _navigation.Setup(n => n.GoToCrisisHubAsync()).Callback(() => _calls.Add("crisis")).Returns(Task.CompletedTask);

        QuestionnaireSubmissionService submission = new(_scoring.Object);
        QuestionnaireDetailBuilder detail = new(_detail.Object, _catalog.Object);
        return new TestRunCoordinator(submission, detail, _care.Object);
    }

    private static QuestionnaireCompletionRequest Request(string analyzerId = "beck") =>
        new([new TestQuestion { Number = 1 }], new TestSessionInfo { TestId = "beck-test", AnalyzerId = analyzerId }, DateTime.UtcNow);

    [Fact]
    public async Task TheResultIsSavedWithItsScoreSummaryAndDetail()
    {
        TestRunCoordinator coordinator = Create(selfHarm: false);

        QuestionnaireSavedResult saved = await coordinator.SaveQuestionnaireAsync(Request(), _progress.Object);

        _progress.Verify(p => p.SaveTestResultAsync("beck-test", 12, It.IsAny<string>(), "{\"detail\":1}", It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(12, saved.Submission.Score);
        Assert.False(saved.Submission.SelfHarmItemEndorsed);
    }

    [Fact]
    public async Task WithoutTheSuicideItemNoRiskIsRecorded()
    {
        TestRunCoordinator coordinator = Create(selfHarm: false);

        await coordinator.SaveQuestionnaireAsync(Request(), _progress.Object);

        _care.Verify(c => c.AssessRiskAsync(It.IsAny<RiskAssessmentInput>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AnEndorsedSuicideItemIsRecordedAsARiskWhateverTheTotal()
    {
        TestRunCoordinator coordinator = Create(selfHarm: true, score: 3);
        RiskAssessmentInput? sent = null;
        _care.Setup(c => c.AssessRiskAsync(It.IsAny<RiskAssessmentInput>(), It.IsAny<CancellationToken>()))
            .Callback<RiskAssessmentInput, CancellationToken>((input, _) => sent = input).ReturnsAsync(new RiskAssessmentDTO());

        await coordinator.SaveQuestionnaireAsync(Request(), _progress.Object);

        Assert.NotNull(sent);
        Assert.True(sent!.HasSelfHarmThoughts);
        Assert.Equal("test:beck", sent.Source);
    }

    [Fact]
    public async Task AFailureToRecordTheRiskDoesNotLoseTheSavedResult()
    {
        TestRunCoordinator coordinator = Create(selfHarm: true);
        _care.Setup(c => c.AssessRiskAsync(It.IsAny<RiskAssessmentInput>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("db"));

        QuestionnaireSavedResult saved = await coordinator.SaveQuestionnaireAsync(Request(), _progress.Object);

        Assert.True(saved.Submission.SelfHarmItemEndorsed);
        _progress.Verify(p => p.SaveTestResultAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HelpOpensAfterTheResultWhenTheSuicideItemWasEndorsed()
    {
        TestRunCoordinator coordinator = Create(selfHarm: true);

        await coordinator.CompleteQuestionnaireAsync(Request(), _progress.Object, _navigation.Object);

        Assert.Equal(["saved", "risk", "result", "crisis"], _calls);
    }

    [Fact]
    public async Task WithoutTheSuicideItemOnlyTheResultOpens()
    {
        TestRunCoordinator coordinator = Create(selfHarm: false);

        await coordinator.CompleteQuestionnaireAsync(Request(), _progress.Object, _navigation.Object);

        Assert.Equal(["saved", "result"], _calls);
    }

    [Fact]
    public async Task ASessionWithoutATestIdSavesNothingButStillShowsTheResult()
    {
        TestRunCoordinator coordinator = Create(selfHarm: false);
        QuestionnaireCompletionRequest request = new([new TestQuestion { Number = 1 }], new TestSessionInfo { TestId = " ", AnalyzerId = "beck" }, DateTime.UtcNow);

        await coordinator.CompleteQuestionnaireAsync(request, _progress.Object, _navigation.Object);

        _progress.Verify(p => p.SaveTestResultAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Contains("result", _calls);
    }

    [Fact]
    public async Task ADuplicateTapOnFinishIsNotAFailure()
    {
        TestRunCoordinator coordinator = Create(selfHarm: false);
        _navigation.Setup(n => n.GoToTestResultAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TechniqueId?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<QuestionnaireResultDetail?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(NavigationRunStatus.DroppedDuplicate);

        await coordinator.CompleteQuestionnaireAsync(Request(), _progress.Object, _navigation.Object);

        _navigation.Verify(n => n.GoToCrisisHubAsync(), Times.Never);
    }

    [Fact]
    public async Task ANavigationThatKeepsFailingIsReportedAfterThreeTries()
    {
        TestRunCoordinator coordinator = Create(selfHarm: true);
        _navigation.Setup(n => n.GoToTestResultAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TechniqueId?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<QuestionnaireResultDetail?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(NavigationRunStatus.Failed);

        await Assert.ThrowsAsync<TestCompletionNavigationException>(() =>
            coordinator.CompleteQuestionnaireAsync(Request(), _progress.Object, _navigation.Object));

        _navigation.Verify(n => n.GoToTestResultAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<TechniqueId?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<QuestionnaireResultDetail?>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }
}
