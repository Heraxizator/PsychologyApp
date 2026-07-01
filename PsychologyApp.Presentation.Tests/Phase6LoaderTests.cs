using Moq;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Features.RunTests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class ListTechniqueSessionHelperTests
{
    [Fact]
    public async Task CompleteAsync_DelegatesToSessionCompletionService()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);
        Mock<INavigationService> navigation = new();
        navigation.Setup(n => n.GoToPracticeCompletionAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

        ListTechniqueSessionHelper helper = new(
            new TechniqueSessionCompletionService(Mock.Of<IPracticeReminderCoordinator>()),
            progress.Object,
            navigation.Object);
        DateTime startedAt = DateTime.UtcNow.AddMinutes(-1);

        await helper.CompleteAsync(
            TechniqueId.Polarity.ToString(),
            "Practice",
            "Polarity",
            startedAt);

        progress.Verify(
            p => p.RecordTechniqueCompletionAsync(
                TechniqueId.Polarity.ToString(),
                "Practice",
                "Polarity",
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        progress.Verify(
            p => p.DeleteSessionDraftAsync(TechniqueId.Polarity.ToString(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

public sealed class QuestionnaireSubmissionServiceTests
{
    [Fact]
    public void TryValidateAllAnswered_ReturnsFalse_WhenAnyQuestionUnanswered()
    {
        QuestionnaireSubmissionService service = new();
        List<Question> questions =
        [
            new()
            {
                Answers =
                [
                    new Answer { Ball = 1, Selected = true },
                    new Answer { Ball = 2, Selected = false }
                ]
            },
            new()
            {
                Answers =
                [
                    new Answer { Ball = 1, Selected = false },
                    new Answer { Ball = 2, Selected = false }
                ]
            }
        ];

        Assert.False(service.TryValidateAllAnswered(questions));
    }

    [Fact]
    public void Calculate_SumsSelectedBallsAndMapsAnalyzer()
    {
        QuestionnaireSubmissionService service = new();
        List<Question> questions =
        [
            new()
            {
                Answers =
                [
                    new Answer { Ball = 3, Selected = true },
                    new Answer { Ball = 1, Selected = false }
                ]
            },
            new()
            {
                Answers =
                [
                    new Answer { Ball = 2, Selected = true },
                    new Answer { Ball = 4, Selected = false }
                ]
            }
        ];

        QuestionnaireSubmission submission = service.Calculate(
            questions,
            score => $"Score {score}",
            new TestSessionInfo { TestId = "beck", AnalyzerId = "beck" });

        Assert.Equal(5, submission.Score);
        Assert.Equal("Score 5", submission.Interpretation);
        Assert.Equal(TechniqueId.Paper, submission.RecommendedTechnique);
        Assert.False(string.IsNullOrWhiteSpace(submission.InterpretationDetail));
    }

    [Fact]
    public async Task SaveAsync_SkipsWhenTestIdMissing()
    {
        Mock<IUserProgressService> progress = new();
        QuestionnaireSubmissionService service = new();

        await service.SaveAsync(
            progress.Object,
            new TestSessionInfo { TestId = string.Empty, AnalyzerId = "beck" },
            score: 1,
            summary: "Mild",
            CancellationToken.None);

        progress.Verify(
            p => p.SaveTestResultAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SaveAsync_PersistsResultWhenSessionPresent()
    {
        Mock<IUserProgressService> progress = new();
        QuestionnaireSubmissionService service = new();

        await service.SaveAsync(
            progress.Object,
            new TestSessionInfo { TestId = "beck", AnalyzerId = "beck" },
            score: 2,
            summary: "Score 2",
            CancellationToken.None);

        progress.Verify(
            p => p.SaveTestResultAsync("beck", 2, "Score 2", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
