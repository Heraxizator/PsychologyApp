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
        navigation.Setup(n => n.GoToPracticeCompletionAsync(It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<long?>())).Returns(Task.CompletedTask);

        ListTechniqueSessionHelper helper = new(
            new TechniqueSessionCompletionService(Mock.Of<IPracticeReminderCoordinator>(), Mock.Of<PsychologyApp.Application.ClinicalCare.IClinicalCareService>()),
            progress.Object,
            navigation.Object);
        DateTime startedAt = DateTime.UtcNow.AddMinutes(-1);

        await helper.CompleteAsync(
            TechniqueId.Polarity.ToString(),
            "Practice",
            "Polarity",
            startedAt);

        // One write records the session and drops its draft, in one transaction, so a crash cannot keep the draft of a finished session.
        progress.Verify(
            p => p.RecordSessionOutcomeAsync(
                It.Is<SessionOutcomeRequest>(r =>
                    r.ItemKey == TechniqueId.Polarity.ToString()
                    && r.ModuleName == "Practice"
                    && r.PageName == "Polarity"
                    && r.DeleteDraft),
                It.IsAny<CancellationToken>()),
            Times.Once);
        navigation.Verify(n => n.GoToPracticeCompletionAsync(It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<long?>()), Times.Once);
    }
}

[Collection("Localization")]
public sealed class QuestionnaireSubmissionServiceTests
{
    [Fact]
    public void TryValidateAllAnswered_ReturnsFalse_WhenAnyQuestionUnanswered()
    {
        QuestionnaireSubmissionService service = new(new QuestionnaireScoringService());
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
        QuestionnaireSubmissionService service = new(new QuestionnaireScoringService());
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
            "beck");

        Assert.Equal(5, submission.Score);
        Assert.Equal(TestScoreLabelMapper.GetSummary("beck", 5), submission.Interpretation);
        Assert.False(string.IsNullOrWhiteSpace(submission.InterpretationDetail));
    }

    [Fact]
    public async Task SaveAsync_SkipsWhenTestIdMissing()
    {
        Mock<IUserProgressService> progress = new();
        QuestionnaireSubmissionService service = new(new QuestionnaireScoringService());

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
        QuestionnaireSubmissionService service = new(new QuestionnaireScoringService());

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
