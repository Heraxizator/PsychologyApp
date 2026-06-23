using Moq;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class LuscherTestSubmissionServiceTests
{
    public LuscherTestSubmissionServiceTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task SaveStandardAsync_PersistsCoBkAndColorJson()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.SaveTestResultAsync(TestIds.LuscherStandard, It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        LuscherTestSubmissionService service = new();
        (ColourValue Colour, ColourMeaning Meaning)[] colors =
        [
            (ColourValue.Red, ColourMeaning.RedVoted),
            (ColourValue.Blue, ColourMeaning.BlueVoted)
        ];

        await service.SaveStandardAsync(progress.Object, coValue: 12, bkValue: 1.5, colors);

        progress.Verify(
            p => p.SaveTestResultAsync(
                TestIds.LuscherStandard,
                12,
                It.Is<string>(summary => summary.Contains(AppStrings.TestsCoLabel) && summary.Contains(AppStrings.TestsBkLabel)),
                It.Is<string?>(json => json!.Contains("\"co\":12") && json.Contains("\"code\":\"#FF0000\"")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveBriefAsync_PersistsTwoColorSummary()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.SaveTestResultAsync(TestIds.LuscherBrief, null, It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        LuscherTestSubmissionService service = new();

        await service.SaveBriefAsync(progress.Object, "Red", "Blue", "First text", "Second text");

        progress.Verify(
            p => p.SaveTestResultAsync(
                TestIds.LuscherBrief,
                null,
                "Red / Blue",
                It.Is<string?>(json => json!.Contains("\"first\"") && json.Contains("\"second\"")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveStandardAsync_SkipsWhenProgressNull()
    {
        LuscherTestSubmissionService service = new();

        await service.SaveStandardAsync(null, 1, 1.0, [(ColourValue.Red, ColourMeaning.RedVoted)]);
    }
}
