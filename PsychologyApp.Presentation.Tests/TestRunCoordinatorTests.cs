using Moq;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.Navigation;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TestRunCoordinatorTests
{
    [Fact]
    public async Task CompleteQuestionnaireAsync_SavesAndNavigatesToResult()
    {
        var navigation = new Mock<INavigation>();
        RecordingNavigationService navigationService = new(navigation.Object);
        var progress = new Mock<IUserProgressService>();
        FakeTestCatalogService catalog = new FakeTestCatalogService().WithCatalog(new TestDefinition
        {
            TestId = "beck",
            Title = "Beck",
            Subtitle = "Sub",
            Description = "Desc",
            Comment = "Note",
            Algorithm = ["Step"],
            Kind = TestKind.Questionnaire,
            AnalyzerId = "beck",
            Construct = "Depression"
        });

        TestRunCoordinator coordinator = TestRunTestHelpers.CreateCoordinator(catalog);
        List<Question> questions =
        [
            new()
            {
                Number = 1,
                Context = "Q1",
                Answers = [new Answer { Ball = 2, Text = "A", Selected = true }]
            }
        ];

        await coordinator.CompleteQuestionnaireAsync(
            new QuestionnaireCompletionRequest(
                questions,
                new TestSessionInfo { TestId = "beck", AnalyzerId = "beck" },
                DateTime.UtcNow.AddMinutes(-2)),
            progress.Object,
            navigationService);

        progress.Verify(p => p.SaveTestResultAsync(
            "beck",
            2,
            It.Is<string>(summary => summary.Contains('2') || summary.Contains("depress", StringComparison.OrdinalIgnoreCase)),
            It.Is<string?>(json => !string.IsNullOrWhiteSpace(json)),
            It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(2, navigationService.LastScore);
        Assert.NotNull(navigationService.LastDetail);
        Assert.Equal(1, navigationService.NavigationAttempts);
    }

    [Fact]
    public async Task CompleteQuestionnaireAsync_ThrowsWhenNavigationFails()
    {
        var navigation = new Mock<INavigation>();
        DroppingNavigationService navigationService = new(navigation.Object);
        var progress = new Mock<IUserProgressService>();
        progress
            .Setup(p => p.SaveTestResultAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        TestRunCoordinator coordinator = TestRunTestHelpers.CreateCoordinator(new FakeTestCatalogService());
        List<Question> questions =
        [
            new()
            {
                Answers = [new Answer { Ball = 1, Selected = true }]
            }
        ];

        await Assert.ThrowsAsync<TestCompletionNavigationException>(() =>
            coordinator.CompleteQuestionnaireAsync(
                new QuestionnaireCompletionRequest(
                    questions,
                    new TestSessionInfo { TestId = "beck", AnalyzerId = "beck" },
                    DateTime.UtcNow),
                progress.Object,
                navigationService));
    }

    private sealed class RecordingNavigationService(INavigation navigation) : TestNavigationService(navigation)
    {
        public int? LastScore { get; private set; }
        public QuestionnaireResultDetail? LastDetail { get; private set; }
        public int NavigationAttempts { get; private set; }

        public override Task<NavigationRunStatus> GoToTestResultAsync(
            int score,
            string interpretation,
            TechniqueId? recommendedTechnique = null,
            string? testId = null,
            string? interpretationDetail = null,
            string? analyzerId = null,
            QuestionnaireResultDetail? detail = null,
            CancellationToken cancellationToken = default)
        {
            NavigationAttempts++;
            LastScore = score;
            LastDetail = detail;
            return Task.FromResult(NavigationRunStatus.Completed);
        }
    }

    private sealed class DroppingNavigationService(INavigation navigation) : TestNavigationService(navigation)
    {
        public override Task<NavigationRunStatus> GoToTestResultAsync(
            int score,
            string interpretation,
            TechniqueId? recommendedTechnique = null,
            string? testId = null,
            string? interpretationDetail = null,
            string? analyzerId = null,
            QuestionnaireResultDetail? detail = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(NavigationRunStatus.Failed);
    }
}
