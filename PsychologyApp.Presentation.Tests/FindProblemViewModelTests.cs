using Moq;
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
            "Description",
            ["Alpha", "Beta"],
            null,
            () => { });

        Assert.Equal(2, viewModel.AlgorithmRows.Count);
        Assert.Equal("Alpha", viewModel.AlgorithmRows[0].Text);
        Assert.Equal("Beta", viewModel.AlgorithmRows[1].Text);
    }
}
