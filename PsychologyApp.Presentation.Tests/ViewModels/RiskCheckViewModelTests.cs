using Moq;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Pages.ClinicalCare.RiskCheck;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

public sealed class RiskCheckViewModelTests
{
    private readonly Mock<INavigationService> _navigation = new();
    private readonly Mock<IClinicalCareService> _care = new();
    private RiskAssessmentInput? _sent;

    public RiskCheckViewModelTests()
    {
        _navigation.Setup(n => n.GoBackAsync()).Returns(Task.CompletedTask);
        _navigation.Setup(n => n.GoToCrisisHubAsync()).Returns(Task.CompletedTask);
    }

    private RiskCheckViewModel Create(RiskLevel result, string source = "startup", Func<RiskAssessmentDTO, Task>? onCompleted = null)
    {
        _care.Setup(c => c.AssessRiskAsync(It.IsAny<RiskAssessmentInput>(), It.IsAny<CancellationToken>()))
            .Callback<RiskAssessmentInput, CancellationToken>((input, _) => _sent = input)
            .ReturnsAsync(new RiskAssessmentDTO { RiskLevel = result });
        return new RiskCheckViewModel(_navigation.Object, _care.Object, source, onCompleted);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData("true", true)]
    [InlineData("yes", true)]
    [InlineData("no", false)]
    [InlineData(false, false)]
    [InlineData(null, false)]
    public void AnAnswerIsParsedTheSameWayEverywhere(object? answer, bool expected)
    {
        RiskCheckViewModel viewModel = Create(RiskLevel.Green);

        viewModel.SetSelfHarmCommand.Execute(answer);

        Assert.Equal(expected, viewModel.IsSelfHarmYes);
        Assert.Equal(expected, viewModel.ShowOpenHelpNow);
    }

    [Fact]
    public void TheHelpNowShortcutAppearsOnlyForThoughtsOfSelfHarm()
    {
        RiskCheckViewModel viewModel = Create(RiskLevel.Green);

        viewModel.SetSubstanceCommand.Execute(true);
        viewModel.SetInsomniaCommand.Execute(true);
        viewModel.SetDisorientationCommand.Execute(true);
        Assert.False(viewModel.ShowOpenHelpNow);

        viewModel.SetSelfHarmCommand.Execute(true);
        Assert.True(viewModel.ShowOpenHelpNow);
    }

    [Fact]
    public async Task SubmittingSendsEveryAnswerAndTheSource()
    {
        RiskCheckViewModel viewModel = Create(RiskLevel.Green, source: "weekly");
        viewModel.SetSelfHarmCommand.Execute(false);
        viewModel.SetDisorientationCommand.Execute(true);
        viewModel.SetSubstanceCommand.Execute(false);
        viewModel.SetInsomniaCommand.Execute(true);

        await VmTestHelpers.RunAsync(viewModel.SubmitCommand);

        Assert.NotNull(_sent);
        Assert.False(_sent!.HasSelfHarmThoughts);
        Assert.True(_sent.HasSevereDisorientation);
        Assert.False(_sent.HasSubstanceRisk);
        Assert.True(_sent.HasSevereInsomnia);
        Assert.Equal("weekly", _sent.Source);
    }

    [Fact]
    public async Task UnansweredQuestionsCountAsNoAndABlankSourceBecomesManual()
    {
        RiskCheckViewModel viewModel = Create(RiskLevel.Green, source: " ");

        await VmTestHelpers.RunAsync(viewModel.SubmitCommand);

        Assert.False(_sent!.HasSelfHarmThoughts);
        Assert.Equal(AppStrings.RiskCheckSourceManual, _sent.Source);
    }

    [Theory]
    [InlineData(RiskLevel.Red, true)]
    [InlineData(RiskLevel.Amber, true)]
    [InlineData(RiskLevel.Green, false)]
    public async Task OnlyAnElevatedResultOpensTheCrisisScreenAfterGoingBack(RiskLevel result, bool opensCrisisHub)
    {
        RiskCheckViewModel viewModel = Create(result);
        List<string> calls = [];
        _navigation.Setup(n => n.GoBackAsync()).Callback(() => calls.Add("back")).Returns(Task.CompletedTask);
        _navigation.Setup(n => n.GoToCrisisHubAsync()).Callback(() => calls.Add("crisis")).Returns(Task.CompletedTask);

        await VmTestHelpers.RunAsync(viewModel.SubmitCommand);

        Assert.Equal(opensCrisisHub ? ["back", "crisis"] : ["back"], calls);
    }

    [Fact]
    public async Task ACompletionHandlerTakesOverNavigation()
    {
        RiskAssessmentDTO? seen = null;
        RiskCheckViewModel viewModel = Create(RiskLevel.Red, onCompleted: a =>
        {
            seen = a;
            return Task.CompletedTask;
        });

        await VmTestHelpers.RunAsync(viewModel.SubmitCommand);

        Assert.Equal(RiskLevel.Red, seen!.RiskLevel);
        _navigation.Verify(n => n.GoBackAsync(), Times.Never);
        _navigation.Verify(n => n.GoToCrisisHubAsync(), Times.Never);
    }

    [Fact]
    public async Task TheHelpNowButtonOpensTheCrisisScreenWithoutSubmitting()
    {
        RiskCheckViewModel viewModel = Create(RiskLevel.Green);

        await VmTestHelpers.RunAsync(viewModel.OpenHelpNowCommand);

        _navigation.Verify(n => n.GoToCrisisHubAsync(), Times.Once);
        _care.Verify(c => c.AssessRiskAsync(It.IsAny<RiskAssessmentInput>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
