using Moq;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Pages.ClinicalCare.CrisisHub;
using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class CrisisHubViewModelTests
{
    [Fact]
    public async Task ContinueSoft_GoesBackThenOpensPracticeTab()
    {
        Mock<INavigationService> navigation = new(MockBehavior.Strict);
        var callOrder = new List<string>();
        navigation.Setup(n => n.GoBackAsync()).Returns(() =>
        {
            callOrder.Add("back");
            return Task.CompletedTask;
        });
        navigation.Setup(n => n.GoToPracticeTabAsync()).Returns(() =>
        {
            callOrder.Add("practice");
            return Task.CompletedTask;
        });
        navigation.Setup(n => n.GoToRiskCheckAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        Mock<IClinicalCareService> clinical = new();
        clinical.Setup(c => c.GetLatestRiskAssessmentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((RiskAssessmentDTO?)null);

        var viewModel = new CrisisHubViewModel(navigation.Object, clinical.Object);
        await Task.Delay(50);

        Assert.True(viewModel.ContinueSoftCommand.CanExecute(null));
        viewModel.ContinueSoftCommand.Execute(null);
        await Task.Delay(50);

        Assert.Equal(["back", "practice"], callOrder);
        navigation.Verify(n => n.GoBackAsync(), Times.Once);
        navigation.Verify(n => n.GoToPracticeTabAsync(), Times.Once);
    }

    [Fact]
    public async Task Recheck_OpensManualRiskCheck()
    {
        Mock<INavigationService> navigation = new();
        navigation.Setup(n => n.GoToRiskCheckAsync(AppStrings.RiskCheckSourceManual)).Returns(Task.CompletedTask);

        Mock<IClinicalCareService> clinical = new();
        clinical.Setup(c => c.GetLatestRiskAssessmentAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((RiskAssessmentDTO?)null);

        var viewModel = new CrisisHubViewModel(navigation.Object, clinical.Object);
        await Task.Delay(50);

        viewModel.RecheckCommand.Execute(null);
        await Task.Delay(50);

        navigation.Verify(n => n.GoToRiskCheckAsync(AppStrings.RiskCheckSourceManual), Times.Once);
    }
}
