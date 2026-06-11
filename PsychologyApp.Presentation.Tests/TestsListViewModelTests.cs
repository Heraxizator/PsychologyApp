using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.ViewModels.Tests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class TestsListViewModelTests
{
    public TestsListViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
        AppReadiness.SignalDatabaseReady();
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
            navigation.Object,
            navigationService,
            progress.Object,
            catalog);

        await viewModel.InitAsync();

        Assert.Single(viewModel.TestItemCollection);
        Assert.Equal("Beck", viewModel.TestItemCollection[0].Title);
        Assert.True(viewModel.TestItemCollection[0].HasLastResult);
        Assert.True(viewModel.TestItemCollection[0].HasMultipleResults);
    }
}
