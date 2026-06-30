using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Pages.TestHistory;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Pages.TestsList;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TestHistoryViewModelTests
{
    [Fact]
    public async Task RetakeCommand_GoesToRootBeforeStartingTest()
    {
        Mock<INavigation> navigation = new();
        RetakeTrackingNavigation navigationService = new(navigation.Object);
        FakeTestCatalogService catalog = new FakeTestCatalogService().WithCatalog(new TestDefinition
        {
            TestId = "gad7",
            AnalyzerId = "gad7",
            Title = "GAD-7",
            Subtitle = "Sub",
            Description = "Desc",
            Comment = "Note",
            Algorithm = ["Step"],
            Kind = TestKind.LuscherStandard
        });

        TestHistoryViewModel viewModel = new(
            navigationService,
            new Mock<IUserProgressService>().Object,
            catalog,
            TestDatabaseReady.CreateSignaled(),
            TestRunTestHelpers.CreateHistoryLoader(catalog),
            TestRunTestHelpers.CreateRetakeOperations(catalog),
            NullLogger<TestHistoryViewModel>.Instance,
            "gad7",
            "GAD-7");

        viewModel.RetakeCommand.Execute(null);
        await Task.Delay(200);

        Assert.True(navigationService.WentToRoot);
        Assert.True(navigationService.StartedLuscherStandard);
    }

    [Fact]
    public async Task LoadAsync_SetsHasChart_WhenTwoScoredResultsExist()
    {
        Mock<IUserProgressService> progress = new();
        DateTime completedAt = new(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        progress
            .Setup(p => p.GetTestResultHistoryAsync("beck", 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestResultDTO>
            {
                new() { TestId = "beck", Score = 8, Summary = "High", CompletedAt = completedAt },
                new() { TestId = "beck", Score = 3, Summary = "Low", CompletedAt = completedAt.AddDays(-7) }
            });
        FakeTestCatalogService catalog = new FakeTestCatalogService().WithCatalog(new TestDefinition
        {
            TestId = "beck",
            Title = "Beck",
            Subtitle = "Sub",
            Description = "Desc",
            Comment = "Note",
            Algorithm = ["Step"],
            Kind = TestKind.Questionnaire
        });

        TestHistoryViewModel viewModel = new(
            new TestNavigationService(new Mock<INavigation>().Object),
            progress.Object,
            catalog,
            TestDatabaseReady.CreateSignaled(),
            TestRunTestHelpers.CreateHistoryLoader(catalog),
            TestRunTestHelpers.CreateRetakeOperations(catalog),
            NullLogger<TestHistoryViewModel>.Instance,
            "beck",
            "Beck");

        await Task.Delay(200);

        Assert.True(viewModel.HasChart);
        Assert.Equal(2, viewModel.ChartPoints.Count);
    }

    private sealed class RetakeTrackingNavigation(INavigation navigation) : TestNavigationService(navigation)
    {
        public bool WentToRoot { get; private set; }
        public bool StartedLuscherStandard { get; private set; }

        public override Task GoToRootAsync()
        {
            WentToRoot = true;
            return Task.CompletedTask;
        }

        public override Task GoToStandardTestAsync()
        {
            StartedLuscherStandard = true;
            return Task.CompletedTask;
        }
    }
}
