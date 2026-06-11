using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.ViewModels.Tests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class QuestionViewModelTests
{
    public QuestionViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task ConfirmCommand_IncompleteAnswers_ShowsValidationToast()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new TestNavigationService(navigation.Object);
        var toast = new Mock<IToastService>();
        var dialog = new Mock<IDialogService>();
        var progress = new Mock<IUserProgressService>();

        List<Question> questions =
        [
            new()
            {
                Answers =
                [
                    new Answer { Ball = 1, Selected = false },
                    new Answer { Ball = 2, Selected = false }
                ]
            }
        ];

        QuestionViewModel viewModel = new(
            navigation.Object,
            questions,
            score => $"Result {score}",
            singleAnswer: true,
            toast.Object,
            dialog.Object,
            navigationService,
            progress.Object);

        viewModel.ConfirmCommand.Execute(null);
        await Task.Delay(50);

        toast.Verify(t => t.LongToast(AppStrings.TestsAnswerAllToast), Times.Once);
        dialog.Verify(
            d => d.AskAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ConfirmCommand_CompleteAnswers_NavigatesToTestResult()
    {
        var navigation = new Mock<INavigation>();
        var trackingNavigation = new TestResultTrackingNavigationService(navigation.Object);
        var toast = new Mock<IToastService>();
        var dialog = new Mock<IDialogService>();
        var progress = new Mock<IUserProgressService>();

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

        QuestionViewModel viewModel = new(
            navigation.Object,
            questions,
            score => $"Score {score}",
            singleAnswer: true,
            toast.Object,
            dialog.Object,
            trackingNavigation,
            progress.Object);

        viewModel.ConfirmCommand.Execute(null);
        await Task.Delay(50);

        dialog.Verify(
            d => d.AskAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
        Assert.Equal(5, trackingNavigation.LastScore);
        Assert.Equal("Score 5", trackingNavigation.LastInterpretation);
    }

    [Fact]
    public async Task ConfirmCommand_WithSession_SavesResult()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new TestNavigationService(navigation.Object);
        var toast = new Mock<IToastService>();
        var dialog = new Mock<IDialogService>();
        var progress = new Mock<IUserProgressService>();
        dialog
            .Setup(d => d.AskAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        List<Question> questions =
        [
            new()
            {
                Answers = [new Answer { Ball = 2, Selected = true }]
            }
        ];

        QuestionViewModel viewModel = new(
            navigation.Object,
            questions,
            score => $"Score {score}",
            singleAnswer: true,
            toast.Object,
            dialog.Object,
            navigationService,
            progress.Object,
            new TestSessionInfo { TestId = "beck", AnalyzerId = "beck" });

        viewModel.ConfirmCommand.Execute(null);
        await Task.Delay(50);

        progress.Verify(
            p => p.SaveTestResultAsync("beck", 2, "Score 2", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ConfirmCommand_WithRecommendation_PassesTechniqueToResultPage()
    {
        var navigation = new Mock<INavigation>();
        var trackingNavigation = new TestResultTrackingNavigationService(navigation.Object);
        var toast = new Mock<IToastService>();
        var dialog = new Mock<IDialogService>();
        var progress = new Mock<IUserProgressService>();

        List<Question> questions =
        [
            new()
            {
                Answers = [new Answer { Ball = 12, Selected = true }]
            }
        ];

        QuestionViewModel viewModel = new(
            navigation.Object,
            questions,
            score => $"Score {score}",
            singleAnswer: true,
            toast.Object,
            dialog.Object,
            trackingNavigation,
            progress.Object,
            new TestSessionInfo { TestId = "beck", AnalyzerId = "beck" });

        viewModel.ConfirmCommand.Execute(null);
        await Task.Delay(50);

        Assert.Equal(TechniqueId.Spin, trackingNavigation.LastRecommendedTechnique);
    }

    private sealed class TestResultTrackingNavigationService(INavigation navigation) : TestNavigationService(navigation)
    {
        public int? LastScore { get; private set; }
        public string? LastInterpretation { get; private set; }
        public TechniqueId? LastRecommendedTechnique { get; private set; }

        public override Task GoToTestResultAsync(int score, string interpretation, TechniqueId? recommendedTechnique = null)
        {
            LastScore = score;
            LastInterpretation = interpretation;
            LastRecommendedTechnique = recommendedTechnique;
            return Task.CompletedTask;
        }
    }
}
