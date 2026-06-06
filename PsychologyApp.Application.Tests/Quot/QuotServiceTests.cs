using Moq;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Services.QuotService;
using DomainQuot = PsychologyApp.Domain.Entities.Quot;
using Xunit;

namespace PsychologyApp.Application.Tests.Quot;

public class QuotServiceTests
{
    [Fact]
    public async Task LoadSingleAsync_LoadsFromProviderAndPersistsQuot()
    {
        IReadOnlyList<QuotSeed> seeds =
        [
            new QuotSeed("Seneca", "Luck is what happens when preparation meets opportunity.", "wisdom")
        ];

        var repository = new Mock<IQuotRepository>();
        var provider = new Mock<IQuotContentProvider>();
        provider.Setup(p => p.LoadAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(seeds);

        var service = new QuotService(repository.Object, provider.Object);

        await service.LoadSingleAsync();

        provider.Verify(p => p.LoadAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(
            r => r.AddAsync(
                It.Is<DomainQuot>(q => q.Text == seeds[0].Text && q.Title == seeds[0].Author),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LoadSingleAsync_WhenCatalogEmpty_Throws()
    {
        var repository = new Mock<IQuotRepository>();
        var provider = new Mock<IQuotContentProvider>();
        provider.Setup(p => p.LoadAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<QuotSeed>());

        var service = new QuotService(repository.Object, provider.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.LoadSingleAsync());
    }

    [Fact]
    public async Task MarkAsFavouriteAsync_UpdatesQuot()
    {
        var quot = DomainQuot.Create("Author", "Text", "Theme", false, false);
        var repository = new Mock<IQuotRepository>();
        repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(quot);
        repository.Setup(r => r.EditAsync(quot, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var service = new QuotService(repository.Object, Mock.Of<IQuotContentProvider>());

        await service.MarkAsFavouriteAsync(1, true);

        repository.Verify(r => r.EditAsync(It.Is<DomainQuot>(q => q.IsFavourite), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsFavouriteAsync_WhenEditFails_ThrowsNotFound()
    {
        var quot = DomainQuot.Create("Author", "Text", "Theme", false, false);
        var repository = new Mock<IQuotRepository>();
        repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(quot);
        repository.Setup(r => r.EditAsync(quot, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PsychologyApp.Application.Exceptions.PersistenceException("not updated"));

        var service = new QuotService(repository.Object, Mock.Of<IQuotContentProvider>());

        await Assert.ThrowsAsync<PsychologyApp.Application.Exceptions.PersistenceException>(
            () => service.MarkAsFavouriteAsync(1, true));
    }
}
