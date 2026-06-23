using Moq;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Pages.TestsList;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class LuscherTestViewModelTests
{
    public LuscherTestViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task StandardMode_AfterEightSelections_PersistsWithStandardTestId()
    {
        var navigation = new Mock<INavigation>();
        var progress = new Mock<IUserProgressService>();
        progress
            .Setup(p => p.SaveTestResultAsync(TestIds.LuscherStandard, It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        LuscherTestViewModel viewModel = new(
            LuscherMode.Standard,
            new TestNavigationService(navigation.Object),
            progress.Object,
            new LuscherTestSubmissionService());

        ColourValue[] colors =
        [
            ColourValue.Red,
            ColourValue.Blue,
            ColourValue.Green,
            ColourValue.Yellow,
            ColourValue.Purple,
            ColourValue.Brown,
            ColourValue.Gray,
            ColourValue.Black
        ];

        foreach (ColourValue color in colors)
        {
            InvokeColorHandler(viewModel, color);
        }

        Assert.True(viewModel.IsFinish);
        Assert.Equal(2, viewModel.ResultItems.Count);
        await Task.Delay(50);

        progress.Verify(
            p => p.SaveTestResultAsync(
                TestIds.LuscherStandard,
                It.IsAny<int?>(),
                It.Is<string>(summary => summary.Contains(':') || summary.Length > 0),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task BriefMode_AfterTwoSelections_PersistsWithBriefTestId()
    {
        var navigation = new Mock<INavigation>();
        var progress = new Mock<IUserProgressService>();
        progress
            .Setup(p => p.SaveTestResultAsync(TestIds.LuscherBrief, null, It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        LuscherTestViewModel viewModel = new(
            LuscherMode.Brief,
            new TestNavigationService(navigation.Object),
            progress.Object,
            new LuscherTestSubmissionService());

        InvokeColorHandler(viewModel, ColourValue.Red);
        InvokeColorHandler(viewModel, ColourValue.Blue);

        Assert.True(viewModel.IsFinish);
        await Task.Delay(50);

        progress.Verify(
            p => p.SaveTestResultAsync(
                TestIds.LuscherBrief,
                null,
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static void InvokeColorHandler(LuscherTestViewModel viewModel, ColourValue color)
    {
        switch (color.Code)
        {
            case "#FF0000":
                viewModel.RedHandler!.Execute(null);
                break;
            case "#0000FF":
                viewModel.BlueHandler!.Execute(null);
                break;
            case "#00FF00":
                viewModel.GreenHandler!.Execute(null);
                break;
            case "#FFFF00":
                viewModel.YellowHandler!.Execute(null);
                break;
            case "#964B00":
                viewModel.BrownHandler!.Execute(null);
                break;
            case "#888888":
                viewModel.GrayHandler!.Execute(null);
                break;
            case "#FF00FF":
                viewModel.PurpleHandler!.Execute(null);
                break;
            case "#000000":
                viewModel.BlackHandler!.Execute(null);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, "Unsupported test color.");
        }
    }
}
