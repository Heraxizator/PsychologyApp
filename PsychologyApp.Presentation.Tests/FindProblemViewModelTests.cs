using System.Reflection;
using Moq;
using PsychologyApp.Presentation.ViewModels;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services.Tests;
using PsychologyApp.Presentation.ViewModels.Tests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class FindProblemViewModelTests
{
    [Fact]
    public async Task Continue_InvokesConfiguredStartAsync()
    {
        bool invoked = false;
        FindProblemViewModel viewModel = new(
            new TestNavigationService(Mock.Of<INavigation>()),
            new FakeTestCatalogService(),
            new FindProblemContentLoader(),
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
        FindProblemViewModel viewModel = new(
            new TestNavigationService(Mock.Of<INavigation>()),
            new FakeTestCatalogService(),
            new FindProblemContentLoader(),
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
            new TestNavigationService(Mock.Of<INavigation>()),
            catalog,
            new FindProblemContentLoader(),
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
}
