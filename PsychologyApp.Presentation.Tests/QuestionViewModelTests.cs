using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Pages.RunTests.Question;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class QuestionViewModelTests
{
    private static readonly QuestionnaireSubmissionService SubmissionService = new();
    private static readonly Mock<ITestCatalogService> CatalogService = new();
    private static readonly QuestionnaireDetailBuilder DetailBuilder = new(CatalogService.Object);
    private static readonly TestRunCoordinator RunCoordinator = new(SubmissionService, DetailBuilder);
    private static readonly ILogger<QuestionViewModel> Logger = NullLogger<QuestionViewModel>.Instance;

    public QuestionViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task NextCommand_WithoutAnswer_ShowsValidationHint()
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

        QuestionViewModel viewModel = CreateViewModel(
            questions,
            navigationService,
            toast.Object,
            dialog.Object,
            progress.Object);

        Assert.False(viewModel.NextCommand.CanExecute(null));
        viewModel.NextCommand.Execute(null);
        await Task.Delay(50);

        Assert.True(viewModel.IsValidationHintVisible);
        toast.Verify(t => t.LongToast(AppStrings.TestsAnswerCurrentToast), Times.Never);
    }

    [Fact]
    public async Task NextCommand_AfterSelectBeforeAutoAdvance_DoesNotShowPhantomToast()
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

        QuestionViewModel viewModel = CreateViewModel(
            questions,
            navigationService,
            toast.Object,
            dialog.Object,
            progress.Object);

        viewModel.CurrentAnswers[0].SelectCommand.Execute(null);
        viewModel.NextCommand.Execute(null);
        await Task.Delay(400);

        Assert.Equal(1, viewModel.CurrentIndex);
        toast.Verify(t => t.LongToast(AppStrings.TestsAnswerCurrentToast), Times.Never);
    }

    [Fact]
    public async Task TryAutoAdvance_DoesNotAdvance_WhenPreferenceDisabled()
    {
        UserPreferences.UseInMemoryStorage(new UserPreferencesState { QuestionnaireAutoAdvance = false });

        try
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

            QuestionViewModel viewModel = CreateViewModel(
                questions,
                navigationService,
                toast.Object,
                dialog.Object,
                progress.Object);

            viewModel.CurrentAnswers[0].SelectCommand.Execute(null);
            await Task.Delay(400);

            Assert.Equal(0, viewModel.CurrentIndex);
        }
        finally
        {
            UserPreferences.ResetInMemoryStorage();
        }
    }

    [Fact]
    public async Task NextCommand_OnLastStepWithAllAnswers_NavigatesToTestResult()
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

        QuestionViewModel viewModel = CreateViewModel(
            questions,
            trackingNavigation,
            toast.Object,
            dialog.Object,
            progress.Object);

        viewModel.NextCommand.Execute(null);
        await Task.Delay(50);
        viewModel.NextCommand.Execute(null);
        await Task.Delay(50);

        Assert.Equal(5, trackingNavigation.LastScore);
        Assert.Equal("Score 5", trackingNavigation.LastInterpretation);
    }

    [Fact]
    public async Task Finish_SavesQuestionnaireDetailJson_WhenSessionIsPresent()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new TestNavigationService(navigation.Object);
        var toast = new Mock<IToastService>();
        var dialog = new Mock<IDialogService>();
        var progress = new Mock<IUserProgressService>();

        CatalogService
            .Setup(s => s.GetByIdAsync("heck_hess", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestDefinition
            {
                TestId = "heck_hess",
                Title = "Heck & Hess",
                Subtitle = "Sub",
                Description = "Desc",
                Comment = "Note",
                Algorithm = ["Step"],
                Kind = TestKind.Questionnaire,
                AnalyzerId = "heck_hess",
                Questions = [],
                SingleAnswer = true,
                Construct = "Neuroticism"
            });

        progress
            .Setup(p => p.SaveTestResultAsync(
                "heck_hess",
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        List<Question> questions =
        [
            new()
            {
                Number = 1,
                Context = "Q1",
                Answers =
                [
                    new Answer { Ball = 1, Text = "Yes", Selected = true },
                    new Answer { Ball = 0, Text = "No", Selected = false }
                ]
            }
        ];

        QuestionViewModel viewModel = CreateViewModel(
            questions,
            navigationService,
            toast.Object,
            dialog.Object,
            progress.Object,
            new TestSessionInfo { TestId = "heck_hess", AnalyzerId = "heck_hess" });

        viewModel.NextCommand.Execute(null);
        await Task.Delay(50);

        progress.Verify(p => p.SaveTestResultAsync(
            "heck_hess",
            It.IsAny<int?>(),
            It.IsAny<string>(),
            It.Is<string?>(json => !string.IsNullOrWhiteSpace(json) && json.Contains("\"testId\":\"heck_hess\"", StringComparison.OrdinalIgnoreCase)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task NextCommand_OnLastStep_WithSession_SavesResult()
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
                Answers = [new Answer { Ball = 2, Selected = true }]
            }
        ];

        QuestionViewModel viewModel = CreateViewModel(
            questions,
            navigationService,
            toast.Object,
            dialog.Object,
            progress.Object,
            new TestSessionInfo { TestId = "beck", AnalyzerId = "beck" });

        viewModel.NextCommand.Execute(null);
        await Task.Delay(50);

        progress.Verify(
            p => p.SaveTestResultAsync("beck", 2, "Score 2", null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task NextCommand_OnLastStep_PassesTechniqueToResultPage()
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

        QuestionViewModel viewModel = CreateViewModel(
            questions,
            trackingNavigation,
            toast.Object,
            dialog.Object,
            progress.Object,
            new TestSessionInfo { TestId = "beck", AnalyzerId = "beck" });

        viewModel.NextCommand.Execute(null);
        await Task.Delay(50);

        Assert.Equal(TechniqueId.Spin, trackingNavigation.LastRecommendedTechnique);
    }

    [Fact]
    public async Task NextCommand_OnLastStep_WhenNavigationFails_ShowsResultNavigationToast()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new FailingResultNavigationService(navigation.Object);
        var toast = new Mock<IToastService>();
        var dialog = new Mock<IDialogService>();
        var progress = new Mock<IUserProgressService>();
        progress
            .Setup(p => p.SaveTestResultAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        List<Question> questions =
        [
            new()
            {
                Answers = [new Answer { Ball = 2, Selected = true }]
            }
        ];

        QuestionViewModel viewModel = CreateViewModel(
            questions,
            navigationService,
            toast.Object,
            dialog.Object,
            progress.Object,
            new TestSessionInfo { TestId = "beck", AnalyzerId = "beck" });

        viewModel.NextCommand.Execute(null);
        await Task.Delay(600);

        toast.Verify(t => t.LongToast(AppStrings.TestsResultNavigationFailedMessage), Times.Once);
        toast.Verify(t => t.LongToast(AppStrings.UnexpectedErrorMessage), Times.Never);
    }

    [Fact]
    public async Task NextCommand_OnLastStep_WhenSaveFails_ShowsResultSaveToast()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new TestNavigationService(navigation.Object);
        var toast = new Mock<IToastService>();
        var dialog = new Mock<IDialogService>();
        var progress = new Mock<IUserProgressService>();
        progress
            .Setup(p => p.SaveTestResultAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database unavailable"));

        List<Question> questions =
        [
            new()
            {
                Answers = [new Answer { Ball = 2, Selected = true }]
            }
        ];

        QuestionViewModel viewModel = CreateViewModel(
            questions,
            navigationService,
            toast.Object,
            dialog.Object,
            progress.Object,
            new TestSessionInfo { TestId = "beck", AnalyzerId = "beck" });

        viewModel.NextCommand.Execute(null);
        await Task.Delay(100);

        toast.Verify(t => t.LongToast(AppStrings.TestsResultSaveFailedMessage), Times.Once);
        toast.Verify(t => t.LongToast(AppStrings.UnexpectedErrorMessage), Times.Never);
    }

    [Fact]
    public async Task NextCommand_OnLastStep_WhenNavigationThrows_ShowsResultNavigationToast()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new ThrowingResultNavigationService(navigation.Object);
        var toast = new Mock<IToastService>();
        var dialog = new Mock<IDialogService>();
        var progress = new Mock<IUserProgressService>();
        progress
            .Setup(p => p.SaveTestResultAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        List<Question> questions =
        [
            new()
            {
                Answers = [new Answer { Ball = 2, Selected = true }]
            }
        ];

        QuestionViewModel viewModel = CreateViewModel(
            questions,
            navigationService,
            toast.Object,
            dialog.Object,
            progress.Object,
            new TestSessionInfo { TestId = "beck", AnalyzerId = "beck" });

        viewModel.NextCommand.Execute(null);
        await Task.Delay(600);

        toast.Verify(t => t.LongToast(AppStrings.TestsResultNavigationFailedMessage), Times.Once);
        toast.Verify(t => t.LongToast(AppStrings.TestsResultSaveFailedMessage), Times.Never);
        progress.Verify(
            p => p.SaveTestResultAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task NextCommand_OnLastStep_WhenNavigationFailsOnce_RetriesAutomaticallyWithoutToast()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new RetryResultNavigationService(navigation.Object);
        var toast = new Mock<IToastService>();
        var dialog = new Mock<IDialogService>();
        var progress = new Mock<IUserProgressService>();
        progress
            .Setup(p => p.SaveTestResultAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        List<Question> questions =
        [
            new()
            {
                Answers = [new Answer { Ball = 2, Selected = true }]
            }
        ];

        QuestionViewModel viewModel = CreateViewModel(
            questions,
            navigationService,
            toast.Object,
            dialog.Object,
            progress.Object,
            new TestSessionInfo { TestId = "beck", AnalyzerId = "beck" });

        viewModel.NextCommand.Execute(null);
        await Task.Delay(500);

        progress.Verify(
            p => p.SaveTestResultAsync(
                It.IsAny<string>(),
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.Equal(2, navigationService.NavigationAttempts);
        toast.Verify(t => t.LongToast(AppStrings.TestsResultNavigationFailedMessage), Times.Never);
    }

    [Fact]
    public void UseBarProgress_IsTrue_WhenMoreThanSevenQuestions()
    {
        List<Question> questions = Enumerable.Range(1, 8)
            .Select(i => new Question
            {
                Number = i,
                Answers = [new Answer { Ball = 1, Selected = false }]
            })
            .ToList();

        QuestionViewModel viewModel = CreateViewModel(
            questions,
            new TestNavigationService(new Mock<INavigation>().Object),
            new Mock<IToastService>().Object,
            new Mock<IDialogService>().Object,
            new Mock<IUserProgressService>().Object);

        Assert.True(viewModel.UseBarProgress);
        Assert.Equal(0.125, viewModel.Progress, 3);
    }

    [Fact]
    public async Task PreviousCommand_OnFirstStep_GoesBack()
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
                Answers = [new Answer { Ball = 1, Selected = true }]
            }
        ];

        QuestionViewModel viewModel = CreateViewModel(
            questions,
            navigationService,
            toast.Object,
            dialog.Object,
            progress.Object);

        viewModel.PreviousCommand.Execute(null);
        await Task.Delay(50);

        navigation.Verify(n => n.PopAsync(false), Times.Once);
    }

    private static QuestionViewModel CreateViewModel(
        List<Question> questions,
        INavigationService navigationService,
        IToastService toast,
        IDialogService dialog,
        IUserProgressService progress,
        TestSessionInfo? session = null) =>
        new(
            questions,
            score => $"Score {score}",
            singleAnswer: true,
            toast,
            dialog,
            navigationService,
            progress,
            SubmissionService,
            RunCoordinator,
            CatalogService.Object,
            Logger,
            session);

    private sealed class TestResultTrackingNavigationService(INavigation navigation) : TestNavigationService(navigation)
    {
        public int? LastScore { get; private set; }
        public string? LastInterpretation { get; private set; }
        public TechniqueId? LastRecommendedTechnique { get; private set; }

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
            LastScore = score;
            LastInterpretation = interpretation;
            LastRecommendedTechnique = recommendedTechnique;
            return Task.FromResult(NavigationRunStatus.Completed);
        }
    }

    private sealed class RetryResultNavigationService(INavigation navigation) : TestNavigationService(navigation)
    {
        private int _attempts;

        public int NavigationAttempts => _attempts;

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
            _attempts++;
            return Task.FromResult(_attempts == 1 ? NavigationRunStatus.Failed : NavigationRunStatus.Completed);
        }
    }

    private sealed class FailingResultNavigationService(INavigation navigation) : TestNavigationService(navigation)
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

    private sealed class ThrowingResultNavigationService(INavigation navigation) : TestNavigationService(navigation)
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
