using Moq;
using PsychologyApp.Presentation.App.Routes;
using PsychologyApp.Presentation.Pages.StartPhysics;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class StartPhysicsViewModelTests
{
    [Fact]
    public async Task StartCommand_NavigatesToPhysicsSearch()
    {
        Mock<INavigationService> navigation = new();
        navigation.Setup(n => n.GoToPhysicsSearchAsync()).Returns(Task.CompletedTask);

        StartPhysicsViewModel viewModel = new(navigation.Object);
        viewModel.StartCommand.Execute(null);

        await Task.Delay(50);

        navigation.Verify(n => n.GoToPhysicsSearchAsync(), Times.Once);
    }
}
