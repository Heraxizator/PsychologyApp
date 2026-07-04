using Moq;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Quot;
using DomainQuot = PsychologyApp.Domain.Entities.Quot;
using Xunit;

namespace PsychologyApp.Application.Tests.Quot;

public sealed class QuotServiceReseedTests
{
    [Fact]
    public async Task ReseedFeedAsync_DeletesAllAndAddsRequestedCount()
    {
        IReadOnlyList<QuotSeed> seeds =
        [
            new QuotSeed("Seneca", "Luck is what happens when preparation meets opportunity.", "wisdom"),
            new QuotSeed("Marcus Aurelius", "The obstacle is the way.", "resilience")
        ];

        var repository = new Mock<IQuotRepository>();
        repository.Setup(r => r.GetExistingTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());
        var provider = new Mock<IQuotContentProvider>();
        provider.Setup(p => p.LoadAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(seeds);

        var service = QuotServiceTestFactory.Create(repository, provider);

        await service.ReseedFeedAsync(2);

        repository.Verify(r => r.DeleteAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(
            r => r.AddManyAsync(
                It.Is<IReadOnlyList<DomainQuot>>(items => items.Count == 2),
                It.IsAny<CancellationToken>()),
            Times.Once);
        provider.Verify(p => p.LoadAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReseedFeedAsync_WhenCountIsZero_DoesNothing()
    {
        var repository = new Mock<IQuotRepository>();
        var provider = new Mock<IQuotContentProvider>();
        var service = QuotServiceTestFactory.Create(repository, provider);

        await service.ReseedFeedAsync(0);

        repository.Verify(r => r.DeleteAllAsync(It.IsAny<CancellationToken>()), Times.Never);
        provider.Verify(p => p.LoadAllAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
