using Moq;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using PsychologyApp.Application.UserProgress;
using Xunit;

namespace PsychologyApp.Application.Tests.Tests;

public sealed class LuscherResultServiceTests
{
    private readonly LuscherResultService _service = new();

    [Fact]
    public async Task SaveStandardAsync_PersistsCoBkAndColorJson()
    {
        Mock<IUserProgressService> progress = new();
        progress
            .Setup(p => p.SaveTestResultAsync(TestIds.LuscherStandard, It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _service.SaveStandardAsync(
            progress.Object,
            "CO: 12; BK: 1.5",
            coValue: 12,
            bkValue: 1.5,
            [new LuscherColorSelection("#FF0000", "Red")],
            [new LuscherColorSelection("#0000FF", "Blue")]);

        progress.Verify(
            p => p.SaveTestResultAsync(
                TestIds.LuscherStandard,
                12,
                "CO: 12; BK: 1.5",
                It.Is<string?>(json =>
                    json!.Contains("\"co\":12") &&
                    json.Contains("\"firstPassColors\"") &&
                    json.Contains("\"code\":\"#FF0000\"") &&
                    json.Contains("\"code\":\"#0000FF\"")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveBriefAsync_PersistsTwoColorSummary()
    {
        Mock<IUserProgressService> progress = new();

        await _service.SaveBriefAsync(
            progress.Object,
            "Red / Blue",
            "Red",
            "Blue",
            "#FF0000",
            "#0000FF",
            "First text",
            "Second text");

        progress.Verify(
            p => p.SaveTestResultAsync(
                TestIds.LuscherBrief,
                null,
                "Red / Blue",
                It.Is<string?>(json => json!.Contains("\"first\"") && json.Contains("\"second\"") && json.Contains("\"code\"")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
