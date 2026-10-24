using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Pages.RunTests.TestsList;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("Localization")]
public sealed class TestsListViewModelTests
{
    public TestsListViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task A_short_luscher_result_reads_in_the_current_language()
    {
        // Saved in Chinese, the summary stayed "蓝色 / 红色" on the German list; the stored colour codes are named anew.
        var progress = new Mock<IUserProgressService>();
        var catalog = new FakeTestCatalogService().WithCatalog(new TestDefinition
        {
            TestId = "luscher_brief",
            Title = "Lüscher",
            Subtitle = "Sub",
            Description = "Desc",
            Comment = "Note",
            Algorithm = ["Step"],
            Kind = TestKind.Questionnaire,
            Questions = [],
            SingleAnswer = true
        });
        progress
            .Setup(p => p.GetLatestTestResultsAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, TestResultDTO>(StringComparer.Ordinal)
            {
                ["luscher_brief"] = new TestResultDTO
                {
                    TestId = "luscher_brief",
                    Summary = "蓝色 / 红色",
                    DetailJson = "{\"first\":{\"code\":\"#0000FF\",\"name\":\"蓝色\"},\"second\":{\"code\":\"#FF0000\",\"name\":\"红色\"}}",
                    CompletedAt = DateTime.UtcNow
                }
            });
        progress
            .Setup(p => p.GetTestResultCountsAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, int>(StringComparer.Ordinal));

        AppStrings.LanguageOverride = "de";
        TestsListViewModel viewModel = new(
            new TestNavigationService(new Mock<INavigation>().Object),
            TestDatabaseReady.CreateSignaled(),
            TestRunTestHelpers.CreateTestsListLoader(progress.Object, catalog),
            Options.Create(new AppSettings()),
            NullLogger<TestsListViewModel>.Instance);
        await viewModel.EnsureInitializedAsync();
        string? summary = viewModel.TestItemCollection[0].LastResultSummary;
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;

        Assert.Contains("Blau / Rot", summary);
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
            SingleAnswer = true,
            ScoreDirection = ScoreDirection.LowerIsBetter
        });

        progress
            .Setup(p => p.GetLatestTestResultsAsync(It.Is<IReadOnlyList<string>>(ids => ids.Contains("beck")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, TestResultDTO>(StringComparer.Ordinal)
            {
                ["beck"] = new TestResultDTO { TestId = "beck", Score = 5, Summary = "Mild", CompletedAt = DateTime.UtcNow }
            });
        progress
            .Setup(p => p.GetTestResultCountsAsync(It.Is<IReadOnlyList<string>>(ids => ids.Contains("beck")), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, int>(StringComparer.Ordinal) { ["beck"] = 2 });

        TestsListViewModel viewModel = new(
            navigationService,
            TestDatabaseReady.CreateSignaled(),
            TestRunTestHelpers.CreateTestsListLoader(progress.Object, catalog),
            Options.Create(new AppSettings()),
            NullLogger<TestsListViewModel>.Instance);

        await viewModel.EnsureInitializedAsync();
        await viewModel.EnsureInitializedAsync();

        Assert.True(viewModel.HasInitialized);
        Assert.Single(viewModel.TestItemCollection);
        Assert.Equal("Beck", viewModel.TestItemCollection[0].Title);
        Assert.True(viewModel.TestItemCollection[0].HasLastResult);
        Assert.True(viewModel.TestItemCollection[0].HasMultipleResults);

        progress.Verify(p => p.GetLatestTestResultsAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()), Times.Once);
        progress.Verify(p => p.GetTestResultCountsAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()), Times.Once);
        progress.Verify(p => p.GetLatestTestResultAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        progress.Verify(p => p.GetTestResultHistoryAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
