using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PsychologyApp.Application.UserProgress;
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
            new TestHistoryLoader(),
            new TestRetakeOperations(),
            NullLogger<TestHistoryViewModel>.Instance,
            "gad7",
            "GAD-7");

        viewModel.RetakeCommand.Execute(null);
        await Task.Delay(200);

        Assert.True(navigationService.WentToRoot);
        Assert.True(navigationService.StartedLuscherStandard);
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
