using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Features.RunTests;
using System.Text.Json;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class ListSessionDraftCoordinatorTests
{
    [Fact]
    public async Task LoadAsync_RestoresSavedItems()
    {
        string key = TechniqueId.Paper.ToString();
        string json = JsonSerializer.Serialize(new PaperListDraft
        {
            Items = [new PaperListDraftItem { Id = "1", Text = "Draft thought" }]
        });
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetSessionDraftAsync(key, It.IsAny<CancellationToken>())).ReturnsAsync(json);
        PaperListDraftCoordinator coordinator = new(NullLogger<PaperListDraftCoordinator>.Instance);
        coordinator.Attach(key, progress.Object);
        List<Paper> papers = [];

        await coordinator.LoadAsync(
            papers,
            item => new Paper { Id = item.Id, Text = item.Text },
            _ => { });

        Assert.Single(papers);
        Assert.Equal("Draft thought", papers[0].Text);
    }

    [Fact]
    public async Task SaveAsync_PersistsMappedItems()
    {
        string key = TechniqueId.Polarity.ToString();
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.SaveSessionDraftAsync(key, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        PolarityListDraftCoordinator coordinator = new(NullLogger<PolarityListDraftCoordinator>.Instance);
        coordinator.Attach(key, progress.Object);
        List<Polarity> polarities =
        [
            new Polarity { Id = "1", Positive = "Calm", Negative = "Anxious" }
        ];

        await coordinator.SaveAsync(
            polarities,
            item => new PolarityListDraftItem
            {
                Id = item.Id,
                Positive = item.Positive,
                Negative = item.Negative
            });

        progress.Verify(
            p => p.SaveSessionDraftAsync(
                key,
                It.Is<string>(payload => payload.Contains("Calm", StringComparison.Ordinal) && payload.Contains("Anxious", StringComparison.Ordinal)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

public sealed class TestModuleOperationsTests
{
    [Fact]
    public async Task RetakeAsync_GoesToRootAndStartsTest()
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
        TestRetakeOperations operations = new();

        await operations.RetakeAsync("gad7", catalog, navigationService);

        Assert.True(navigationService.WentToRoot);
        Assert.True(navigationService.StartedLuscherStandard);
    }

    [Fact]
    public async Task TestHistoryLoader_MapsEntriesAndTitle()
    {
        Mock<IUserProgressService> progress = new();
        DateTime completedAt = new(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        progress
            .Setup(p => p.GetTestResultHistoryAsync("beck", 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestResultDTO>
            {
                new() { TestId = "beck", Summary = "Mild", CompletedAt = completedAt }
            });
        FakeTestCatalogService catalog = new FakeTestCatalogService().WithCatalog(new TestDefinition
        {
            TestId = "beck",
            Title = "Beck Inventory",
            Subtitle = "Sub",
            Description = "Desc",
            Comment = "Note",
            Algorithm = ["Step"],
            Kind = TestKind.Questionnaire
        });
        TestHistoryLoader loader = new();

        TestHistoryLoadResult result = await loader.LoadEntriesAsync(
            "beck",
            "Fallback",
            progress.Object,
            catalog);

        Assert.Equal("Beck Inventory", result.Title);
        Assert.Single(result.Entries);
        Assert.Equal("Mild", result.Entries[0].SummaryText);
    }

    [Fact]
    public async Task TestsListLoader_SetsHasMultipleResults()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.GetLatestTestResultAsync("beck", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestResultDTO { Summary = "Mild" });
        progress
            .Setup(p => p.GetTestResultHistoryAsync("beck", 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestResultDTO>
            {
                new() { Summary = "Mild" },
                new() { Summary = "Low" }
            });
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
            Questions = [],
            SingleAnswer = true
        });
        Mock<INavigation> navigation = new();
        TestNavigationService navigationService = new(navigation.Object);
        TestsListLoader loader = new(progress.Object, catalog);

        IReadOnlyList<TestItem> items = await loader.LoadItemsAsync(
            navigationService,
            _ => Task.CompletedTask);

        Assert.Single(items);
        Assert.True(items[0].HasMultipleResults);
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
