using System.Reflection;
using Moq;
using PsychologyApp.Presentation.ViewModels;
using PsychologyApp.Presentation.ViewModels.Tests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class FindProblemViewModelTests
{
    [Fact]
    public void ToContinue_InvokesConfiguredAction()
    {
        bool invoked = false;
        FindProblemViewModel viewModel = new(
            Mock.Of<INavigation>(),
            new TestNavigationService(Mock.Of<INavigation>()),
            new FakeTestCatalogService(),
            "Description",
            ["Step 1", "Step 2"],
            "Comment",
            () => invoked = true);

        viewModel.ToContinue();

        Assert.True(invoked);
    }

    [Fact]
    public void Constructor_InitializesAlgorithmRows()
    {
        FindProblemViewModel viewModel = new(
            Mock.Of<INavigation>(),
            new TestNavigationService(Mock.Of<INavigation>()),
            new FakeTestCatalogService(),
            "Description",
            ["Alpha", "Beta"],
            null,
            () => { });

        Assert.Equal(2, viewModel.AlgorithmRows.Count);
        Assert.Equal("Alpha", viewModel.AlgorithmRows[0].Text);
        Assert.Equal("Beta", viewModel.AlgorithmRows[1].Text);
    }

    [Fact]
    public async Task ReloadLocalizedTestContent_UsesGetByIdInsteadOfFullCatalog()
    {
        var catalog = new FakeTestCatalogService().WithCatalog(new PsychologyApp.Presentation.Models.Tests.TestDefinition
        {
            TestId = "beck",
            Title = "Updated",
            Subtitle = "Sub",
            Description = "Updated description",
            Comment = "Updated comment",
            Algorithm = ["New step"],
            Kind = PsychologyApp.Presentation.Models.Tests.TestKind.Questionnaire,
            AnalyzerId = "beck"
        });

        FindProblemViewModel viewModel = new(
            Mock.Of<INavigation>(),
            new TestNavigationService(Mock.Of<INavigation>()),
            catalog,
            "Old",
            ["Old step"],
            "Old comment",
            () => { },
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
}
