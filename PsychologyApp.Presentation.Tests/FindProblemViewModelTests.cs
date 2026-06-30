using System.Reflection;
using Moq;
using PsychologyApp.Presentation.Shared.ViewModels;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Pages.FindProblem;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class FindProblemViewModelTests
{
    [Fact]
    public async Task Continue_InvokesConfiguredStartAsync()
    {
        bool invoked = false;
        FakeTestCatalogService catalog = new();
        FindProblemViewModel viewModel = new(
            new TestNavigationService(Mock.Of<INavigation>()),
            catalog,
            new FindProblemContentLoader(),
            TestRunTestHelpers.CreateCoordinator(catalog),
            "Description",
            ["Step 1", "Step 2"],
            "Comment",
            () =>
            {
                invoked = true;
                return Task.CompletedTask;
            });

        viewModel.Continue.Execute(null);
        await Task.Delay(50);

        Assert.True(invoked);
    }

    [Fact]
    public void Constructor_InitializesAlgorithmRows()
    {
        FakeTestCatalogService catalog = new();
        FindProblemViewModel viewModel = new(
            new TestNavigationService(Mock.Of<INavigation>()),
            catalog,
            new FindProblemContentLoader(),
            TestRunTestHelpers.CreateCoordinator(catalog),
            "Description",
            ["Alpha", "Beta"],
            null,
            () => Task.CompletedTask);

        Assert.Equal(2, viewModel.AlgorithmRows.Count);
        Assert.Equal("Alpha", viewModel.AlgorithmRows[0].Text);
        Assert.Equal("Beta", viewModel.AlgorithmRows[1].Text);
    }

    [Fact]
    public async Task ReloadLocalizedTestContent_UsesGetByIdInsteadOfFullCatalog()
    {
        var catalog = new FakeTestCatalogService().WithCatalog(new TestDefinition
        {
            TestId = "beck",
            Title = "Updated",
            Subtitle = "Sub",
            Description = "Updated description",
            Comment = "Updated comment",
            Algorithm = ["New step"],
            Kind = TestKind.Questionnaire,
            AnalyzerId = "beck"
        });

        FindProblemViewModel viewModel = new(
            new TestNavigationService(Mock.Of<INavigation>()),
            catalog,
            new FindProblemContentLoader(),
            TestRunTestHelpers.CreateCoordinator(catalog),
            "Old",
            ["Old step"],
            "Old comment",
            () => Task.CompletedTask,
            "beck");

        typeof(BaseViewModel).GetMethod(
                "RefreshLocalizedProperties",
                BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(viewModel, null);
        await Task.Delay(100);

        Assert.Equal(0, catalog.GetCatalogCallCount);
        Assert.True(catalog.GetByIdCallCount >= 1);
        Assert.Equal("Updated description", viewModel.DescriptionText);
        Assert.Equal("Updated comment", viewModel.CommentText);
        Assert.Single(viewModel.AlgorithmRows);
        Assert.Equal("New step", viewModel.AlgorithmRows[0].Text);
    }

    [Fact]
    public async Task Continue_WithTestId_StartsQuestionnaireViaNavigationService()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new RecordingQuestionnaireNavigation(navigation.Object);
        var catalog = new FakeTestCatalogService().WithCatalog(new TestDefinition
        {
            TestId = "beck",
            Title = "Beck",
            Subtitle = "Sub",
            Description = "Desc",
            Comment = "Note",
            Algorithm = ["Step"],
            Kind = TestKind.Questionnaire,
            AnalyzerId = "beck",
            SingleAnswer = true,
            Questions =
            [
                new Question
                {
                    Number = 1,
                    Context = "Question 1",
                    Answers = [new Answer { Ball = 1, Text = "Yes", Selected = false }]
                }
            ]
        });

        FindProblemViewModel viewModel = new(
            navigationService,
            catalog,
            new FindProblemContentLoader(),
            TestRunTestHelpers.CreateCoordinator(catalog),
            "Desc",
            ["Step"],
            "Note",
            () => Task.FromException(new InvalidOperationException("Fallback should not run.")),
            "beck");

        viewModel.Continue.Execute(null);
        await Task.Delay(100);

        Assert.True(navigationService.QuestionPageRequested);
        Assert.Equal("beck", navigationService.LastSession?.TestId);
    }

    private sealed class RecordingQuestionnaireNavigation(INavigation navigation) : TestNavigationService(navigation)
    {
        public bool QuestionPageRequested { get; private set; }
        public TestSessionInfo? LastSession { get; private set; }

        public override Task GoToQuestionPageAsync(
            List<Question> questions,
            Func<int, string> scoreAnalyzer,
            bool singleAnswer,
            TestSessionInfo? session = null)
        {
            QuestionPageRequested = true;
            LastSession = session;
            return Task.CompletedTask;
        }
    }
}
