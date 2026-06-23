using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Pages.TestsList;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TestsListViewModelTests
{
    public TestsListViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task InitAsync_LoadsCatalogAndEnrichesLatestResult()
    {
        var navigation = new Mock<INavigation>();
        var navigationService = new TestNavigationService(navigation.Object);
        var progress = new Mock<IUserProgressService>();
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
            Questions = [],
            SingleAnswer = true
        });

        progress
            .Setup(p => p.GetLatestTestResultAsync("beck", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TestResultDTO { TestId = "beck", Score = 5, Summary = "Mild", CompletedAt = DateTime.UtcNow });
        progress
            .Setup(p => p.GetTestResultHistoryAsync("beck", 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestResultDTO>
            {
                new() { TestId = "beck", Score = 5, Summary = "Mild", CompletedAt = DateTime.UtcNow },
                new() { TestId = "beck", Score = 3, Summary = "Low", CompletedAt = DateTime.UtcNow.AddDays(-1) }
            });

        TestsListViewModel viewModel = new(
            navigationService,
            TestDatabaseReady.CreateSignaled(),
            new TestsListLoader(progress.Object, catalog),
            NullLogger<TestsListViewModel>.Instance);

        await viewModel.InitAsync();

        Assert.Single(viewModel.TestItemCollection);
        Assert.Equal("Beck", viewModel.TestItemCollection[0].Title);
        Assert.True(viewModel.TestItemCollection[0].HasLastResult);
        Assert.True(viewModel.TestItemCollection[0].HasMultipleResults);
    }
}
