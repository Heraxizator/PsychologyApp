using Moq;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using Xunit;

namespace PsychologyApp.Application.Tests.Tests;

public sealed class CachedTestCatalogProviderTests
{
    [Fact]
    public async Task LoadAllAsync_LoadsInnerProviderOnlyOnceUntilInvalidated()
    {
        var inner = new Mock<ITestCatalogProvider>();
        inner.Setup(p => p.LoadAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateDefinition("beck")]);

        var cache = new CachedTestCatalogProvider(inner.Object);

        IReadOnlyList<TestDefinition> first = await cache.LoadAllAsync();
        IReadOnlyList<TestDefinition> second = await cache.LoadAllAsync();

        Assert.Single(first);
        Assert.Single(second);
        inner.Verify(p => p.LoadAllAsync(It.IsAny<CancellationToken>()), Times.Once);

        cache.Invalidate();
        await cache.LoadAllAsync();
        inner.Verify(p => p.LoadAllAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Invalidate_ForcesReload()
    {
        var inner = new Mock<ITestCatalogProvider>();
        inner.SetupSequence(p => p.LoadAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([CreateDefinition("beck")])
            .ReturnsAsync([CreateDefinition("gad7")]);

        var cache = new CachedTestCatalogProvider(inner.Object);
        Assert.Equal("beck", (await cache.LoadAllAsync())[0].TestId);

        cache.Invalidate();
        Assert.Equal("gad7", (await cache.LoadAllAsync())[0].TestId);
    }

    private static TestDefinition CreateDefinition(string testId) => new()
    {
        TestId = testId,
        Title = testId,
        Subtitle = "Sub",
        Description = "Desc",
        Comment = "Note",
        Algorithm = ["Step"],
        Kind = TestKind.Questionnaire,
        AnalyzerId = testId
    };
}
