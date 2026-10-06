using Microsoft.Maui.ApplicationModel;
using Moq;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Presentation.Pages.ClinicalCare.CrisisHub;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>The crisis screen is the one a person in distress lands on, so its behaviour is pinned down here.</summary>
[Collection("StaticShims")]
public sealed class CrisisHubViewModelTests
{
    private readonly Mock<INavigationService> _navigation = new();
    private readonly Mock<IClinicalCareService> _care = new();

    public CrisisHubViewModelTests()
    {
        Launcher.Opened.Clear();
        Launcher.FailWith = null;
        Browser.Opened.Clear();
        _navigation.Setup(n => n.GoBackAsync()).Returns(Task.CompletedTask);
        _navigation.Setup(n => n.GoToPracticeTabAsync()).Returns(Task.CompletedTask);
        _navigation.Setup(n => n.GoToRiskCheckAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _navigation.Setup(n => n.GoToSafetyPlanAsync()).Returns(Task.CompletedTask);
    }

    private async Task<CrisisHubViewModel> CreateAsync(bool red)
    {
        _care.Setup(c => c.ShouldRouteToCrisisHubAsync(It.IsAny<CancellationToken>())).ReturnsAsync(red);
        CrisisHubViewModel viewModel = new(_navigation.Object, _care.Object);
        await VmTestHelpers.WaitUntilAsync(() => _care.Invocations.Count > 0);
        await Task.Delay(30);
        return viewModel;
    }

    [Fact]
    public async Task ACurrentRedHidesTheShortcutBackToPractices()
    {
        CrisisHubViewModel viewModel = await CreateAsync(red: true);

        Assert.True(viewModel.IsRed);
        Assert.False(viewModel.ShowContinueSoft);
    }

    [Fact]
    public async Task WithoutARedTheShortcutBackIsOffered()
    {
        CrisisHubViewModel viewModel = await CreateAsync(red: false);

        Assert.False(viewModel.IsRed);
        Assert.True(viewModel.ShowContinueSoft);
    }

    [Fact]
    public async Task WhenTheRiskStateCannotBeReadTheScreenFailsClosed()
    {
        _care.Setup(c => c.ShouldRouteToCrisisHubAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("db"));
        CrisisHubViewModel viewModel = new(_navigation.Object, _care.Object);

        await VmTestHelpers.WaitUntilAsync(() => viewModel.IsRed);

        Assert.False(viewModel.ShowContinueSoft);
    }

    [Fact]
    public async Task TheDialButtonsCallTheRussianHelplineAndTheEmergencyNumber()
    {
        CrisisHubViewModel viewModel = await CreateAsync(red: true);

        await VmTestHelpers.RunAsync(viewModel.CallHotlineRuCommand);
        await VmTestHelpers.RunAsync(viewModel.CallEmergencyCommand);

        Assert.Equal(["tel:88002000122", "tel:112"], Launcher.Opened);
    }

    [Fact]
    public async Task ADialIntentThatFailsDoesNotCrashTheScreen()
    {
        CrisisHubViewModel viewModel = await CreateAsync(red: true);
        Launcher.FailWith = new InvalidOperationException("no dialer");

        await VmTestHelpers.RunAsync(viewModel.CallEmergencyCommand);

        Assert.True(viewModel.CallEmergencyCommand.CanExecute(null));
    }

    [Fact]
    public async Task TheHelplineLinkOpensTheInternationalDirectory()
    {
        CrisisHubViewModel viewModel = await CreateAsync(red: true);

        await VmTestHelpers.RunAsync(viewModel.OpenHelplineCommand);

        Assert.Equal(["https://findahelpline.com"], Browser.Opened);
    }

    [Fact]
    public async Task ContinueGoesBackAndThenToPractices()
    {
        CrisisHubViewModel viewModel = await CreateAsync(red: false);
        List<string> calls = [];
        _navigation.Setup(n => n.GoBackAsync()).Callback(() => calls.Add("back")).Returns(Task.CompletedTask);
        _navigation.Setup(n => n.GoToPracticeTabAsync()).Callback(() => calls.Add("practice")).Returns(Task.CompletedTask);

        await VmTestHelpers.RunAsync(viewModel.ContinueSoftCommand);

        Assert.Equal(["back", "practice"], calls);
    }

    [Fact]
    public async Task RecheckOpensTheRiskCheckAsAManualCheck()
    {
        CrisisHubViewModel viewModel = await CreateAsync(red: true);

        await VmTestHelpers.RunAsync(viewModel.RecheckCommand);

        _navigation.Verify(n => n.GoToRiskCheckAsync(AppStrings.RiskCheckSourceManual), Times.Once);
    }

    [Fact]
    public async Task TheSafetyPlanAndBackCommandsNavigate()
    {
        CrisisHubViewModel viewModel = await CreateAsync(red: true);

        await VmTestHelpers.RunAsync(viewModel.OpenSafetyPlanCommand);
        await VmTestHelpers.RunAsync(viewModel.BackCommand);

        _navigation.Verify(n => n.GoToSafetyPlanAsync(), Times.Once);
        _navigation.Verify(n => n.GoBackAsync(), Times.Once);
    }

    [Fact]
    public async Task ThePageTextIsNotEmptyInBothLanguages()
    {
        string? previous = AppStrings.LanguageOverride;
        try
        {
            foreach (string language in new[] { "ru", "en" })
            {
                AppStrings.LanguageOverride = language;
                CrisisHubViewModel viewModel = await CreateAsync(red: true);
                foreach (string text in new[] { viewModel.PageTitle, viewModel.LeadText, viewModel.HotlineRu, viewModel.CallEmergencyText, viewModel.SpecialistHint })
                {
                    Assert.False(string.IsNullOrWhiteSpace(text), language);
                }
            }
        }
        finally
        {
            AppStrings.LanguageOverride = previous;
        }
    }
}
